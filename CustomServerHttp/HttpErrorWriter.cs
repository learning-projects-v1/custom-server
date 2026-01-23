using System.Net.Sockets;
using System.Text;

namespace CustomServerHttp;

public static class HttpErrorWriter
{
    public static async Task WriteBadRequestAsync(NetworkStream stream, string? reason = null)
    {
        reason ??= "Bad Request";

        var body = $"400 Bad Request\n{reason}";
        var bodyBytes = Encoding.UTF8.GetBytes(body);

        var response =
            "HTTP/1.1 400 Bad Request\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/plain\r\n" +
            $"Content-Length: {bodyBytes.Length}\r\n" +
            "\r\n";

        await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
        await stream.WriteAsync(bodyBytes);
    }
}