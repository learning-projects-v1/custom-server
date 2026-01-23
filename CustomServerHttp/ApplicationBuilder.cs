using CustomHttp;

namespace CustomServerHttp;

public delegate Task RequestDelegate(HttpContext context);

public interface IApplicationBuilder
{
    void Use(Func<HttpContext, RequestDelegate, Task> middleware);
    RequestDelegate Build();
}

public class ApplicationBuilder : IApplicationBuilder
{
    private readonly List<Func<RequestDelegate, RequestDelegate>> _container = new();

    public void Use(Func<HttpContext, RequestDelegate, Task> middleware)
    {
        _container.Add(next => context => middleware(context, next));
    }

    public RequestDelegate Build()
    {
        RequestDelegate pipeline = context => Task.CompletedTask;
        foreach (var container in _container.AsEnumerable().Reverse()) pipeline = container.Invoke(pipeline);
        return pipeline;
    }
}