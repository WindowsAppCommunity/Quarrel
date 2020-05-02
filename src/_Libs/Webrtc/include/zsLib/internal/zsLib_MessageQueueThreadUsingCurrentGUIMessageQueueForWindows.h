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

#if defined(_WIN32) && !defined(WINUWP)

#ifndef ZSLIB_INTERNAL_MESSAGEQUEUETHREADUSINGCURRENTGUIMESSAGEQUEUEFORWINDOWS_H_e059928c0dab4631bdaeab09d5b25847
#define ZSLIB_INTERNAL_MESSAGEQUEUETHREADUSINGCURRENTGUIMESSAGEQUEUEFORWINDOWS_H_e059928c0dab4631bdaeab09d5b25847

#include <zsLib/internal/zsLib_MessageQueueThread.h>

#include <zsLib/Exception.h>

namespace zsLib
{
  namespace internal
  {
    ZS_DECLARE_CLASS_PTR(MessageQueueThreadUsingCurrentGUIMessageQueueForWindows)

    class MessageQueueThreadUsingCurrentGUIMessageQueueForWindows : public MessageQueueThread,
                                                                    public IMessageQueueNotify
    {
    protected:
      MessageQueueThreadUsingCurrentGUIMessageQueueForWindows() noexcept;
      static MessageQueueThreadUsingCurrentGUIMessageQueueForWindowsPtr create() noexcept;
      void setup() noexcept;

    public:
      ~MessageQueueThreadUsingCurrentGUIMessageQueueForWindows() noexcept;

      static MessageQueueThreadUsingCurrentGUIMessageQueueForWindowsPtr singleton() noexcept;

      // IMessageQueue
      void post(IMessageQueueMessageUniPtr message) noexcept(false) override;

      size_type getTotalUnprocessedMessages() const noexcept override;

      bool isCurrentThread() const noexcept override;

      // IMessageQueueNotify
      void notifyMessagePosted() noexcept override;

      // (duplicate) virtual bool isCurrentThread() const noexcept;

      // IMessageQueueThread
      void waitForShutdown() noexcept override;

      void setThreadPriority(ThreadPriorities threadPriority) noexcept override;

    public:
      virtual void process() noexcept;
      virtual void processMessagesFromThread() noexcept;

    protected:
      mutable Lock mLock;

      MessageQueuePtr mQueue;
      HWND mHWND;

      std::atomic_bool mIsShutdown {};
    };
  }
}

#endif //ZSLIB_INTERNAL_MESSAGEQUEUETHREADUSINGCURRENTGUIMESSAGEQUEUEFORWINDOWS_H_e059928c0dab4631bdaeab09d5b25847

#endif // defined(_WIN32) && !defined(WINUWP)
