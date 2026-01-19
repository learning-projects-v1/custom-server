using CustomMvc;

namespace Test;
public class MyController : AControllerBase
{
    [HttpGet("/")]
    public IActionResult GetRoot()
    {
        return Text("HELLO WORLD FROM ROOT");
    }

    [HttpGet("/a/b/c")]
    public IActionResult GetABC()
    {
        return Text("HELLO WORLD FROM ABC");
    }

    [HttpGet("/a/b/{id}/d")]
    public IActionResult GetABCD(int id)
    {
        return Text("HELLO WORLD FROM ABCD." + " id = " + id);
    }

    [HttpGet("/a/b/{msg}/c/d")]
    public IActionResult GetABCDMsg(string msg)
    {
        return Text("HELLO WORLD FROM ABCD." + msg);
    }
}