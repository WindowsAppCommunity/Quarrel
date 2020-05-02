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

#pragma once

#ifndef ZSLIB_INTERNAL_TIMER_H_1d1227118903c8b55faa0906dd0a99f8
#define ZSLIB_INTERNAL_TIMER_H_1d1227118903c8b55faa0906dd0a99f8

#include <zsLib/ITimer.h>

namespace zsLib
{
  namespace internal
  {
    class Timer : public ITimer
    {
    protected:
      struct make_private {};
      friend interaction ITimer;
      friend class TimerMonitor;

    public:
      Timer(
            const make_private &,
            ITimerDelegatePtr delegate,
            Microseconds timeout,
            bool repeat,
            size_t maxFiringTimerAtOnce
            ) noexcept;

      ~Timer() noexcept;

    public:
      static TimerPtr create(
                             ITimerDelegatePtr delegate,
                             Microseconds timeout,
                             bool repeat,
                             size_t maxFiringTimerAtOnce
                             ) noexcept;

      static TimerPtr create(
                             ITimerDelegatePtr delegate,
                             Time timeout
                             ) noexcept;

      PUID getID() const noexcept override;

      void cancel() noexcept override;      // cancel a timer (it is no longer needed)

      void background(bool background = true) noexcept override;  // background the timer (will run until timer is cancelled even if reference to object is forgotten)

    protected:
      bool tick(const Time &time, Microseconds &sleepTime) noexcept;  // returns true if should expire the timer

    protected:
      RecursiveLock mLock;
      PUID mID;
      TimerWeakPtr mThisWeak;
      TimerPtr mThisBackground;
      ITimerDelegatePtr mDelegate;

      size_t mMaxFiringsAtOnce {};
      Time mFireNextAt {};
      Microseconds mTimeout {};
      bool mOnceOnly {};
      bool mMonitored {};
    };

  }
}

#endif //ZSLIB_INTERNAL_TIMER_H_1d1227118903c8b55faa0906dd0a99f8
