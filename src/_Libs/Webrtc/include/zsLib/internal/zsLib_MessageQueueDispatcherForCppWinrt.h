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

#ifdef __has_include
#if __has_include(<winrt/windows.ui.core.h>)
#include <winrt/windows.ui.core.h>
#endif //__has_include(<winrt/windows.ui.core.h>)
#endif //__has_include

#ifdef CPPWINRT_VERSION

#include <Windows.h>

#include <zsLib/internal/zsLib_MessageQueueDispatcher.h>

#include <zsLib/Exception.h>

namespace zsLib
{
  namespace internal
  {
    ZS_DECLARE_CLASS_PTR(MessageQueueDispatcherForCppWinrt);

    class MessageQueueDispatcherForCppWinrt : public MessageQueueDispatcher,
                                              public IMessageQueueNotify
    {
    public:
      typedef winrt::Windows::UI::Core::CoreDispatcher CoreDispatcher;

      struct Exceptions
      {
        ZS_DECLARE_CUSTOM_EXCEPTION(MessageQueueAlreadyDeleted)
      };

    protected:
      MessageQueueDispatcherForCppWinrt() noexcept;
      static void dispatch(MessageQueueDispatcherForCppWinrtPtr queue) noexcept;

    public:
      ~MessageQueueDispatcherForCppWinrt() noexcept;

      static MessageQueueDispatcherForCppWinrtPtr create(
        CoreDispatcher dispatcher,
        ThreadPriorities threadPriority = ThreadPriority_Normal
        ) noexcept;

      // IMessageQueue
      void post(IMessageQueueMessageUniPtr message) noexcept(false) override;

      size_type getTotalUnprocessedMessages() const noexcept override;

      bool isCurrentThread() const noexcept override;

      // IMessageQueueNotify
      void notifyMessagePosted() noexcept override;

      // (duplicate) virtual bool isCurrentThread() const noexcept = 0;

      // IMessageQueueThread
      void waitForShutdown() noexcept override;

      void setThreadPriority(ThreadPriorities threadPriority) noexcept override;

    public:
      virtual void process() noexcept;
      virtual void processMessagesFromThread() noexcept;

    protected:
      mutable Lock lock_;
      MessageQueueDispatcherForCppWinrtWeakPtr thisWeak_;

      MessageQueuePtr queue_;
      CoreDispatcher dispatcher_ {nullptr};

      std::atomic_bool isShutdown_ {};

      winrt::Windows::UI::Core::CoreDispatcherPriority priority_ {winrt::Windows::UI::Core::CoreDispatcherPriority::Normal};
    };
  }
}

#endif //CPPWINRT_VERSION
