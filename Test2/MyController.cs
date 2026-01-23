using CustomServerMvc;
using Test;

namespace Test2;
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

    [HttpPost("/a/b")]
    public IActionResult PostAb(UserDto model)
    {
        Console.WriteLine($"Got model: {model.Name}, {model.Email}, {model.Age}");
        return Json(model);
    }

    [HttpDelete("/a/{id}/b")]
    public IActionResult DeleteAb(string id)
    {
        return Text($"Delete request with Id: {id}");
    }

    [HttpPut("/a/b")]
    public IActionResult PutAb(UserDto model)
    {
        return Text($"Put request with model: {model.Name}, {model.Email}, {model.Age}");
    }
    
}