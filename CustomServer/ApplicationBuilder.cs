namespace CustomServeer;

public delegate Task RequestDelegate(HttpContext context);
public class ApplicationBuilder
{
    public List<Func<RequestDelegate, RequestDelegate>> _components = new();

    public ApplicationBuilder Use(Func<HttpContext, Func<Task>, Task> middleware)
    {
        _components.Add(next => context => middleware(context, ()=> next(context)));
        return this;
    }

    public RequestDelegate Build()
    {
        RequestDelegate pipeline = (context) => Task.CompletedTask;
        foreach (var component in _components.AsEnumerable().Reverse())
        {
            pipeline = component.Invoke(pipeline);
        }
        return pipeline;
    }
}