using System.Runtime.InteropServices;

namespace CustomServeer.Middlewares;

public class NativeLogic
{
    private const string DLL_NAME = "b.dylib";
    [DllImport(DLL_NAME,CallingConvention = CallingConvention.Cdecl)]
    public static extern int CalculateSum(int a, int b);
    
    [DllImport(DLL_NAME)]
    public static extern int CheckAccess(int userId);
    
    [DllImport(DLL_NAME, CharSet = CharSet.Ansi)]
    public static extern string GetUserName(int userId);
    
    [DllImport(DLL_NAME, CharSet = CharSet.Ansi)]
    public static extern int ApplyConfig(ServerConfig config);

    [DllImport(DLL_NAME, CharSet = CharSet.Ansi)]
    public static extern IntPtr GenerateJsonResponse(int userId);
    
    [DllImport(DLL_NAME, CharSet = CharSet.Ansi)]
    public static extern void FreeStringMemory(IntPtr memory);
    
    public Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next)
    {
        return default;
    }
}