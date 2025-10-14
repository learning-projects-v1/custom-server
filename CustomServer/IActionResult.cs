namespace CustomServeer;

public interface IActionResult
{
    public void Execute(HttpContext context);   
}