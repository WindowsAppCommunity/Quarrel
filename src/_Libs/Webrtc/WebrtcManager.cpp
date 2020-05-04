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
			manager->UpdateInBytes(winrt::single_threaded_vector<float>(std::move(data)));
		};
		std::string ToString() const override { return "AudioAnalyzer"; }
	};

	StreamTransport::StreamTransport(webrtc::Call* call, winrt::Windows::Storage::Streams::DataWriter const& sendStream) : call(call), sendStream(sendStream)
	{
		this->call->SignalChannelNetworkState(webrtc::MediaType::AUDIO, webrtc::NetworkState::kNetworkUp);
		//this->call->SignalChannelNetworkState(webrtc::MediaType::VIDEO, webrtc::NetworkState::kNetworkUp);
	}

	bool StreamTransport::SendRtp(const uint8_t* packet, size_t length, const webrtc::PacketOptions& options)
	{
		return true;
	}

	bool StreamTransport::SendRtcp(const uint8_t* packet, size_t length)
	{
		sendStream.WriteBytes(winrt::array_view(packet, packet + length));
		sendStream.StoreAsync();
		return true;
	}
}

namespace winrt::Webrtc::implementation
{

	void WebrtcManager::UpdateInBytes(Windows::Foundation::Collections::IVector<float> const& data)
	{
		this->m_audioInData(*this, data);
	}

	void WebrtcManager::UpdateOutBytes(Windows::Foundation::Collections::IVector<float> const& data)
	{
		this->m_audioOutData(*this, data);
	}
	
	void WebrtcManager::CreateVoe()
	{
		this->g_audioDecoderFactory = webrtc::CreateBuiltinAudioDecoderFactory();
		this->g_audioEncoderFactory = webrtc::CreateBuiltinAudioEncoderFactory();

		rtc::scoped_refptr<webrtc::AudioDeviceModule> audioDeviceModule;
		::Webrtc::IAudioDeviceWasapi::CreationProperties props;
		props.id_ = "";
		props.playoutEnabled_ = true;
		props.recordingEnabled_ = true;
		auto adm = rtc::scoped_refptr<webrtc::AudioDeviceModule>(::Webrtc::IAudioDeviceWasapi::create(props));
		::Webrtc::AudioAnalyzer* tmp = new ::Webrtc::AudioAnalyzer(this);
		rtc::scoped_refptr<webrtc::AudioProcessing> apm = webrtc::AudioProcessingBuilder().SetCaptureAnalyzer(std::unique_ptr<::Webrtc::AudioAnalyzer>(tmp)).Create();

		this->g_engine = new cricket::WebRtcVoiceEngine(adm,
			g_audioEncoderFactory,
			g_audioDecoderFactory,
			NULL,
			apm);

		this->g_engine->Init();
	}


	void WebrtcManager::CreateCall()
	{
		webrtc::AudioState::Config stateconfig;
		rtc::scoped_refptr<webrtc::AudioState> audio_state = this->g_engine->GetAudioState();


		std::unique_ptr<webrtc::RtcEventLog> log = webrtc::RtcEventLog::Create(webrtc::RtcEventLog::EncodingType::Legacy);
		std::unique_ptr<webrtc::RtcEventLogOutput> output = std::make_unique<MyRtcEventLogOutput>();
		log->StartLogging(std::move(output), 100);

		webrtc::Call::Config config(log.release());
		config.audio_state = audio_state;
		config.audio_processing = NULL;
		this->g_call = webrtc::Call::Create(config);
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
		auto it = audioReceiveStreams.find(ssrc);
		if (speaking != 0)
		{
			if (it == audioReceiveStreams.end())
			{
				this->audioReceiveStreams[ssrc] = workerThread->Invoke<webrtc::AudioReceiveStream*>(RTC_FROM_HERE, [this, ssrc]() {
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
				workerThread->Invoke<void>(RTC_FROM_HERE, [this, it]() {
					this->g_call->DestroyAudioReceiveStream(it->second);
				});
				audioReceiveStreams.erase(it);
			}
		}
	}

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

	void WebrtcManager::SetupCall()
	{
		this->CreateVoe();
		this->CreateCall();
		this->g_audioSendTransport = new ::Webrtc::StreamTransport(this->g_call, this->outputStream);
		this->createAudioSendStream(this->ssrc, 120);
	}

	WebrtcManager::WebrtcManager()
	{
		udpSocket = Windows::Networking::Sockets::DatagramSocket();
		udpSocket.MessageReceived({ this, &WebrtcManager::OnMessageReceived });
	}

	void WebrtcManager::Create()
	{
		workerThread = rtc::Thread::Create();
		workerThread->Start();

		workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
			this->SetupCall();
		});

	}

	Windows::Foundation::IAsyncAction WebrtcManager::ConnectAsync(hstring ip, hstring port, UINT32 ssrc)
	{
		this->ssrc = ssrc;
		co_await this->udpSocket.ConnectAsync(Windows::Networking::HostName(ip), port);

		this->outputStream = Windows::Storage::Streams::DataWriter(this->udpSocket.OutputStream());
		this->Create();
		this->SendSelectProtocol(ssrc);
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

	bool hasGotIp = false;

	void WebrtcManager::OnMessageReceived(Windows::Networking::Sockets::DatagramSocket const& sender, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs const& args)
	{
		Windows::Storage::Streams::DataReader dr = args.GetDataReader();
		unsigned int dataLength = dr.UnconsumedBufferLength();
		if (hasGotIp)
		{
			std::vector<BYTE> bytes = std::vector<BYTE>(dataLength);
			dr.ReadBytes(bytes);

			if (webrtc::RtpHeaderParser::IsRtcp(bytes.data(), dataLength))
			{
				rtc::CopyOnWriteBuffer buffer = rtc::CopyOnWriteBuffer(bytes.data(), dataLength);

				workerThread->Invoke<void>(RTC_FROM_HERE, [this, buffer, dataLength]() {
					rtc::PacketTime pTime = rtc::CreatePacketTime(0);
					webrtc::PacketReceiver::DeliveryStatus status = this->g_call->Receiver()->DeliverPacket(webrtc::MediaType::ANY, buffer, pTime.timestamp);
				});
			}
			else
			{
				BYTE nonce[24] = { 0 };
				BYTE* decrypted = new BYTE[dataLength - 16];

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
			hasGotIp = true;
			dr.ReadInt32();
			std::array<BYTE, 64> bytes;
			dr.ReadBytes(bytes);

			wchar_t wcstring[65] = { 0 };
			size_t length = 0;

			std::wstring ip(std::begin(bytes), std::end(bytes));
			USHORT port = dr.ReadUInt16();
			this->m_ipAndPortObtained(hstring(ip), port);
		}
	}

	void WebrtcManager::SetKey(array_view<const BYTE> key)
	{
		memcpy(this->key, key.begin(), 32);
	}
}
