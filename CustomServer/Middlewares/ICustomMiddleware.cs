namespace CustomServeer.Middlewares;

public interface ICustomMiddleware
{
    public Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next);
}