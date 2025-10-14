using System.Text;
using System.Text.Json;

namespace CustomServeer;

public class JsonActionResult : IActionResult
{
    private readonly object _data;
    private readonly int _statusCode;
    
    public JsonActionResult(object data, int statusCode)
    {
        _data = data;
        _statusCode = statusCode;
    }
    
    public void Execute(HttpContext context)
    {
        var responseMessage = JsonSerializer.Serialize(_data);
        context.Response.StatusCode = _statusCode;
        context.Response.SetHeader("Content-Type", "application/json");
        context.Response.Body = responseMessage;
        context.Response.SetHeader("Content-Length", responseMessage.Length.ToString());
    }
}

public class TextActionResult : IActionResult
{
    private readonly string _data;
    private readonly int _statusCode;

    public TextActionResult(string data, int statusCode)
    {
        _data = data;
        _statusCode = statusCode;
    }
    
    public void Execute(HttpContext context)
    {
        context.Response.StatusCode = _statusCode;
        context.Response.Body =  _data;
        context.Response.SetHeader("Content-Type", "text/plain");
        context.Response.SetHeader("Content-Length", _data.Length.ToString());
    }
}