/*

 Copyright (c) 2016, Robin Raymond
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

#include <zsLib/IMessageQueueManager.h>
#include <zsLib/IWakeDelegate.h>
#include <zsLib/Singleton.h>

#include <zsLib/Log.h>

#define ZSLIB_SETTING_MESSAGE_QUEUE_MANAGER_PROCESS_APPLICATION_MESSAGE_QUEUE_ON_QUIT "zsLib/message-queue-manager/process-application-message-queue-on-quit"

namespace zsLib
{
  namespace internal
  {
    //-----------------------------------------------------------------------
    //-----------------------------------------------------------------------
    //-----------------------------------------------------------------------
    //-----------------------------------------------------------------------
    //
    // MessageQueueManager
    //

    class MessageQueueManager : public IMessageQueueManager,
                                public IWakeDelegate,
                                public ISingletonManagerDelegate
    {
    protected:
      struct make_private {};
      typedef zsLib::Log::Params Params;

      ZS_DECLARE_TYPEDEF_PTR(zsLib::XML::Element, Element);

    public:
      friend interaction IMessageQueueManager;
      friend interaction IMessageQueueManagerForBackgrounding;

      typedef std::map<MessageQueueName, ThreadPriorities> ThreadPriorityMap;
      typedef std::pair<IMessageQueueThreadPoolPtr, size_t> MessageQueueThreadPoolPair;
      typedef std::map<MessageQueueName, MessageQueueThreadPoolPair> MessageQueuePoolMap;

    public:
      MessageQueueManager(const make_private &) noexcept;

    protected:
      void init() noexcept;

      static MessageQueueManagerPtr create() noexcept;

    protected:
      static MessageQueueManagerPtr singleton() noexcept;

    public:
      ~MessageQueueManager() noexcept;

    protected:
      //---------------------------------------------------------------------
      //
      // MessageQueueManager => IMessageQueueManager
      //

      IMessageQueuePtr getMessageQueueForGUIThread() noexcept;
      IMessageQueuePtr getMessageQueue(const char *assignedQueueName) noexcept;

      IMessageQueuePtr getThreadPoolQueue(
                                          const char *assignedThreadPoolQueueName,
                                          size_t minThreadsRequired = 4
                                          ) noexcept;

      void registerMessageQueueThreadPriority(
                                              const char *assignedQueueName,
                                              ThreadPriorities priority
                                              ) noexcept;

      MessageQueueMapPtr getRegisteredQueues() noexcept;

      size_t getTotalUnprocessedMessages() const noexcept;

      void shutdownAllQueues() noexcept;

      virtual void blockUntilDone() noexcept;

      //---------------------------------------------------------------------
      //
      // MessageQueueManager => IWakeDelegate
      //

      void onWake() override;

      //---------------------------------------------------------------------
      //
      // MessageQueueManager => ISingletonManagerDelegate
      //

      void notifySingletonCleanup() noexcept override;

    protected:
      //---------------------------------------------------------------------
      //
      // MessageQueueManager => (internal)
      //

      Params log(const char *message) const noexcept;
      static Params slog(const char *message) noexcept;
      Params debug(const char *message) const noexcept;

      virtual ElementPtr toDebug() const noexcept;

      void cancel() noexcept;

    protected:
      //---------------------------------------------------------------------
      //
      // MessageQueueManager => (data)
      //

      AutoPUID mID;
      mutable RecursiveLock mLock;
      MessageQueueManagerWeakPtr mThisWeak;

      MessageQueueManagerPtr mGracefulShutdownReference;
      bool mFinalCheck {};
      std::atomic<bool> mFinalCheckComplete {};
      size_t mPending;

      MessageQueueMap mQueues;
      ThreadPriorityMap mThreadPriorities;

      MessageQueuePoolMap mPools;

      bool mProcessApplicationQueueOnShutdown {};
    };

  } // namespace internal
} // namespace zsLib
