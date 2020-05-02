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

#include <zsLib/types.h>
#include <zsLib/IMessageQueue.h>

namespace zsLib
{

  enum ThreadPriorities
  {
    ThreadPriority_Idle,
    ThreadPriority_Lowest,
    ThreadPriority_Low,
    ThreadPriority_Normal,
    ThreadPriority_High,
    ThreadPriority_Highest,
    ThreadPriority_Realtime
  };

  const char *toString(ThreadPriorities priority) noexcept;
  ThreadPriorities threadPriorityFromString(const char *str) noexcept;

  void setThreadPriority(
                         Thread &thread,
                         ThreadPriorities threadPriority
                         ) noexcept;

  interaction IMessageQueueThread : public IMessageQueue
  {
    static IMessageQueueThreadPtr createBasic(const char *threadName = NULL, ThreadPriorities threadPriority = ThreadPriority_Normal) noexcept;
    static IMessageQueueThreadPtr singletonUsingCurrentGUIThreadsMessageQueue() noexcept;

    virtual void waitForShutdown() noexcept = 0;

    virtual void setThreadPriority(ThreadPriorities priority) noexcept = 0;

    virtual void processMessagesFromThread() noexcept = 0;
  };

#ifdef __QNX__

  ZS_DECLARE_INTERACTION_PTR(IQtCrossThreadNotifierDelegate)

  interaction IQtCrossThreadNotifierDelegate
  {
    virtual void processMessageFromThread() = 0;
  };

  ZS_DECLARE_INTERACTION_PTR(IQtCrossThreadNotifier)

  interaction IQtCrossThreadNotifier
  {
    static IQtCrossThreadNotifierPtr createNotifier();

    virtual void setDelegate(IQtCrossThreadNotifierDelegatePtr delegate) = 0;
    virtual void notifyMessagePosted() = 0;
  };

#endif // __QNX__
} // namespace zsLib
