#pragma once
#include <modules/audio_processing/include/audio_processing.h>

#include "WebrtcManager.h"

namespace Webrtc
{
	class AudioSinkAnalyzer : public webrtc::CustomProcessing
	{
	private:
		winrt::Webrtc::implementation::WebrtcManager* manager;
	public:
		AudioSinkAnalyzer(winrt::Webrtc::implementation::WebrtcManager* manager);
		void Initialize(int sample_rate_hz, int num_channels) override;
		void Process(webrtc::AudioBuffer* audio) override;
		void SetRuntimeSetting(webrtc::AudioProcessing::RuntimeSetting setting) override;
		[[nodiscard]] std::string ToString() const override;
	};
}


