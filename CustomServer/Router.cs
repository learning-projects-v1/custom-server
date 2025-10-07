namespace CustomServeer;

public class Router
{
    private readonly Dictionary<string, RequestDelegate> _routes = new();

    public void Register(string route, RequestDelegate func)
    {
        _routes.Add(route, func);
    }

    public RequestDelegate GetRoute(string route)
    {
        if (_routes.TryGetValue(route, out var func))
        {
            return func;
        }
        return null;
    }

    public RequestDelegate Build()
    {
        return context =>
        {
            if (_routes.TryGetValue(context.Request.Path, out var handler))
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


