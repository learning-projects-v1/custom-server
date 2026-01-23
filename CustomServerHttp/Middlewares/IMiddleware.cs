using CustomServerHttp;

namespace CustomHttp.Middlewares;

public interface IMiddleware
{
    Task InvokeAsync(HttpContext context, RequestDelegate next);
}