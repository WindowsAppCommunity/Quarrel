/*
*  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
*
*  Use of this source code is governed by a BSD-style license
*  that can be found in the LICENSE file in the root of the source
*  tree. An additional intellectual property rights grant can be found
*  in the file PATENTS.  All contributing project authors may
*  be found in the AUTHORS file in the root of the source tree.
*/


/* This header file includes the inline functions for ARM processors in
* the fix point signal processing library.
*/

#ifndef WEBRTC_COMMON_AUDIO_SIGNAL_PROCESSING_INCLUDE_SPL_INL_NEON_H_
#define WEBRTC_COMMON_AUDIO_SIGNAL_PROCESSING_INCLUDE_SPL_INL_NEON_H_

#include <armintr.h>

#define WEBRTC_SPL_MUL_16_32_RSFT16(a, b) ((int32_t)_arm_smulwb((b), (a)))
#define WEBRTC_SPL_MUL_16_16(a, b) ((int32_t)_arm_smulbb((b), (a)))
#define WebRtc_MulAccumW16(a, b, c) ((int32_t)_arm_smlabb((a), (b), (c)))
#define WebRtcSpl_AddSatW16(a, b) ((int16_t)_arm_qadd16((a), (b)))
#define WebRtcSpl_AddSatW32(a, b) ((int32_t)_arm_qadd((a), (b)))
#define WebRtcSpl_SubSatW32(a, b) ((int32_t)_arm_qsub((a), (b)))
#define WebRtcSpl_SubSatW16(a, b) ((int16_t)_arm_qsub16((a), (b)))
#define WebRtcSpl_GetSizeInBits(a) ((int16_t)(32 - _arm_clz((a))))
#define WebRtcSpl_NormW32(a) ((int16_t)((a) == 0 ? 0 : ((a) < 0 ? _arm_clz((((a) ^ 0xFFFFFFFF))) - 1 : _arm_clz((uint32_t)(a)) - 1)))
#define WebRtcSpl_NormU32(a) ((int16_t)((a) == 0 ? 0 : _arm_clz((uint32_t)(a))))
#define WebRtcSpl_NormW16(a) ((int16_t)((a) == 0 ? 0 : ((a) < 0 ? _arm_clz(((int32_t)(a)) ^ 0xFFFFFFFF) - 17 : _arm_clz((uint32_t)(a)) - 17)))
#define WebRtcSpl_SatW32ToW16(a) ((int16_t)_arm_ssat(16, (a), _ARM_LSL, 0))

#endif  // WEBRTC_COMMON_AUDIO_SIGNAL_PROCESSING_INCLUDE_SPL_INL_NEON_H_
