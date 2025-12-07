namespace CustomMvc;

[AttributeUsage(AttributeTargets.Method)]
public abstract class AHttpAttribute : Attribute
{
    public string _path;
}


public class HttpGetAttribute : AHttpAttribute
{
    public HttpGetAttribute(string path)
    {
        _path = path;
    }
}