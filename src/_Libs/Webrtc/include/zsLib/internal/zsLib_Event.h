/*

 Copyright (c) 2014, Robin Raymond
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice, this
 list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright notice,
 this list of conditions and the following disclaimer in the documentation
 and/or other materials provided with the distribution.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 The views and conclusions contained in the software and documentation are those
 of the authors and should not be interpreted as representing official policies,
 either expressed or implied, of the FreeBSD Project.
 
 */

#ifndef ZSLIB_INTERNAL_EVENT_H_2070e4cfb8f24209647d3c9ec55098ee
#define ZSLIB_INTERNAL_EVENT_H_2070e4cfb8f24209647d3c9ec55098ee

#include <zsLib/types.h>
#include <condition_variable>

#ifdef _WIN32
#define ZSLIB_INTERNAL_USE_WIN32_EVENT
#endif //_WIN32

namespace zsLib
{
  namespace internal
  {
    class Event
    {
    public:
      Event(bool manualReset = true) noexcept;
      ~Event() noexcept;

      Event(const Event &) = delete;

    protected:
#ifdef ZSLIB_INTERNAL_USE_WIN32_EVENT
      HANDLE mEvent {};
#else
      bool mManualReset {};
      std::atomic_bool mNotified {};
      std::mutex mMutex;
      std::condition_variable mCondition;
#endif
    };
  }
}

#endif //ZSLIB_INTERNAL_EVENT_H_2070e4cfb8f24209647d3c9ec55098ee
