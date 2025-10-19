#include <stdint.h>  // for int32_t

// Use extern "C" to prevent C++ name mangling
extern "C" {

    // Simple addition function
    int32_t CalculateSum(int32_t a, int32_t b)
    {
        return a + b;
    }

} // extern "C"