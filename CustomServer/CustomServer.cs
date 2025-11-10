using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomServer;
public class CustomServer
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly IPAddress _ipAddress = IPAddress.Any;
    public CustomServer(int port = 8000)
    {
        // step 1: start the server
        // step 1.1: open OS socket to listen for incoming requests on a ipEndpoint
        _port = port;
        _listener = new TcpListener(_ipAddress, _port);
    }
    
    public void Start()
    {
        _listener.Start();
        Console.WriteLine($"Listening on {_ipAddress}:{_port}");
        
        // step 1.2: accept each request and send them for handling asyncrhounously
        while (true)
        {
            var client = _listener.AcceptTcpClient();
            var _ = Task.Run(()=> HandleRequest(client));
        }
    }

    private async Task HandleRequest(TcpClient client)
    {
        // step 2: parse request to create HttpContext
        using (client)
        {
            // step 2.1: read header
            await using var stream = client.GetStream();
            var buffer = new byte[4096];
            var requestString = new StringBuilder();
            var totalBytesRead = 0;
            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer);     
                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                requestString.Append(request);
                if (request.IndexOf("\r\n\r\n") != -1)      /// todo: handle when stream is chunked or comes in multiple packets
                {
                    break;
                }
            }

            var httpContext = new HttpContext(requestString.ToString());
            string httpResponse =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain\r\n" +
                "Content-Length: "+ 50 + "\r\n" +
                "\r\n" +
                "<h1>Ki ase jibone<h1></br><p>Kisu nai:(</p>\r\n";

            var responseBytes = Encoding.UTF8.GetBytes(httpResponse);
            await stream.WriteAsync(responseBytes);

            client.Close();
            // step 2.2: parse httpContext from request string
        
            // step 3: handle pipeline
        
            // step 4: map response
        }
   
        
        
    }
}