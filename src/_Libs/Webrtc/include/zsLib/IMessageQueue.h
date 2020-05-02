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
#include <zsLib/Exception.h>

namespace zsLib
{
  interaction IMessageQueueMessage
  {
    virtual const char *getDelegateName() const noexcept = 0;
    virtual const char *getMethodName() const noexcept = 0;

    virtual void processMessage() noexcept = 0;

    virtual ~IMessageQueueMessage() noexcept {}
  };
  
  template <class Closure>
  interaction IMessageQueueMessageClosure : public IMessageQueueMessage
  {
    explicit IMessageQueueMessageClosure(const Closure &closure) noexcept : mClosure(closure) {}

    virtual const char *getDelegateName() const noexcept {return __func__;}
    virtual const char *getMethodName() const noexcept {return __func__;}
    virtual void processMessage() noexcept {mClosure();}

    Closure mClosure;
  };

  interaction IMessageQueueNotify
  {
    virtual void notifyMessagePosted() noexcept = 0;

    virtual bool isCurrentThread() const noexcept = 0;
  };

  interaction IMessageQueue
  {
    struct Exceptions
    {
      ZS_DECLARE_CUSTOM_EXCEPTION(MessageQueueGone)
    };

    typedef size_t size_type;

    static IMessageQueuePtr create(IMessageQueueNotifyPtr notify) noexcept;

    virtual void post(IMessageQueueMessageUniPtr message) noexcept(false) = 0;

    template <class Closure>
    void postClosure(const Closure &closure) noexcept(false) {post(IMessageQueueMessageUniPtr(new IMessageQueueMessageClosure<Closure>(closure)));}

    virtual size_type getTotalUnprocessedMessages() const noexcept = 0;

    virtual bool isCurrentThread() const noexcept = 0;
  };
}
