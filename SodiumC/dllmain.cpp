#include "pch.h"

#define SodiumC_EXPORTS  
#ifdef SodiumC_EXPORTS  
#define SodiumC_API __declspec(dllexport)  
#else  
#define TRADITIONALDLL_API __declspec(dllimport)  
#endif 
//TODO: Basic function call

extern "C" {
	SodiumC_API int add(int a, int b);
}

 int add(int a, int b) {
	return a + b;
}

//BOOL APIENTRY DllMain(HMODULE /* hModule */, DWORD ul_reason_for_call, LPVOID /* lpReserved */)
//{
//    switch (ul_reason_for_call)
//    {
//    case DLL_PROCESS_ATTACH:
//    case DLL_THREAD_ATTACH:
//    case DLL_THREAD_DETACH:
//    case DLL_PROCESS_DETACH:
//        break;
//    }
//    return TRUE;
//}

