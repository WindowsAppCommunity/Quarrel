#pragma once
// Hack since generic is a custom keyword in C++/cli but the header files use it as a variable name 
#define generic genericA
#include <call/call.h>
#include <media/engine/webrtcvoiceengine.h>
#include <modules/rtp_rtcp/include/rtp_header_parser.h>
#include <api/audio_codecs/builtin_audio_encoder_factory.h>
#include <api/audio_codecs/builtin_audio_decoder_factory.h>
#undef generic
#include <iostream>
#include <IAudioDeviceWasapi.h>
#include <sodium/crypto_secretbox.h>

namespace Webrtc
{
	public ref class WebrtcManager sealed
	{
	public:
		WebrtcManager::WebrtcManager();
		void Create();
		void ConnectAsync(Platform::String^ ip, Platform::String^ port, UINT32 ssrc);
		void WebrtcManager::SendSelectProtocol(UINT32 ssrc);
		event Windows::Foundation::TypedEventHandler<Platform::String^, USHORT>^ IpAndPortObtained;
		void SetKey(const Platform::Array<BYTE>^ key);
		void SetSpeaking(UINT32 ssrc, int speaking);

	private:
		void WebrtcManager::CreateVoe();
		void WebrtcManager::CreateCall();
		void WebrtcManager::SetupCall();
		webrtc::AudioSendStream* WebrtcManager::createAudioSendStream(uint32_t ssrc, uint8_t payloadType);
		webrtc::AudioReceiveStream* WebrtcManager::createAudioReceiveStream(uint32_t local_ssrc, uint32_t remote_ssrc, uint8_t payloadType);
		Windows::Networking::Sockets::DatagramSocket^ udpSocket;
		Windows::Storage::Streams::DataWriter^ outputStream;
		void OnMessageReceived(Windows::Networking::Sockets::DatagramSocket^ sender, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs^ args);
		unsigned char key[32];
		uint32_t ssrc;

		webrtc::Call* g_call = nullptr;

		std::map<short, webrtc::AudioReceiveStream*> audioReceiveStreams;
		webrtc::AudioSendStream* audioSendStream;

		rtc::scoped_refptr<webrtc::AudioDecoderFactory> g_audioDecoderFactory;
		rtc::scoped_refptr<webrtc::AudioEncoderFactory> g_audioEncoderFactory;

		cricket::WebRtcVoiceEngine* g_engine;

		int g_audioSendChannelId = -1;
		int g_audioReceiveChannelId = -1;
		int g_videoSendChannelId = -1;
		int g_videoReceiveChannelId = -1;

		class AudioLoopbackTransport;
		class VideoLoopbackTransport;
		webrtc::Transport* g_audioSendTransport = nullptr;

		std::unique_ptr<rtc::Thread> workerThread;

	};


	class StreamTransport : public webrtc::Transport
	{
	public:
		StreamTransport(webrtc::Call* call, Windows::Storage::Streams::DataWriter^ sendStream);
		virtual bool SendRtp(const uint8_t* packet, size_t length, const webrtc::PacketOptions& options);
		virtual bool SendRtcp(const uint8_t* packet, size_t length);
	private:
		webrtc::Call* call;
		Windows::Storage::Streams::DataWriter^ sendStream;
	};
}