#include "pch.h"
#include "StreamTransport.h"

#include <winrt/Windows.Foundation.Collections.h>
#include <sodium/crypto_secretbox.h>

namespace Webrtc
{
	StreamTransport::StreamTransport(winrt::Webrtc::implementation::WebrtcManager* manager) : manager(manager)
	{
		//this->call->SignalChannelNetworkState(webrtc::MediaType::VIDEO, webrtc::NetworkState::kNetworkUp);
	}

	void StreamTransport::StopSend()
	{
		is_sending = false;
		const webrtc::PacketOptions options;
		constexpr uint8_t silence[] = { 0xF8, 0xFF, 0xFE };
		for (int i = 0; i < 5; i++) {
			SendRtp(silence, sizeof(silence), options);
		}
	}
	
	void StreamTransport::StartSend()
	{
		is_sending = true;
	}

	bool StreamTransport::SendRtp(const uint8_t* packet, size_t length, const webrtc::PacketOptions& options)
	{
		if (!this->is_sending) return true;
		uint8_t nonce[24]{ 0 };
		memcpy(nonce, packet, 12);

		std::vector<uint8_t> encrypted(length + 16);

		memcpy(encrypted.data(), packet, 12);

		crypto_secretbox_easy(encrypted.data() + 12, packet + 12, length - 12, nonce, this->manager->key);

		this->manager->output_stream.WriteBytes(encrypted);
		(void)this->manager->output_stream.StoreAsync();
		return true;
	}

	bool StreamTransport::SendRtcp(const uint8_t* packet, size_t length)
	{
		uint8_t nonce[24]{ 0 };
		memcpy(nonce, packet, 8);

		std::vector<uint8_t> encrypted(length + 16);

		memcpy(encrypted.data(), packet, 8);

		crypto_secretbox_easy(encrypted.data() + 8, packet + 8, length - 8, nonce, this->manager->key);

		this->manager->output_stream.WriteBytes(encrypted);
		(void)this->manager->output_stream.StoreAsync();
		return true;
	}
}
