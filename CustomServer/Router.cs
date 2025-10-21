using System.Net;
using System.Reflection;
using System.Text.Json;

namespace CustomServeer;

public interface IRouter
{
    void MapControllers(Assembly assembly);
    public RequestDelegate Build();
}
public class Router : IRouter
{
    private readonly Dictionary<(string method, string path), RequestDelegate> _routes = new(); // method, path 
    public void MapControllers(Assembly assembly)
    {
        var controllerTypes = assembly
            .GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            foreach (var method in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var httpMethodAttribute = method.GetCustomAttributes<HttpMethodAttribute>();
                foreach (var attribute in httpMethodAttribute)
                {
                    
                    _routes.Add((attribute.Method.ToUpper(), attribute.Path.ToLower()) , ( async context =>
                    {
                        var parameters = attribute.Path.Split('/');
                        var controllerInstance = Activator.CreateInstance(controllerType, Array.Empty<object>());
                        var result = method.Invoke(controllerInstance, Array.Empty<object>());

                        MapResponse(result, context);

                        Console.WriteLine($"Mapped {attribute.Method} and {attribute.Path} -> {controllerType.Name}:{method.Name}");
                    }));
                }
            }
        }
    }

    private static void MapResponse(object? result, HttpContext context)
    {
        switch (result)
        {
            case HttpResponse response:
                context.Response =  response;
                break;
            case IActionResult actionResult:
                actionResult.Execute(context);
                break;
            default:
                var json = JsonSerializer.Serialize(result);
                context.Response.Body = json;
                break;
        }
    }

    public RequestDelegate Build()
    {
        return context =>
        {
            if (_routes.TryGetValue((context.Request.Method, context.Request.Path.ToLower()), out var handler))
            {
                handler(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Body = "Route not found";
            }
            return Task.CompletedTask;
        };
    }
}


public class RouteItem
{
    public string RouteSegment { get; set; } = "";
    public Dictionary<string, RouteItem> Children { get; set; } = new();
    public RequestDelegate? Handler { get; set; }
    public RouteItem? ParameterChild { get; set; } // 1 child can exist with parameter i.e api/user/{username}/info
    public bool IsLeaf { get; set; }
}

public class TrieBasedRouter : IRouter
{
    public RouteItem RootItem { get; set; } = new();

    private string[] GetSegments(string path)
    {
        return path.Trim('/', '/').Split("/");
    }
    public void Add(string path, RequestDelegate handler)
    {
        var current = RootItem;
        var segments = GetSegments(path);
        foreach (var segment in segments)
        {
            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                if (current.ParameterChild is not null)
                {
                    current =  current.ParameterChild;
                }
                else
                {
                    current.ParameterChild = new RouteItem{RouteSegment = segment.Trim('{', '}'), Handler = handler};
                    current = current.ParameterChild;
                }
            }
            else
            {
                if (current.Children.TryGetValue(segment, out var child))
                {
                    current = child;
                }
                else
                {
                    var routeItem = new RouteItem
                    {
                        RouteSegment = segment,
                        Handler = handler,
                    };
                    current.Children.Add(segment, routeItem);
                    current = routeItem;
                }
            }
        }
        current.IsLeaf = true;
    }


    private RequestDelegate? GetRouteRec(string[] segments, int idx, RouteItem current)
    {
        if (idx == segments.Length)
        {
            if(current.IsLeaf) return current.Handler;
            return null;
        }

        RequestDelegate? handler = null;
        if (current.Children.TryGetValue(segments[idx], out var child)) // first try direct match
        {
            handler = GetRouteRec(segments, idx + 1, child);
        }

        if (handler == null && current.ParameterChild != null)    /// match this with parameter {}
        {
            handler = GetRouteRec(segments, idx + 1, current.ParameterChild);
        }
        return handler;
    }
    
    public RequestDelegate? GetRoute(string path)
    {
        var segments = GetSegments(path.ToLower());
        Console.WriteLine("Getting route for path: " + path);
        var handler = GetRouteRec(segments, 0, RootItem);
        return handler;

    }
    public void MapControllers(Assembly assembly)
    {
        var controllerTypes = assembly.GetTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
        foreach (var controllerType in controllerTypes)
        {
            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                var methodParams = method.GetParameters();
                var httpMethodAttributes = method.GetCustomAttributes<HttpMethodAttribute>();
                foreach (var attribute in httpMethodAttributes)
                {
                    Add(attribute.Path.ToLower(), context =>
                    {
                        var parameters = GetParameters(attribute.Path, context.Request.Path, methodParams);
                        var controllerInstance = Activator.CreateInstance(controllerType, Array.Empty<object>());
                        var result = method.Invoke(controllerInstance, parameters.ToArray());
                        MapResponse(result, context);
                        return Task.CompletedTask;
                    });
                }
            }
        }
    }

    private object[] GetParameters(string attributePath, string requestPath, ParameterInfo[] methodParams)
    {
        var methodSegments = GetSegments(attributePath.ToLower());
        var requestSegments = GetSegments(requestPath.ToLower());
        var requestParameters = new Dictionary<string, object>();
        for(int i = 0; i < methodSegments.Length; i++)
        {
            var segment = methodSegments[i];
            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                requestParameters.Add(segment.Trim('{', '}'), requestSegments[i]);
            }
        }

        Console.WriteLine("Request Parameters: " + string.Join(", ", requestParameters));
        var result = new object[methodSegments.Length];
        var index = 0;
        foreach(var methodParam in methodParams)
        {
            if (requestParameters.TryGetValue(methodParam.Name, out var requestParameter))
            {
                var parameter = Convert.ChangeType(requestParameter, methodParam.ParameterType);
                result[index++] = parameter;
            }
        }
        
        return result;
    }

    public RequestDelegate Build()
    {
        return (context) =>
        {
            var path = context.Request.Path;
            var handler = GetRoute(path);
            if (handler != null)
            {
                handler(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Body = "Route not found";
            }
            return Task.CompletedTask;
        };
    }
    
    private static void MapResponse(object? result, HttpContext context)
    {
        switch (result)
        {
            case HttpResponse response:
                context.Response =  response;
                break;
            case IActionResult actionResult:
                actionResult.Execute(context);
                break;
            default:
                var json = JsonSerializer.Serialize(result);
                context.Response.Body = json;
                break;
        }
    }
}
