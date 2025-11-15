using System.Text;

namespace CustomHttp;

public class HttpContext
{
    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }

    public HttpContext(string httpRequestString)
    {
        Request = new HttpRequest(httpRequestString);
        Response = new HttpResponse();
    }
}

public class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public string[] PathSegments { get; set; }
    public Dictionary<string, string> QueryParams { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; }

    public HttpRequest(string requestString)
    {
        var lineParts = requestString.Split("\r\n");
        if (lineParts.Length < 1) return;
        var httpRequestLine = lineParts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (httpRequestLine.Length < 3 || !httpRequestLine[2].Contains("HTTP")) return;
        Method = httpRequestLine[0];
        Path = httpRequestLine[1];

        var headers = lineParts.Skip(1).TakeWhile(x => x != "").ToList();
        Headers = headers.ToDictionary(h => h.Split(':', StringSplitOptions.TrimEntries).First(), h => h.Split(':', StringSplitOptions.RemoveEmptyEntries).Last());
        PathSegments = ParsePathSegments(Path);
        QueryParams = ParseQueryParams(Path);
    }

    private string[] ParsePathSegments(string path)
    {
        return path.Split("?", StringSplitOptions.RemoveEmptyEntries)[0].Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    private Dictionary<string, string> ParseQueryParams(string path)
    {
        var pathSplit = path.Split("?", StringSplitOptions.RemoveEmptyEntries);
        if (pathSplit.Length < 2) return new();
        var queryString = pathSplit[1].Split('&',  StringSplitOptions.RemoveEmptyEntries);
        return queryString.ToDictionary(q => q.Split('=').First(), q => q.Split('=').Last());
    }
}

public class HttpResponse
{
    public int StatusCode { get; set; }
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
        {
            response
                .Append(responseHeader.Key + ": " + responseHeader.Value)
                .Append("\r\n");
        }

        response.Append("\r\n");
        response.Append(Body);
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
}

