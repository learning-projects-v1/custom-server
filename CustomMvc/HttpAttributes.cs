namespace CustomMvc;

[AttributeUsage(AttributeTargets.Method)]
public abstract class AHttpAttribute : Attribute
{
    public string Path { get; }
    public string Method { get; }

    public AHttpAttribute(string method, string path)
    {
        Method = method.ToUpperInvariant();
        Path = NormalizePath(path);
    }
    
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "/";

        return path.StartsWith('/') ? path : "/" + path;
    }
}

public class HttpGetAttribute : AHttpAttribute
{

    public HttpGetAttribute(string path): base("GET", path)
    {
        
    }
}

public class HttpPostAttribute : AHttpAttribute
{
    public HttpPostAttribute(string path):  base("POST", path)
    {

    }
}
