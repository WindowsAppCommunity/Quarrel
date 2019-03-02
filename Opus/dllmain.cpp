#include "pch.h"
#include "opus/opus.h"

BOOL APIENTRY DllMain(HMODULE /* hModule */, DWORD ul_reason_for_call, LPVOID /* lpReserved */)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" {
	__declspec(dllexport) OpusEncoder* _cdecl OpusEncoderCreate(int Fs, int channels, int application, int* error) {
		return opus_encoder_create(Fs, channels, application, error);
	}

	__declspec(dllexport) void OpusEncoderDestroy(OpusEncoder* encoder) {
		opus_encoder_destroy(encoder);
	}

	__declspec(dllexport) int OpusEncode(OpusEncoder* st, opus_int16* pcm, int frame_size, unsigned char* data, opus_int32 max_data_bytes) {
		return opus_encode(st, pcm, frame_size, data, max_data_bytes);
	}

	__declspec(dllexport) OpusDecoder* _cdecl OpusDecoderCreate(int Fs, int channels, int* error) {
		return opus_decoder_create(Fs, channels, error);
	}

	__declspec(dllexport) void OpusDecoderDestroy(OpusDecoder* decoder) {
		opus_decoder_destroy(decoder);
	}

	__declspec(dllexport) int OpusDecode(OpusDecoder* st, unsigned char* data, int len, opus_int16* pcm, int frame_size, int decode_fec) {
		return opus_decode(st, data, len, pcm, frame_size, decode_fec);
	}

	__declspec(dllexport) int OpusEncoderCtl(OpusEncoder* st, int request, int value) {
		return opus_encoder_ctl(st, request, value);
	}

	__declspec(dllexport) int OpusDecoderCtl(OpusDecoder* st, int request, int* value) {
		return opus_decoder_ctl(st, request, value);
	}
}