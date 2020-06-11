#include "pch.h"
#include "WebrtcManager.h"

class MyRtcEventLogOutput : public webrtc::RtcEventLogOutput
{
public:
	explicit MyRtcEventLogOutput() {}
	virtual ~MyRtcEventLogOutput() {}
	bool IsActive() const { return true; }


	bool Write(const std::string& output) {
		std::cout << output;
		return true;
	}

	void Flush() {}
};

void WINAPIV DebugOut(const TCHAR* fmt, ...) {
	TCHAR s[1025];
	va_list args;
	va_start(args, fmt);
	vswprintf(s, 1025, fmt, args);
	va_end(args);
	OutputDebugString(s);
}

const webrtc::SdpAudioFormat kOpusFormat = { "opus", 48000, 2, { {"stereo", "1"}, {"usedtx", "1"}, {"useinbandfec", "1" } } };

namespace Webrtc
{

	class AudioAnalyzer : public webrtc::CustomAudioAnalyzer {
	private:
		winrt::Webrtc::implementation::WebrtcManager* manager;
	public:
		AudioAnalyzer() {}
		AudioAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager)
		{
			this->manager = manager;
		}
		void Initialize(int sample_rate_hz, int num_channels) override {}
		void Analyze(const webrtc::AudioBuffer* audio) override {
			const webrtc::ChannelBuffer<float>* channel = audio->data_f();
			std::vector<float> data(channel->num_frames());

			webrtc::DownmixToMono<float, float>(channel->channels(), channel->num_frames(), channel->num_channels(), data.data());

			double sum = 0;

			for(int i = 0; i < data.size(); i++)
			{
				const double sample = data[i] / 32768.0;
				sum += sample * sample;
			}

			double decibels = 10 * log10(2 * sum / data.size());

			this->manager->SetCurrentVolume(decibels);
			
			manager->UpdateInBytes(winrt::single_threaded_vector<float>(std::move(data)));
		};
		std::string ToString() const override { return "AudioAnalyzer"; }
	};

	class OutputAnalyzer : public webrtc::CustomProcessing
	{
	private:
		winrt::Webrtc::implementation::WebrtcManager* manager;
	public:
		OutputAnalyzer() {}
		OutputAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager)
		{
			this->manager = manager;
		}
		void Initialize(int sample_rate_hz, int num_channels) override {}
		void Process(webrtc::AudioBuffer* audio) override {
			const webrtc::ChannelBuffer<float>* channel = audio->data_f();
			std::vector<float> data(channel->num_frames());

			webrtc::DownmixToMono<float, float>(channel->channels(), channel->num_frames(), channel->num_channels(), data.data());

			manager->UpdateOutBytes(winrt::single_threaded_vector<float>(std::move(data)));
		};
		void SetRuntimeSetting(webrtc::AudioProcessing::RuntimeSetting setting) override {}
		std::string ToString() const override { return "OutputAnalyzer"; }
	};

	StreamTransport::StreamTransport(winrt::Webrtc::implementation::WebrtcManager* manager) : manager(manager)
	{
		//this->call->SignalChannelNetworkState(webrtc::MediaType::VIDEO, webrtc::NetworkState::kNetworkUp);
	}

	void StreamTransport::StopSend()
	{
		isSending = false;
	}

	void StreamTransport::StartSend()
	{
		isSending = true;
	}

	bool StreamTransport::SendRtp(const uint8_t* packet, size_t length, const webrtc::PacketOptions& options)
	{
		if (!this->isSending) return true;
		uint8_t nonce[24]{ 0 };
		memcpy(nonce, packet, 12);
		
		std::vector<uint8_t> encrypted(length + 16);

		memcpy(encrypted.data(), packet, 12);

		
		crypto_secretbox_easy(encrypted.data() + 12, packet + 12, length - 12, nonce, this->manager->key);
		
		this->manager->outputStream.WriteBytes(encrypted);
		this->manager->outputStream.StoreAsync();
		return true;
	}

	bool StreamTransport::SendRtcp(const uint8_t* packet, size_t length)
	{
		uint8_t nonce[24]{ 0 };
		memcpy(nonce, packet, 8);

		std::vector<uint8_t> encrypted(length + 16);

		memcpy(encrypted.data(), packet, 8);


		crypto_secretbox_easy(encrypted.data() + 8, packet + 8, length - 8, nonce, this->manager->key);

		this->manager->outputStream.WriteBytes(encrypted);
		this->manager->outputStream.StoreAsync();
		return true;
	}
}

namespace winrt::Webrtc::implementation
{
	WebrtcManager::WebrtcManager()
	{
	}

	WebrtcManager::WebrtcManager(hstring outputDeviceId, hstring inputDeviceId)
	{
		output_device_id = to_string(outputDeviceId);
		input_device_id = to_string(inputDeviceId);
	}

	void WebrtcManager::Create()
	{
		workerThread = rtc::Thread::Create();
		workerThread->Start();

		workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
			this->SetupCall();
		});

	}
	
	void WebrtcManager::SetupCall()
	{
		this->CreateCall();
		this->g_audioSendTransport = new ::Webrtc::StreamTransport(this);
		this->audioSendStream = this->createAudioSendStream(this->ssrc, 120);
		this->g_call->SignalChannelNetworkState(webrtc::MediaType::AUDIO, webrtc::NetworkState::kNetworkUp);
	}

	void WebrtcManager::Destroy()
	{
		this->m_connected = false;
		this->ssrc_to_create.clear();
		if (this->audioReceiveStreams.size() > 0) {
			for (auto it = this->audioReceiveStreams.cbegin(); it != this->audioReceiveStreams.cend();)
			{
				this->workerThread->Invoke<void>(RTC_FROM_HERE, [this, it]() {
					this->g_call->DestroyAudioReceiveStream(it->second);
				});
				this->audioReceiveStreams.erase(it++);
			}
		}
		if (this->audioSendStream) {
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				this->g_call->DestroyAudioSendStream(this->audioSendStream);
			});
		}
		
		if (this->outputStream) this->outputStream.Close();
		this->outputStream = nullptr;
		if (this->udpSocket) this->udpSocket.Close();
		this->udpSocket = nullptr;
		
		delete this->g_audioSendTransport;
		this->g_audioSendTransport = nullptr;
		
		this->audioSendStream = nullptr;

		if (this->workerThread)
		{
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				delete this->g_call;
			  this->audioDevice = nullptr;
			});
		}
		this->g_call = nullptr;
		if(this->workerThread) this->workerThread.reset();

		this->g_audioDecoderFactory = nullptr;
		
		this->g_audioEncoderFactory = nullptr;
		
	}
	
	void WebrtcManager::CreateCall()
	{
		this->g_audioDecoderFactory = webrtc::CreateBuiltinAudioDecoderFactory();
		this->g_audioEncoderFactory = webrtc::CreateBuiltinAudioEncoderFactory();
		
		::Webrtc::IAudioDeviceWasapi::CreationProperties props;
		props.id_ = "";
		props.playoutEnabled_ = true;
		props.recordingEnabled_ = true;
		
		webrtc::AudioState::Config stateconfig;

		stateconfig.audio_processing = webrtc::AudioProcessingBuilder()
			.SetCaptureAnalyzer(std::make_unique<::Webrtc::AudioAnalyzer>(this))
			.SetRenderPreProcessing(std::make_unique<::Webrtc::OutputAnalyzer>(this))
			.Create();

		this->audioDevice = ::Webrtc::IAudioDeviceWasapi::create(props);
		
		stateconfig.audio_device_module = this->audioDevice;

		stateconfig.audio_mixer = webrtc::AudioMixerImpl::Create();

		rtc::scoped_refptr<webrtc::AudioState> audio_state = webrtc::AudioState::Create(stateconfig);

		RTC_CHECK_EQ(0, this->audioDevice->Init()) << "Failed to initialize the ADM.";

		if(this->audioDevice->PlayoutDevices() > 0)
		{
			int deviceIndex = GetPlaybackDeviceIndexFromId(output_device_id);

			if (deviceIndex == -1) deviceIndex = 0;
			
			if (this->audioDevice->SetPlayoutDevice(deviceIndex) != 0) {
				RTC_LOG(LS_ERROR) << "Unable to set playout device.";
				return;
			}
			if (this->audioDevice->InitSpeaker() != 0) {
				RTC_LOG(LS_ERROR) << "Unable to access speaker.";
			}

			// Set number of channels
			bool available = false;
			if (this->audioDevice->StereoPlayoutIsAvailable(&available) != 0) {
				RTC_LOG(LS_ERROR) << "Failed to query stereo playout.";
			}
			if (this->audioDevice->SetStereoPlayout(available) != 0) {
				RTC_LOG(LS_ERROR) << "Failed to set stereo playout mode.";
			}
		}

		if (this->audioDevice->RecordingDevices() > 0)
		{
			int deviceIndex = GetRecordingDeviceIndexFromId(input_device_id);

			if (deviceIndex == -1) deviceIndex = 0;

			if (this->audioDevice->SetRecordingDevice(deviceIndex) != 0) {
				RTC_LOG(LS_ERROR) << "Unable to set recording device.";
				return;
			}
			if (this->audioDevice->InitMicrophone() != 0) {
				RTC_LOG(LS_ERROR) << "Unable to access microphone.";
			}

			// Set number of channels
			bool available = false;
			if (this->audioDevice->StereoRecordingIsAvailable(&available) != 0) {
				RTC_LOG(LS_ERROR) << "Failed to query stereo recording.";
			}
			if (this->audioDevice->SetStereoRecording(available) != 0) {
				RTC_LOG(LS_ERROR) << "Failed to set stereo recording mode.";
			}
		}
		
		webrtc::apm_helpers::Init(stateconfig.audio_processing);
		this->audioDevice->RegisterAudioCallback(audio_state->audio_transport());
		
		std::unique_ptr<webrtc::RtcEventLog> log = webrtc::RtcEventLog::Create(webrtc::RtcEventLog::EncodingType::Legacy);
		std::unique_ptr<webrtc::RtcEventLogOutput> output = std::make_unique<MyRtcEventLogOutput>();
		log->StartLogging(std::move(output), 100);

		webrtc::Call::Config config(log.release());
		
		config.audio_state = audio_state;
		config.audio_processing = stateconfig.audio_processing;
		
		this->g_call = webrtc::Call::Create(config);
		this->m_connected = true;
		for (auto const& [ssrc, speaking] : this->ssrc_to_create)
		{
			if(speaking != 0)
			{
				this->audioReceiveStreams[ssrc] = this->createAudioReceiveStream(this->ssrc, ssrc, 120);
			}
		}
	}

	WebrtcManager::~WebrtcManager()
	{
		this->Destroy();
	}

	webrtc::AudioSendStream* WebrtcManager::createAudioSendStream(uint32_t ssrc, uint8_t payloadType)
	{
		webrtc::AudioSendStream::Config config{ this->g_audioSendTransport };
		config.rtp.ssrc = ssrc;
		config.rtp.extensions = { {"urn:ietf:params:rtp-hdrext:ssrc-audio-level", 1} };
		config.encoder_factory = g_audioEncoderFactory;
		config.send_codec_spec = webrtc::AudioSendStream::Config::SendCodecSpec(payloadType, kOpusFormat);

		webrtc::AudioSendStream* audioStream = g_call->CreateAudioSendStream(config);
		audioStream->Start();
		return audioStream;
	}

	webrtc::AudioReceiveStream* WebrtcManager::createAudioReceiveStream(uint32_t local_ssrc, uint32_t remote_ssrc, uint8_t payloadType)
	{
		webrtc::AudioReceiveStream::Config config;
		config.rtp.local_ssrc = local_ssrc;
		config.rtp.remote_ssrc = remote_ssrc;
		config.rtp.extensions = { {"urn:ietf:params:rtp-hdrext:ssrc-audio-level", 1} };
		config.decoder_factory = g_audioDecoderFactory;
		config.decoder_map = { { payloadType, kOpusFormat } };
		config.rtcp_send_transport = this->g_audioSendTransport;

		webrtc::AudioReceiveStream* audioStream = g_call->CreateAudioReceiveStream(config);
		audioStream->Start();

		return audioStream;
	}

	void WebrtcManager::SetSpeaking(UINT32 ssrc, int speaking)
	{
		// Todo: handle priority speaker
		if (!this->m_connected)
		{
			this->ssrc_to_create[ssrc] = speaking;
		}
		else
		{
			auto it = this->audioReceiveStreams.find(ssrc);
			if (speaking != 0)
			{
				if (it == this->audioReceiveStreams.end())
				{
					this->audioReceiveStreams[ssrc] = this->workerThread->Invoke<webrtc::AudioReceiveStream*>(RTC_FROM_HERE, [this, ssrc]() {
						return this->createAudioReceiveStream(this->ssrc, ssrc, 120);
					});
				}
				else
				{
					// How?
				}
			}
			else
			{
				if (it != audioReceiveStreams.end())
				{
					this->workerThread->Invoke<void>(RTC_FROM_HERE, [this, it]() {
						this->g_call->DestroyAudioReceiveStream(it->second);
					});
					audioReceiveStreams.erase(it);
				}
			}
		}
	}

	void WebrtcManager::SetCurrentVolume(double volume)
	{
		if (volume < -40)
		{
			// Not speaking
			if (this->isSpeaking)
			{
				this->frameCount++;
				if (this->frameCount >= 50) {
					this->isSpeaking = false;
					this->g_audioSendTransport->StopSend();
					this->m_speaking(*this, false);
					this->frameCount = 0;
				}
			}
		}
		else
		{
			// Are Speaking
			if (!this->isSpeaking)
			{
				this->isSpeaking = true;
				this->g_audioSendTransport->StartSend();
				this->m_speaking(*this, true);
			}
		}
	}

	int WebrtcManager::GetPlaybackDeviceIndexFromId(std::string deviceId) const
	{
		char target_id[webrtc::kAdmMaxGuidSize];

		int target_device_index = -1;

		for (unsigned int i = 0; i < this->audioDevice->PlayoutDevices(); ++i) {

			char name[webrtc::kAdmMaxDeviceNameSize];
			char guid[webrtc::kAdmMaxGuidSize];
			this->audioDevice->PlayoutDeviceName(i, name, guid);
			if (deviceId == guid)
			{
				return i;
			}
		}
		return -1;
	}

	int WebrtcManager::GetRecordingDeviceIndexFromId(std::string deviceId) const
	{
		char target_id[webrtc::kAdmMaxGuidSize];

		int target_device_index = -1;

		for (unsigned int i = 0; i < this->audioDevice->RecordingDevices(); ++i) {

			char name[webrtc::kAdmMaxDeviceNameSize];
			char guid[webrtc::kAdmMaxGuidSize];
			this->audioDevice->RecordingDeviceName(i, name, guid);
			if (deviceId == guid)
			{
				return i;
			}
		}
		return -1;
	}

	void WebrtcManager::SetPlaybackDevice(hstring deviceId) {
		output_device_id = to_string(deviceId);

		if (!this->audioDevice) return;

		bool wasRecording = false;
		
		if(this->audioDevice->SpeakerIsInitialized())
		{
			wasRecording = true;
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				this->audioDevice->StopPlayout();
			});
		}

		const int target_device_index = GetPlaybackDeviceIndexFromId(output_device_id);
		
		if(target_device_index > -1)
		{
			this->audioDevice->SetPlayoutDevice(target_device_index);
		}

		if(wasRecording)
		{
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				this->audioDevice->InitPlayout();
				this->audioDevice->StartPlayout();
			});
		}
	}

	void WebrtcManager::SetRecordingDevice(hstring deviceId) {
		input_device_id = to_string(deviceId);
		
		if (!this->audioDevice) return;

		bool wasRecording = false;

		if (this->audioDevice->MicrophoneIsInitialized())
		{
			wasRecording = true;
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				this->audioDevice->StopRecording();
			});
		}

		const int target_device_index = GetRecordingDeviceIndexFromId(input_device_id);
		
		if (target_device_index > -1)
		{
			this->audioDevice->SetRecordingDevice(target_device_index);
		}

		if (wasRecording)
		{
			this->workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
				this->audioDevice->InitRecording();
				this->audioDevice->StartRecording();
			});
		}
	}
	
	void WebrtcManager::SetKey(array_view<const BYTE> key)
	{
		memcpy(this->key, key.begin(), 32);
	}

	void WebrtcManager::UpdateInBytes(Windows::Foundation::Collections::IVector<float> const& data)
	{
		this->m_audioInData(*this, data);
	}

	void WebrtcManager::UpdateOutBytes(Windows::Foundation::Collections::IVector<float> const& data)
	{
		this->m_audioOutData(*this, data);
	}

	Windows::Foundation::IAsyncAction WebrtcManager::ConnectAsync(hstring ip, hstring port, UINT32 ssrc)
	{
		this->hasGotIp = false;
		this->ssrc = ssrc;
		
		udpSocket = Windows::Networking::Sockets::DatagramSocket();
		udpSocket.MessageReceived({ this, &WebrtcManager::OnMessageReceived });

		co_await this->udpSocket.ConnectAsync(Windows::Networking::HostName(ip), port);

		this->outputStream = Windows::Storage::Streams::DataWriter(this->udpSocket.OutputStream());
		this->SendSelectProtocol(ssrc);
		this->Create();
	}

	void WebrtcManager::SendSelectProtocol(UINT32 ssrc)
	{
		std::array<unsigned char, 70> packet;
		packet[0] = (BYTE)(ssrc >> 24);
		packet[1] = (BYTE)(ssrc >> 16);
		packet[2] = (BYTE)(ssrc >> 8);
		packet[3] = (BYTE)(ssrc >> 0);
		this->outputStream.WriteBytes(packet);
		this->outputStream.StoreAsync();
	}

	void WebrtcManager::OnMessageReceived(Windows::Networking::Sockets::DatagramSocket const& sender, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs const& args)
	{
		Windows::Storage::Streams::DataReader dr = args.GetDataReader();
		unsigned int dataLength = dr.UnconsumedBufferLength();
		if (this->hasGotIp)
		{
			std::vector<BYTE> bytes = std::vector<BYTE>(dataLength);
			dr.ReadBytes(bytes);

			BYTE nonce[24] = { 0 };
			BYTE* decrypted = new BYTE[dataLength - 16];
			if (webrtc::RtpHeaderParser::IsRtcp(bytes.data(), dataLength))
			{
				memcpy(nonce, bytes.data(), 8);
				memcpy(decrypted, bytes.data(), 8);
				crypto_secretbox_open_easy(decrypted + 8, bytes.data() + 8, dataLength - 8, nonce, key);

				workerThread->Invoke<void>(RTC_FROM_HERE, [this, decrypted, dataLength]() {
					rtc::PacketTime pTime = rtc::CreatePacketTime(0);
					webrtc::PacketReceiver::DeliveryStatus status = this->g_call->Receiver()->DeliverPacket(webrtc::MediaType::ANY, rtc::CopyOnWriteBuffer(decrypted, dataLength - 16), pTime.timestamp);
					delete decrypted;
				});
			}
			else
			{

				memcpy(nonce, bytes.data(), 12);
				memcpy(decrypted, bytes.data(), 12);

				crypto_secretbox_open_easy(decrypted + 12, bytes.data() + 12, dataLength - 12, nonce, key);
				
				workerThread->Invoke<void>(RTC_FROM_HERE, [this, decrypted, dataLength]() {
					rtc::PacketTime pTime = rtc::CreatePacketTime(0);
					webrtc::PacketReceiver::DeliveryStatus status = this->g_call->Receiver()->DeliverPacket(webrtc::MediaType::AUDIO, rtc::CopyOnWriteBuffer(decrypted, dataLength - 16), pTime.timestamp);
					delete decrypted;
				});
			}

		}
		else {
			this->hasGotIp = true;
			dr.ReadInt32();
			std::array<BYTE, 64> bytes{};
			dr.ReadBytes(bytes);

			std::wstring ip(std::begin(bytes), std::end(bytes));
			USHORT port = dr.ReadUInt16();
			this->m_ipAndPortObtained(hstring(ip), port);
		}
	}

	
#pragma region Events
	void WebrtcManager::IpAndPortObtained(event_token const& token) noexcept
	{
		this->m_ipAndPortObtained.remove(token);
	}

	event_token WebrtcManager::IpAndPortObtained(Windows::Foundation::TypedEventHandler<hstring, USHORT> const& handler)
	{
		return this->m_ipAndPortObtained.add(handler);
	}

	void WebrtcManager::AudioOutData(event_token const& token) noexcept
	{
		this->m_audioOutData.remove(token);
	}

	event_token WebrtcManager::AudioOutData(Windows::Foundation::EventHandler<Windows::Foundation::Collections::IVector<float>> const& handler)
	{
		return this->m_audioOutData.add(handler);
	}

	void WebrtcManager::AudioInData(event_token const& token) noexcept
	{
		this->m_audioOutData.remove(token);
	}

	event_token WebrtcManager::AudioInData(Windows::Foundation::EventHandler<Windows::Foundation::Collections::IVector<float>> const& handler)
	{
		return this->m_audioInData.add(handler);
	}

	void WebrtcManager::Speaking(event_token const& token) noexcept
	{
		this->m_speaking.remove(token);
	}

	event_token WebrtcManager::Speaking(Windows::Foundation::EventHandler<bool> const& handler)
	{
		return this->m_speaking.add(handler);
	}
#pragma endregion

}
