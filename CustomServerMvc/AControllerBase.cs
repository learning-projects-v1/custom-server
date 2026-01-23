using CustomServerHttp;

namespace CustomServerMvc;

public abstract class AControllerBase
{
    public HttpContext Context
    {
        get; internal set;
    }
    
    protected HttpResponse Ok(string msg) => new () {Body = msg,  StatusCode = 200, StatusText = "ok"};
    protected HttpResponse NotFound(string msg) => new () {Body = msg,  StatusCode = 404, StatusText = "Not found!"};
    protected HttpResponse BadRequest(string msg) => new () {Body = msg,  StatusCode = 400, StatusText = "Bad Request"};
    
    protected JsonActionResult Json(object data, int status = 200) => new JsonActionResult(data, status);
    protected TextActionResult Text(string data, int status = 200) => new TextActionResult(data, status);

}