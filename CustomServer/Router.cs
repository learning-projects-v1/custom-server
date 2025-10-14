using System.Net;
using System.Reflection;
using System.Text.Json;

namespace CustomServeer;

public class Router
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
                        var controllerInstance = Activator.CreateInstance(controllerType, Array.Empty<object>());
                        var result = method.Invoke(controllerInstance, Array.Empty<object>());

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


