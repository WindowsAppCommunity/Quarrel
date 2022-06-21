#pragma once
#include "WebrtcManager.g.h"

#include <winrt/Windows.Networking.Sockets.h>
#include <winrt/Windows.Storage.Streams.h>
#include <call/call.h>
#include <api/task_queue/default_task_queue_factory.h>
#include <media/engine/webrtc_voice_engine.h>
#include <rtc_base/thread.h>
#include <api/audio_codecs/builtin_audio_decoder_factory.h>
#include <modules/audio_device/include/audio_device.h>
#include <modules/audio_processing/audio_buffer.h>

#include <iostream>

namespace Webrtc
{
	class AudioSourceAnalyzer;
	class StreamTransport;
}

namespace winrt::Webrtc::implementation
{
	struct WebrtcManager : WebrtcManagerT<WebrtcManager>
	{
	public:

		WebrtcManager();
		WebrtcManager(hstring outputDeviceId, hstring inputDeviceId);
		~WebrtcManager() override;

		void Create();
		void Destroy();

		winrt::fire_and_forget Connect(hstring ip, hstring port, UINT32 ssrc);
		void SendSelectProtocol(UINT32 ssrc) const;

		void SetKey(array_view<const BYTE> key);
		void SetSpeaking(UINT32 ssrc, int speaking);
		
		IpAndPortObtainedDelegate IpAndPortObtained() noexcept;
		AudioOutDataDelegate AudioOutData() noexcept;
		AudioInDataDelegate AudioInData() noexcept;
		SpeakingDelegate Speaking() noexcept;

		void IpAndPortObtained(IpAndPortObtainedDelegate value) noexcept;
		void AudioOutData(AudioOutDataDelegate value) noexcept;
		void AudioInData(AudioInDataDelegate value) noexcept;
		void Speaking(SpeakingDelegate value) noexcept;

		void UpdateInBytes(Windows::Foundation::Collections::IVector<float> const& data) const;
		void UpdateOutBytes(Windows::Foundation::Collections::IVector<float> const& data) const;

		void UpdateSpeaking(bool speaking);

		void SetPlaybackDevice(hstring deviceId);
		void SetRecordingDevice(hstring deviceId);
	private:
		friend class ::Webrtc::StreamTransport;
		friend class ::Webrtc::AudioSourceAnalyzer;

		void CreateCall();
		void SetupCall();

		uint16_t WebrtcManager::GetPlaybackDeviceIndexFromId(std::string device_id) const;
		uint16_t WebrtcManager::GetRecordingDeviceIndexFromId(std::string device_id) const;

		webrtc::AudioSendStream* CreateAudioSendStream(uint32_t ssrc, uint8_t payload_type) const;
		webrtc::AudioReceiveStream* CreateAudioReceiveStream(uint32_t local_ssrc, uint32_t remote_ssrc, uint8_t payload_type) const;
		void OnMessageReceived(Windows::Networking::Sockets::DatagramSocket const& sender, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs const& args);

		unsigned char key[32];
		uint32_t ssrc = 0;

		Windows::Networking::Sockets::DatagramSocket udp_socket{ nullptr };
		Windows::Storage::Streams::DataWriter output_stream{ nullptr };

		std::unique_ptr<webrtc::Call> call;
		
		std::map<short, webrtc::AudioReceiveStream*> audio_receive_streams{};
		webrtc::AudioSendStream* audio_send_stream{ nullptr };

		rtc::scoped_refptr<webrtc::AudioDecoderFactory> audio_decoder_factory;
		rtc::scoped_refptr<webrtc::AudioEncoderFactory> audio_encoder_factory;

		std::unique_ptr<::Webrtc::StreamTransport> audio_send_transport;
		
		bool is_speaking = false;
		int frame_count = 0;;
		int16_t output_device_index, input_device_index;
		std::string output_device_id, input_device_id;

		bool has_got_ip = false;
		bool connected = false;

		rtc::scoped_refptr<webrtc::AudioDeviceModule> audio_device;

		rtc::scoped_refptr<webrtc::AudioProcessing> audio_processing;

		std::map<uint32_t, uint32_t> ssrc_to_create;

		std::unique_ptr<webrtc::TaskQueueFactory> task_queue_factory;
		rtc::TaskQueue task_queue;

		IpAndPortObtainedDelegate ipAndPortObtained;
		AudioOutDataDelegate audioOutData;
		AudioInDataDelegate audioInData;
		SpeakingDelegate speaking;
	};
}

namespace winrt::Webrtc::factory_implementation
{
	struct WebrtcManager : WebrtcManagerT<WebrtcManager, implementation::WebrtcManager>
	{
	};
}