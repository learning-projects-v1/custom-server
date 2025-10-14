using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace CustomServeer;

[StructLayout(LayoutKind.Sequential)] 
public struct ServerConfig
{
    // int32_t maps directly to C# int
    public int MaxMessageSize; 

    // 2. Use ByValTStr and SizeConst to match the C++ 'char ServerName[64]' fixed array
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] 
    public string ServerName;
}

public static class NativeLogic
{
    private static int Counter = 0;
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


    public static void Increment()
    {
        Counter++;
    }

    public static void Decrement()
    {
        Counter--;
    }
    public static int GetCounter()
    {
        return Counter;
    }

}