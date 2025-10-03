
using System.Net.Sockets;

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
        // TODO release managed resources here
    }
}

public class HttpRequest
{
    public string RequestPath { get; set; }
    public string RequestMethod { get; set; }
    public string HttpVersion { get; set; }
    public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public byte[] RequestBody;
    
}

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string StatusText { get; set; } = "Ok";
    public string ContentType { get; set; } = "text/plain";
    public string ResponseBody;
    public Dictionary<string, string> ResponseHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool HasStarted { get; set; }

    public void StartResponse()
    {
        HasStarted = true;
    }
    
}