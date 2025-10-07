#include <stdint.h>

// Use extern "C" to prevent name mangling, which is vital for P/Invoke
// extern "C" __declspec(dllexport) //
typedef ServerConfig{
    int32_t MaxMessageSize;
    char ServerName[64];
};

extern "C" {
    
    int32_t CalculateSum(int32_t a, int32_t b)
    {
        return a + b;
    }
    
    int32_t CheckAccess(int32_t userId)
    {
        if (userId == 100)
            return 0;
        else
            return -1;
    }

    
    const char* GetUserName(int32_t userId) 
    {
        if (userId == 100)
            return "Alice Johnson";
        else
            return "Unknown User";
    }

    int32_t ApplyConfig(ServerConfig config)
    {
        
    }
}
