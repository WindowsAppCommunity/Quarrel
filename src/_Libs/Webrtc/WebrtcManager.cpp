#include "pch.h"
#include "WebrtcManager.h"

class AudioAnalyzer : public webrtc::CustomAudioAnalyzer {
private:
	webrtc::AudioSendStream* stream;
public:
	void Initialize(int sample_rate_hz, int num_channels) override {}
	void Analyze(const webrtc::AudioBuffer* audio) override {

	};
	std::string ToString() const override { return "AudioAnalyzer"; }
};


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

	StreamTransport::StreamTransport(webrtc::Call* call, Windows::Storage::Streams::DataWriter^ sendStream) : call(call), sendStream(sendStream)
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
		sendStream->WriteBytes(Platform::ArrayReference<uint8_t>((uint8_t*)packet, length));
		sendStream->StoreAsync();
		return true;
	}

	void WebrtcManager::CreateVoe()
	{
		this->g_audioDecoderFactory = webrtc::CreateBuiltinAudioDecoderFactory();
		this->g_audioEncoderFactory = webrtc::CreateBuiltinAudioEncoderFactory();

		rtc::scoped_refptr<webrtc::AudioDeviceModule> audioDeviceModule;
		IAudioDeviceWasapi::CreationProperties props;
		props.id_ = "";
		props.playoutEnabled_ = true;
		props.recordingEnabled_ = true;
		auto adm = rtc::scoped_refptr<webrtc::AudioDeviceModule>(IAudioDeviceWasapi::create(props));

		rtc::scoped_refptr<webrtc::AudioProcessing> apm = webrtc::AudioProcessingBuilder().SetCaptureAnalyzer(std::unique_ptr<AudioAnalyzer>(new AudioAnalyzer())).Create();

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


	void WebrtcManager::SetupCall()
	{
		this->CreateVoe();
		this->CreateCall();
		this->g_audioSendTransport = new StreamTransport(this->g_call, this->outputStream);
	}

	WebrtcManager::WebrtcManager()
	{
		udpSocket = ref new Windows::Networking::Sockets::DatagramSocket();
		udpSocket->MessageReceived += ref new Windows::Foundation::TypedEventHandler<Windows::Networking::Sockets::DatagramSocket^, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs^>(this, &WebrtcManager::OnMessageReceived);
	}

	void WebrtcManager::Create()
	{
		workerThread = rtc::Thread::Create();
		workerThread->Start();

		workerThread->Invoke<void>(RTC_FROM_HERE, [this]() {
			this->SetupCall();
		});

	}

	void WebrtcManager::ConnectAsync(Platform::String^ ip, Platform::String^ port, UINT32 ssrc)
	{
		this->ssrc = ssrc;
		concurrency::create_task(udpSocket->ConnectAsync(ref new Windows::Networking::HostName(ip), port)).then([this, ssrc]()
		{
			outputStream = ref new Windows::Storage::Streams::DataWriter(udpSocket->OutputStream);
			WebrtcManager::Create();
			SendSelectProtocol(ssrc);
		});

	}

	void WebrtcManager::SendSelectProtocol(UINT32 ssrc)
	{
		Platform::Array<unsigned char>^ packet = ref new Platform::Array<unsigned char>(70);
		packet[0] = (BYTE)(ssrc >> 24);
		packet[1] = (BYTE)(ssrc >> 16);
		packet[2] = (BYTE)(ssrc >> 8);
		packet[3] = (BYTE)(ssrc >> 0);
		outputStream->WriteBytes(packet);
		outputStream->StoreAsync();
	}

	bool hasGotIp = false;

	void WebrtcManager::OnMessageReceived(Windows::Networking::Sockets::DatagramSocket^ sender, Windows::Networking::Sockets::DatagramSocketMessageReceivedEventArgs^ args)
	{
		Windows::Storage::Streams::DataReader^ dr = args->GetDataReader();
		unsigned int dataLength = dr->UnconsumedBufferLength;
		if (hasGotIp)
		{
			Platform::Array<BYTE>^ bytes = ref new Platform::Array<BYTE>(dataLength);
			dr->ReadBytes(bytes);
			BYTE* packet = bytes->begin();

			if (webrtc::RtpHeaderParser::IsRtcp(packet, dataLength))
			{
				workerThread->Invoke<void>(RTC_FROM_HERE, [this, packet, dataLength]() {
					rtc::PacketTime pTime = rtc::CreatePacketTime(0);
					webrtc::PacketReceiver::DeliveryStatus status = this->g_call->Receiver()->DeliverPacket(webrtc::MediaType::ANY, rtc::CopyOnWriteBuffer(packet, dataLength), pTime.timestamp);
				});
			}
			else
			{
				BYTE nonce[24] = { 0 };
				BYTE* decrypted = new BYTE[dataLength - 16];

				memcpy(nonce, packet, 12);
				memcpy(decrypted, packet, 12);

				crypto_secretbox_open_easy(decrypted + 12, packet + 12, dataLength - 12, nonce, key);
				workerThread->Invoke<void>(RTC_FROM_HERE, [this, decrypted, dataLength]() {
					rtc::PacketTime pTime = rtc::CreatePacketTime(0);
					webrtc::PacketReceiver::DeliveryStatus status = this->g_call->Receiver()->DeliverPacket(webrtc::MediaType::AUDIO, rtc::CopyOnWriteBuffer(decrypted, dataLength - 16), pTime.timestamp);
				});
			}

		}
		else {
			hasGotIp = true;
			dr->ReadInt32();
			Platform::Array<BYTE>^ bytes = ref new Platform::Array<BYTE>(64);
			dr->ReadBytes(bytes);

			wchar_t wcstring[65] = { 0 };
			size_t length = 0;
			mbstowcs_s(&length, wcstring, 65, (char*)bytes->begin(), _TRUNCATE);

			Platform::String^ ip = ref new Platform::String(wcstring, length - 1);
			USHORT port = dr->ReadUInt16();
			IpAndPortObtained(ip, port);
		}
	}

	void WebrtcManager::SetKey(const Platform::Array<BYTE>^ key)
	{
		memcpy(this->key, key->begin(), 32);
	}
}
