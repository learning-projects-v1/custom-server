namespace CustomServeer;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class HttpMethodAttribute: Attribute
{
    public string Path { get; set; }
    public string Method { get; set; }

    public HttpMethodAttribute(string method, string path)
    {
        Path = path;
        Method = method;
    }
}

public class HttpGetAttribute : HttpMethodAttribute
{
    public HttpGetAttribute(string path) : base("GET", path) { }
}

public class HttpPostAttribute : HttpMethodAttribute
{
    public HttpPostAttribute(string path) : base("POST", path)
    {
    }
}

public class HttpPutAttribute : HttpMethodAttribute
{
    public HttpPutAttribute(string path) : base("PUT", path)
    {
    }
}

public class HttpDeleteAttribute : HttpMethodAttribute
{
    public HttpDeleteAttribute(string path) : base("DELETE", path)
    {
    }
}