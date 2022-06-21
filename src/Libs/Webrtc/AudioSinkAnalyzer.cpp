#include "pch.h"
#include "AudioSinkAnalyzer.h"

#include <modules/audio_processing/include/audio_processing.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "WebrtcManager.h"

namespace Webrtc
{
	AudioSinkAnalyzer::AudioSinkAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager)
	{
		this->manager = manager;
	}
	void AudioSinkAnalyzer::Initialize(int sample_rate_hz, int num_channels) {}
	void AudioSinkAnalyzer::Process(webrtc::AudioBuffer* audio) {
		std::vector<float> data(audio->num_frames());

		webrtc::DownmixToMono<float, float>(audio->channels_const_f(), audio->num_frames(), audio->num_channels(), data.data());
		manager->UpdateOutBytes(winrt::single_threaded_vector<float>(std::move(data)));
	};
	void AudioSinkAnalyzer::SetRuntimeSetting(webrtc::AudioProcessing::RuntimeSetting setting) {}
	std::string AudioSinkAnalyzer::ToString() const { return "OutputAnalyzer"; }
}
