


using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    const int port = 8000;
    private static IPAddress _ipAddress = IPAddress.Any;
    
    public async Task StartServer()
    {
        var listener = new TcpListener(_ipAddress, port);
        listener.Start();
        Console.WriteLine($"Server started on {_ipAddress}:{port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            var ipEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Client connected on {ipEndPoint.Address.ToString()}:{ipEndPoint.Port}");
            var _ = Task.Run(()=> HandleClientAsync(client)) ;
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            await using var stream = client.GetStream();
            var buffer = new Byte[2048];
            var totalBytesRead = 0;
            Memory<byte> memory = buffer;
            while (totalBytesRead < buffer.Length)
            {
                var byteCount = await stream.ReadAsync(memory.Slice(totalBytesRead));
                if (byteCount == 0) break;
                totalBytesRead += byteCount;
            }
            Console.WriteLine($"Total received {totalBytesRead} bytes from client");
        
            var requestString = Encoding.ASCII.GetString(memory.Span);
            var newLinePos = requestString.IndexOf("\r\n");
            if (newLinePos == -1)
            {
                return;
            }
        
            var requestParts = requestString.Substring(0, newLinePos).Split(' ');
            var method = requestParts[0];
            var path = requestParts[1];
            var version = requestParts[2];
            Console.WriteLine($"Parsed Request: Method='{method}', Path='{path}'");
        

            var responseMessage = "HTTP/1.1 200 OK\r\n" + "Content-Type: text/plain\r\n" + "\r\n" +"Hello from custom server.\r\n";
            await stream.WriteAsync(Encoding.ASCII.GetBytes(responseMessage));

            int sum = NativeLogic.CalculateSum(10, 12);
            Console.WriteLine($"Sum: {sum}");
        }
        
    }
}