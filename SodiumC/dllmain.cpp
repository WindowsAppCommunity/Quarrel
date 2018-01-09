#include "pch.h"
#include "sodium.h"

BOOL APIENTRY DllMain(HMODULE /* hModule */, DWORD ul_reason_for_call, LPVOID /* lpReserved */)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
		if (_sodium_alloc_init() == -1) {
			return false;
		}
		break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" {

	__declspec(dllexport) int __cdecl Encrypt(UINT8* output, UINT8* input, long inputLength, UINT8 nonce[], UINT8 secret[]) {
		return crypto_secretbox_easy(output, input, inputLength, nonce, secret);
	}

	__declspec(dllexport) int __cdecl Decrypt(UINT8* output, UINT8* input, long inputLength, UINT8 nonce[], UINT8 secret[]) {
		return crypto_secretbox_open_easy(output, input, inputLength, nonce, secret);
	}
}
