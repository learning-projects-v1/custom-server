using System.Reflection;
using CustomHttp.Middlewares;

namespace CustomHttp;


public static class ApplicationBuilderMiddlewareExtensions
{
    public static ApplicationBuilder UseMiddleware<T>(
        this ApplicationBuilder app)
        where T : IMiddleware, new()
    {
        app.Use(async (context, next) =>
        {
            var middleware = new T(); // later: DI
            await middleware.InvokeAsync(context, next);
        });
        return app;
    }
}   
