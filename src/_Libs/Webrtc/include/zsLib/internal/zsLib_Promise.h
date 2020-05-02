/*

 Copyright (c) 2015, Robin Raymond
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

#ifndef ZSLIB_INTERNAL_PROMISE_H_01c12a0f71cf48877359c5f3853b75f0da59f66a
#define ZSLIB_INTERNAL_PROMISE_H_01c12a0f71cf48877359c5f3853b75f0da59f66a

#include <zsLib/types.h>
#include <zsLib/Proxy.h>
#include <zsLib/MessageQueueAssociator.h>

namespace zsLib
{
  namespace internal
  {
    class Promise : public zsLib::MessageQueueAssociator,
                    public zsLib::IPromiseDelegate
    {
    protected:
      struct make_private {};

    public:
      friend class zsLib::Promise;

      enum PromiseStates
      {
        PromiseState_Pending,
        PromiseState_Resolved,
        PromiseState_Rejected,
      };
      static const char *toString(PromiseStates state) noexcept;

    public:
      Promise(IMessageQueuePtr queue) noexcept;
      Promise(
              const std::list<PromisePtr> &promises,
              IMessageQueuePtr queue
              ) noexcept;

      ~Promise() noexcept;

    protected:
      PromiseWeakPtr mThisWeak;
      AutoPUID mID;

      mutable RecursiveLock mLock;

      PromisePtr mThisBackground;

      PromiseStates mState {PromiseState_Pending};

      IPromiseDelegatePtr mThen;
      IPromiseDelegateWeakPtr mThenWeak;

      AnyPtr mValue;
      AnyPtr mReason;
      AnyPtr mUserData;
      AnyPtr mReferenceHolder;

      bool mFired {false};
      std::list<PromisePtr> mPromises;
    };
  }
}

#endif //ZSLIB_INTERNAL_PROMISE_H_01c12a0f71cf48877359c5f3853b75f0da59f66a
