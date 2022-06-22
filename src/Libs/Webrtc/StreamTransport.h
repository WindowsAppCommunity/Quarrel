#pragma once
#include <api/call/transport.h>

#include "WebrtcManager.h"

namespace Webrtc
{
	class StreamTransport : public webrtc::Transport
	{
	public:
		StreamTransport(winrt::Webrtc::implementation::WebrtcManager* manager);
		virtual bool SendRtp(const uint8_t* packet, size_t length, const webrtc::PacketOptions& options);
		virtual bool SendRtcp(const uint8_t* packet, size_t length);
		void StopSend();
		void StartSend();
	private:
		winrt::Webrtc::implementation::WebrtcManager* manager;
		bool is_sending = false;
	};
}
