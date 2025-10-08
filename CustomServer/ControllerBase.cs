namespace CustomServeer;

public abstract class ControllerBase
{
    public HttpContext Context
    {
        get; internal set;
    }
    
    public HttpResponse Ok(string msg) => new () {Body = msg,  StatusCode = 200, StatusText = "ok"};
    public HttpResponse NotFound(string msg) => new () {Body = msg,  StatusCode = 404, StatusText = "Not found!"};
    public HttpResponse BadRequest(string msg) => new () {Body = msg,  StatusCode = 400, StatusText = "Bad Request"};

}