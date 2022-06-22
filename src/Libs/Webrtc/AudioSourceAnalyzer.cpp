#include "pch.h"
#include "AudioSourceAnalyzer.h"

#include <winrt/Windows.Foundation.Collections.h>

#include "WebrtcManager.h"

namespace Webrtc
{
	
	AudioSourceAnalyzer::AudioSourceAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager)
	{
		this->manager = manager;
	}
	void AudioSourceAnalyzer::Initialize(int sample_rate_hz, int num_channels) {}
	void AudioSourceAnalyzer::Analyze(const webrtc::AudioBuffer* audio) {
		std::vector<float> data(audio->num_frames());

		webrtc::DownmixToMono<float, float>(audio->channels_const_f(), audio->num_frames(), audio->num_channels(), data.data());

		this->manager->UpdateSpeaking(this->manager->audio_processing->GetStatistics().voice_detected.value_or(true));

		this->manager->UpdateInBytes(winrt::single_threaded_vector<float>(std::move(data)));
	};
	std::string AudioSourceAnalyzer::ToString() const { return "AudioAnalyzer"; }
}
