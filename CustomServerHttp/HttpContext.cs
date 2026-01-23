using System.Text;

namespace CustomServerHttp;

public class HttpContext
{
    public HttpContext(string httpRequestString)
    {
        Request = new HttpRequest(httpRequestString);
        Response = new HttpResponse();
    }

    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }
}

public class HttpRequest
{
    public HttpRequest(string requestString)
    {
        var lineParts = requestString.Split("\r\n");
        if (lineParts.Length < 1) return;
        var httpRequestLine = lineParts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (httpRequestLine.Length < 3 || !httpRequestLine[2].Contains("HTTP")) return;
        Method = httpRequestLine[0];
        Path = httpRequestLine[1];

        var headers = lineParts.Skip(1).TakeWhile(x => x != "").ToList();
        Headers = headers.ToDictionary(h => h.Split(':', StringSplitOptions.TrimEntries).First(),
            h => h.Split(':', StringSplitOptions.TrimEntries).Last());
        var pathSplit = Path.Split("?", StringSplitOptions.RemoveEmptyEntries);
        PathSegments = ParsePathSegments(pathSplit[0]);
        if (pathSplit.Length > 1) QueryParams = ParseQueryParams(pathSplit[1]);
    }

    public string Method { get; set; }
    public string Path { get; set; }
    public string[] PathSegments { get; set; }
    public Dictionary<string, string> QueryParams { get; set; } = new();

    public Dictionary<string, string> Headers { get; set; } = new();

    // public string Body { get; set; }
    public byte[] Body { get; set; }

    private string[] ParsePathSegments(string path)
    {
        return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    private Dictionary<string, string> ParseQueryParams(string path)
    {
        var queryString = path.Split('&', StringSplitOptions.RemoveEmptyEntries);
        return queryString.ToDictionary(q => q.Split('=').First(), q => q.Split('=').Last());
    }
}

public class HttpResponse
{
    public int StatusCode { get; set; }
    public string StatusText { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; }
    public string Version { get; set; } = "HTTP/1.0";

    public string GetResponse()
    {
        var response = new StringBuilder();
        response.Append($"{Version} {StatusCode} {GetStatusText(StatusCode)}\r\n");
        if (!Headers.ContainsKey("Date"))
            Headers["Date"] = DateTime.UtcNow.ToString("R");

        if (!Headers.ContainsKey("Server"))
            Headers["Server"] = "CustomServer/1.0";

        foreach (var responseHeader in Headers)
            response
                .Append(responseHeader.Key + ": " + responseHeader.Value)
                .Append("\r\n");

        response.Append("\r\n");
        response.Append(Body);
        return response.ToString();
    }


    public void SetHeader(string key, string value)
    {
        Headers[key] = value;
    }

    private string GetStatusText(int code)
    {
        return code switch
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
    }
}