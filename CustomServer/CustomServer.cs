using System.Net;
using System.Net.Sockets;
using System.Text;
using CustomHttp;
using CustomMvc;

namespace CustomServer;
public class CustomServer
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly IPAddress _ipAddress = IPAddress.Any;
    private readonly RequestDelegate _pipeline;
    public CustomServer(RequestDelegate pipeline, int port = 8000)
    {
        // step 1: start the server
        // step 1.1: open OS socket to listen for incoming requests on a ipEndpoint
        _port = port;
        _listener = new TcpListener(_ipAddress, _port);
        _pipeline = pipeline;
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
            try
            {
                var buffer = new byte[4096];
                var headerBuffer = new MemoryStream();
                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0)
                        throw new Exception("Client disconnected");
                    headerBuffer.Write(buffer, 0, bytesRead);
                    
                    if (headerBuffer.Length >= 4)
                    {
                        var data = headerBuffer.ToArray();
                        var headerEnd = FindHeaderEnd(data);
                        if (headerEnd != -1)
                        {
                            break;
                        }
                    }
                }
            
                var allBytes = headerBuffer.ToArray();
                var headerEndIndex = FindHeaderEnd(allBytes); // index after \r\n\r\n

                var headerBytes = allBytes[..headerEndIndex];
                var remainingBytes = allBytes[headerEndIndex..];

                var requestString = Encoding.UTF8.GetString(headerBytes);
              
                // step 2.2: parse httpContext from request string
                var httpContext = new HttpContext(requestString.ToString());
                
            
                // step 2.3: body
                if (httpContext.Request.Headers.TryGetValue("Content-Length", out var lenStr))
                {
                    var contentLength = int.Parse(lenStr);
                    var body = new byte[contentLength];

                    var alreadyRead = Math.Min(remainingBytes.Length, contentLength);
                    Array.Copy(remainingBytes, body, alreadyRead);

                    var remaining = contentLength - alreadyRead;
                    var offset = alreadyRead;

                    while (remaining > 0)
                    {
                        var read = await stream.ReadAsync(body, offset, remaining);
                        if (read == 0)
                            throw new Exception("Client disconnected");

                        offset += read;
                        remaining -= read;
                    }

                    httpContext.Request.Body = body;
                }
                var bodyString = Encoding.UTF8.GetString(httpContext.Request.Body);
                // step 3: handle pipeline
                await _pipeline(httpContext);
                
                // step 4: map response
                var response = httpContext.Response.GetResponse();
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes);
                
            }
            // catch (HttpParseException ex)
            // {
            //     await HttpErrorWriter.WriteBadRequestAsync(stream, ex.Message);
            // }
            catch (Exception ex)
            {
                await HttpErrorWriter.WriteBadRequestAsync(stream, ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
   
        
        
    }
    
    static int FindHeaderEnd(byte[] data)
    {
        for (int i = 0; i <= data.Length - 4; i++)
        {
            if (data[i] == 13 && data[i + 1] == 10 &&
                data[i + 2] == 13 && data[i + 3] == 10)
                return i + 4;
        }

        return -1;
    }

}