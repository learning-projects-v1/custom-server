using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomServeer;
public class CustomServer
{
    private readonly RequestDelegate _pipeline;
    private static IPAddress _ipAddress = IPAddress.Any;
    private readonly TcpListener _listener;
    private readonly int _port;
    public CustomServer(RequestDelegate pipeline, int port = 8000)
    {
        _port = port;
        _pipeline = pipeline;
        _listener = new TcpListener(_ipAddress, port);
        
    }
    
    public async Task StartServer()
    {
        _listener.Start();
        Console.WriteLine($"Server started on {_ipAddress}:{_port}");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            var ipEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Client connected on {ipEndPoint.Address.ToString()}:{ipEndPoint.Port}");
            var _ = Task.Run(()=> HandleClientAsync(client)) ;
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            await using var stream = client.GetStream();
            var buffer = new Byte[4096];
            var totalBytesRead = 0;
            Memory<byte> memory = buffer;
            StringBuilder requestString = new StringBuilder();
            while (totalBytesRead < buffer.Length)
            {
                var byteCount = await stream.ReadAsync(memory.Slice(totalBytesRead));
                var currentString = Encoding.UTF8.GetString(memory.Span.Slice(totalBytesRead, byteCount));
                requestString.Append(currentString);
                totalBytesRead += byteCount;
                
                if (currentString.IndexOf("\r\n") != -1 || currentString.Length == 0) break;
            }
            Console.WriteLine($"Total received {totalBytesRead} bytes from client");
        
            // var requestString = Encoding.ASCII.GetString(memory.Span);
            var httpContext = ParseHttpContext(client, requestString.ToString());
            await _pipeline(httpContext);
            
            var responseMessage = httpContext.GetResponse();
            //var responseMessage = $"HTTP/1.1 {httpContext.Response.StatusCode} OK\r\nContent-Type: text/plain\r\n\r\n{httpContext.Response.Body} \r\n";
            await stream.WriteAsync(Encoding.ASCII.GetBytes(responseMessage));
            //
            // int sum = NativeLogic.CalculateSum(10, 12);
            // Console.WriteLine($"Sum: {sum}");
        }
        
    }

    private static HttpContext ParseHttpContext(TcpClient client, string request)
    {
        var httpContext = new HttpContext(client);
        var lineParts = request.Split("\r\n");
        if (lineParts.Length < 2)
        {
            throw new Exception("Invalid HTTP request");        // refactor after fixing proper response pattern
        }
        var HttpMethodLine = lineParts[0].Split(" ");
        var method = HttpMethodLine[0];
        var path = HttpMethodLine[1];
        var version = HttpMethodLine[2];
        
        httpContext.Request = new HttpRequest
        {
            Method = method,
            Path = path,
            Version = version,
        };
        
        int bodyStart = -1;
        var requestHeaders = httpContext.Request.Headers;
        for (int i = 1; i < lineParts.Length; i++)
        {
            if (lineParts[i] == "\r\n" || lineParts[i] == "\n")
            {
                bodyStart = i + 1;
                break;
            }

            var header = lineParts[i].Split(":");
            if (header.Length < 2)
            {
                bodyStart = i + 1;
                break;
            }
            requestHeaders.Add(header[0].Trim(), header[1].Trim());
        }
        
        httpContext.Request.RequestBody = lineParts.Length > bodyStart ? lineParts[bodyStart].Trim(): "";
        
        Console.WriteLine($"Parsed Request: Method='{method}', Path='{path}'");
        
        return httpContext;
    }
    
}