#pragma once
#include <modules/audio_processing/include/audio_processing.h>

#include "WebrtcManager.h"

namespace Webrtc
{
	class AudioSourceAnalyzer : public webrtc::CustomAudioAnalyzer {
	private:
		winrt::Webrtc::implementation::WebrtcManager* manager;
	public:
		AudioSourceAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager);
		void Initialize(int sample_rate_hz, int num_channels) override;
		void Analyze(const webrtc::AudioBuffer* audio) override;
		[[nodiscard]] std::string ToString() const override;
	};
}
