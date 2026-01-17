using CustomMvc;

namespace Test;
public class MyController : AControllerBase
{
    [HttpGet("/")]
    public IActionResult GetRoot()
    {
        return Text("HELLO WORLD FROM ROOT");
    }
}