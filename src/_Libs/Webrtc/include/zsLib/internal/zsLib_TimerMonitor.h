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

#include <condition_variable>

#include <zsLib/types.h>
#include <zsLib/Log.h>
#include <zsLib/IMessageQueueThread.h>
#include <zsLib/Singleton.h>

#include <map>
#include <list>

#ifdef __QNX__
#include <pthread.h>
#endif //__QNX__


#define ZSLIB_SETTING_TIMER_MONITOR_THREAD_PRIORITY  "zsLib/timer-monitor/thread-priority"

namespace zsLib
{
  namespace internal
  {
    ZS_DECLARE_CLASS_PTR(TimerMonitor)

    class TimerMonitor : public ISingletonManagerDelegate
    {
    public:
      ZS_DECLARE_TYPEDEF_PTR(zsLib::XML::Element, Element)
      ZS_DECLARE_TYPEDEF_PTR(zsLib::XML::Text, Text)

    protected:
      TimerMonitor() noexcept;
      TimerMonitor(const TimerMonitor &) noexcept = delete;

      void init() noexcept;

    public:
      ~TimerMonitor() noexcept;

      static TimerMonitorPtr singleton() noexcept;
      static TimerMonitorPtr create() noexcept;

      void monitorBegin(TimerPtr timer) noexcept;
      void monitorEnd(Timer &timer) noexcept;

      void operator()() noexcept;

      //-----------------------------------------------------------------------
      //
      // TimerMonitor => ISingletonManagerDelegate
      //

      void notifySingletonCleanup() noexcept override;

    private:
      zsLib::Log::Params log(const char *message) const noexcept;
      static zsLib::Log::Params slog(const char *message) noexcept;

      void cancel() noexcept;

      Microseconds fireTimers() noexcept;
      void wakeUp() noexcept;

    private:
      AutoPUID mID;

      RecursiveLock mLock;
      Lock mFlagLock;
      std::condition_variable mFlagNotify;

      TimerMonitorWeakPtr mThisWeak;
      TimerMonitorPtr mGracefulShutdownReference;

      ThreadPtr mThread;
      bool mShouldShutdown;

      typedef std::map<PUID, TimerWeakPtr> TimerMap;

      TimerMap mMonitoredTimers;

#ifdef __QNX__
      pthread_cond_t      mCondition;
      pthread_mutex_t     mMutex;
#endif //__QNX__
    };
  }
}
