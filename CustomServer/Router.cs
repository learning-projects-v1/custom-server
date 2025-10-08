using System.Reflection;

namespace CustomServeer;

public class Router
{
    private readonly Dictionary<(string method, string path), RequestDelegate> _routes = new(); // method, path 

    // public void Register(string route, RequestDelegate func)
    // {
    //     _routes.Add(route, func);
    // }

    // public RequestDelegate GetRoute(string route)
    // {
    //     if (_routes.TryGetValue(route, out var func))
    //     {
    //         return func;
    //     }
    //     return null;
    // }

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
                    _routes.Add((attribute.Method, attribute.Path) , ( async context =>
                    {
                        var controllerInstance = Activator.CreateInstance(controllerType, Array.Empty<object>());
                        var result = method.Invoke(controllerInstance, Array.Empty<object>());
                        if (result is HttpResponse response)
                        {
                            context.Response = response;
                        }
                        else if (result is string str)
                        {
                            context.Response = new() {Body = str, StatusCode = 200, StatusText = "ok"};
                        }

                        Console.WriteLine($"Mapped {attribute.Method} and {attribute.Path} -> {controllerType.Name}:{method.Name}");
                    }));
                }
            }
        }
    }
    public RequestDelegate Build()
    {
        return context =>
        {
            if (_routes.TryGetValue((context.Request.Method, context.Request.Path), out var handler))
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


