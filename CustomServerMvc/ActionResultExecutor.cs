using System.Text.Json;
using CustomServerHttp;

namespace CustomServerMvc;

public static class ActionResultExecutor
{
    public static void Execute(object? result, HttpContext context)
    {
        if (result == null)
        {
            context.Response.StatusCode = 204;
            return;
        }

        switch (result)
        {
            case IActionResult actionResult:
                actionResult.Execute(context);
                break;

            case HttpResponse response:
                context.Response = response;
                break;
            
            default:
                var json = JsonSerializer.Serialize(result);
                context.Response.Body = json;
                break;
        }
    }
}
