using System.Net.Sockets;

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
    public string ContentType { get; set; } = "text/plain";
    public string Body { get; set; } = String.Empty;
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool HasStarted { get; set; }

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
    
    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}   