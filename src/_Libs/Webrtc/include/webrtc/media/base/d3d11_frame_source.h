/*
*  Copyright (c) 2019 The WebRTC project authors. All Rights Reserved.
*
*  Use of this source code is governed by a BSD-style license
*  that can be found in the LICENSE file in the root of the source
*  tree. An additional intellectual property rights grant can be found
*  in the file PATENTS.  All contributing project authors may
*  be found in the AUTHORS file in the root of the source tree.
*/

#ifndef WEBRTC_D3D11VIDEOFRAMESOURCE
#define WEBRTC_D3D11VIDEOFRAMESOURCE

#include "media/base/adaptedvideotracksource.h"
#include "rtc_base/asyncinvoker.h"

#include <winrt/base.h>
#include <d3d11.h>

namespace webrtc {
    class D3D11VideoFrameSource : public rtc::AdaptedVideoTrackSource {
        //Threading in this lib is all over the place, and engines have their own threading considerations
        //so let's not forget this. We might need a thread checker like android or other impls.
        public:
        static rtc::scoped_refptr<webrtc::D3D11VideoFrameSource> 
            Create(ID3D11Device* device, ID3D11DeviceContext* context, D3D11_TEXTURE2D_DESC* desc, rtc::Thread* signaling_thread);
        
        void OnFrameCaptured(ID3D11Texture2D* rendered_image);

        absl::optional<bool> needs_denoising() const override;

        bool is_screencast() const override;

        rtc::AdaptedVideoTrackSource::SourceState state() const override;

        bool remote() const override;

        void SetState(rtc::AdaptedVideoTrackSource::SourceState state);

        protected:
        D3D11VideoFrameSource(ID3D11Device* device, ID3D11DeviceContext* context, D3D11_TEXTURE2D_DESC* desc, rtc::Thread* signaling_thread);

        private:
        winrt::com_ptr<ID3D11Texture2D> staging_texture_;
        winrt::com_ptr<ID3D11Device> device_;
        winrt::com_ptr<ID3D11DeviceContext> context_;
        int width_;
        int height_;

        uint8_t* dst_y_;
        uint8_t* dst_u_;
        uint8_t* dst_v_;

        rtc::Thread* signaling_thread_;
        rtc::AdaptedVideoTrackSource::SourceState state_ = rtc::AdaptedVideoTrackSource::SourceState::kInitializing;
        rtc::AsyncInvoker invoker_;
        const bool is_screencast_;
    };
}

#endif // WEBRTC_D3D11VIDEOFRAMESOURCE