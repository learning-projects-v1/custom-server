namespace CustomServeer.Routes;

public class InitialController : ControllerBase
{
    [HttpGet("/api/Initial")]
    public string GetInitialGet()
    {
        return "Initial Get Success";
    }
    
    [HttpPost("/api/Increment")]
    public void Increment()
    {
        NativeLogic.Increment();
    }

    [HttpPost("/api/Decrement")]
    public void Decrement()
    {
        NativeLogic.Decrement();
    }

    [HttpGet("/api/Counter")]
    public int GetCounter()
    {
        return NativeLogic.GetCounter();
    }

    [HttpGet("/api/json")]
    public IActionResult GetJson()
    {
        return Json(new {a = 23, b = 34, c = "Hello world", d = "Ki ase jibone"});
    }

    [HttpGet("api/json/{name}/{age}")]
    public IActionResult GetJson(string name, int age)
    {
        return Json(new {name = name, age = age});
    }
}
