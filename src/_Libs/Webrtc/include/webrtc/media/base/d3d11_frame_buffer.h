/*
*  Copyright (c) 2019 The WebRTC project authors. All Rights Reserved.
*
*  Use of this source code is governed by a BSD-style license
*  that can be found in the LICENSE file in the root of the source
*  tree. An additional intellectual property rights grant can be found
*  in the file PATENTS.  All contributing project authors may
*  be found in the AUTHORS file in the root of the source tree.
*/

#ifndef WEBRTC_D3D11VIDEOFRAMEBUFFER
#define WEBRTC_D3D11VIDEOFRAMEBUFFER

#include <d3d11.h>
#include <winrt/base.h>

#include "api/video/video_frame_buffer.h"
#include "rtc_base/refcountedobject.h"

namespace webrtc {
    class D3D11VideoFrameBuffer : public webrtc::VideoFrameBuffer {
        public:
        //TODO: look up if refcounting is needed/common for buffers. It is for sources but
        //maybe I mixed it up.
        static rtc::scoped_refptr<D3D11VideoFrameBuffer> Create(ID3D11DeviceContext* context, ID3D11Texture2D* staging_texture, ID3D11Texture2D* rendered_image, int width, int height);
        static rtc::scoped_refptr<D3D11VideoFrameBuffer> Create(ID3D11DeviceContext* context, 
            ID3D11Texture2D* staging_texture, ID3D11Texture2D* rendered_image, int width, int height, uint8_t* dst_y, uint8_t* dst_u, uint8_t* dst_v);
        virtual webrtc::VideoFrameBuffer::Type type() const override;
        int width() const override { return width_; }
        int height() const override { return height_; }
        rtc::scoped_refptr<webrtc::I420BufferInterface> ToI420() override;

        protected:
        D3D11VideoFrameBuffer(ID3D11DeviceContext* context, ID3D11Texture2D* staging_texture, ID3D11Texture2D* rendered_image, int width, int height);
        D3D11VideoFrameBuffer(ID3D11DeviceContext* context, ID3D11Texture2D* staging_texture, ID3D11Texture2D* rendered_image, int width, int height, uint8_t* dst_y, uint8_t* dst_u, uint8_t* dst_v);
        private:
        const int width_;
        const int height_;
        winrt::com_ptr<ID3D11Texture2D> staging_texture_;
        winrt::com_ptr<ID3D11Texture2D> rendered_image_;
        winrt::com_ptr<ID3D11DeviceContext> context_;

        //i guess these don't really need to be members and could be passed as parameters instead
        uint8_t* dst_y_;
        uint8_t* dst_u_;
        uint8_t* dst_v_;
    };
}

#endif // WEBRTC_D3D11VIDEOFRAMEBUFFER