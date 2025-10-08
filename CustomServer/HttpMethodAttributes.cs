namespace CustomServeer;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class HttpMethodAttribute: Attribute
{
    public string Path { get; set; }
    public string Method { get; set; }

    public HttpMethodAttribute(string method, string path)
    {
        path = path;
        Method = method;
    }
}

public class HttpGetAttribute : HttpMethodAttribute
{
    public HttpGetAttribute(string method, string path) : base(method, path) { }
}

public class HttpPostAttribute : HttpMethodAttribute
{
    public HttpPostAttribute(string method, string path) : base(method, path)
    {
    }
}

public class HttpPutAttribute : HttpMethodAttribute
{
    public HttpPutAttribute(string method, string path) : base(method, path)
    {
    }
}

public class HttpDeleteAttribute : HttpMethodAttribute
{
    public HttpDeleteAttribute(string method, string path) : base(method, path)
    {
    }
}