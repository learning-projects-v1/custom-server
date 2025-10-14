using System.Net.Sockets;
using System.Text;

namespace CustomServeer;
public class HttpRequest
{
    public string Method { get; set; } = String.Empty;
    public string Path { get; set; } = String.Empty;
    public string Version { get; set; } = String.Empty;
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public string RequestBody { get; set; } = String.Empty;
}

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string StatusText { get; set; } = "Ok";
    public string Body { get; set; } = String.Empty;
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool HasStarted { get; set; }

    public void SetHeader(string key, string value)
    {
        Headers[key] = value;
    }
    public void StartResponse()
    {
        HasStarted = true;
    }
}

public class HttpContext : IDisposable
{
    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }
    public TcpClient Client { get; set; }

    public HttpContext(TcpClient client){
        Request = new HttpRequest();
        Response = new HttpResponse();
        if (client == null)
        {
            throw new ArgumentNullException("client");
        }
        Client = client;
    }

    public string GetResponse()
    {
        var response = new StringBuilder();
        response.Append($"{Request.Version} {Response.StatusCode} {GetStatusText(Response.StatusCode)}\r\n");
        if (!Response.Headers.ContainsKey("Date"))
            Response.Headers["Date"] = DateTime.UtcNow.ToString("R");

        if (!Response.Headers.ContainsKey("Server"))
            Response.Headers["Server"] = "CustomServer/1.0";
        
        foreach (var responseHeader in Response.Headers)
        {
            response
                .Append(responseHeader.Key + ": " + responseHeader.Value)
                .Append("\r\n");
        }

        response.Append("\r\n");
        response.Append(Response.Body);
        return response.ToString();
    }
    
    
    private string GetStatusText(int code) => code switch
    {
        200 => "OK",
        201 => "Created",
        204 => "No Content",
        301 => "Moved Permanently",
        302 => "Found",
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "Unknown"
    };
    
    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}   