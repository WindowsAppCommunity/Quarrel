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

#include <zsLib/IMessageQueueThread.h>

#include <map>

namespace zsLib
{
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //
  // IMessageQueueManager
  //

  interaction IMessageQueueManager
  {
    typedef String MessageQueueName;
    typedef std::map<MessageQueueName, IMessageQueuePtr> MessageQueueMap;
    ZS_DECLARE_PTR(MessageQueueMap);

    //-------------------------------------------------------------------------
    // PURPOSE: Get the message queue assigned with the GUI
    //
    // WARNING: This method MUST be called first on the GUI thread before
    //          any other thread may use this method otherwise unpredictable
    //          results may occur.
    static IMessageQueuePtr getMessageQueueForGUIThread() noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: Obtains an existing message queue for for registered queues
    //          or creates a new message queue thread if no such queue name
    //          exists.
    static IMessageQueuePtr getMessageQueue(const char *assignedQueueName) noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: Create a message queue for a pool
    static IMessageQueuePtr getThreadPoolQueue(
                                                const char *assignedThreadPoolQueueName,
                                                size_t minThreadsRequired = 4
                                                ) noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: Registers the thread priority to use for a thread that may
    //          be created by way of "getMessageQueue" or thread pools
    //          registered with previous calls to "getThreadPoolQueue"
    //
    // WARNING: This method must be called before the thread is obtained via
    //          the "getMessageQueue" or "getThreadPoolQueue" method or
    //          the thread priority will be ignored.
    static void registerMessageQueueThreadPriority(
                                                   const char *assignedQueueName,
                                                   ThreadPriorities priority
                                                   ) noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: Count the number of unprocessed messages in each queue and
    //          return the summary total
    static size_t getTotalUnprocessedMessages() noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: Shutdown all threads now
    //
    // WARNING: The total unprocessed messages in all the queues should be
    //          "0" to ensure all pending work is completed
    static void shutdownAllQueues() noexcept;

    //-------------------------------------------------------------------------
    // PURPOSE: block current thread until all non-GUI threads are completed.
    static void blockUntilDone() noexcept;
  };

} // namespace zsLib
