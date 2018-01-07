﻿#include "pch.h"
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

	__declspec(dllexport) int __cdecl StreamCypher(UINT8* output, UINT8* input, long inputLength, UINT8 nonce[], UINT8 secret[]) {
		return crypto_stream_xor(output, input, inputLength, nonce, secret);
	}

}
