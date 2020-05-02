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

#include <zsLib/Proxy.h>

namespace zsLib
{
  ZS_DECLARE_INTERACTION_PROXY(IPromiseDelegate)

  interaction IPromiseDelegate
  {
    virtual void onPromiseSettled(PromisePtr promise) = 0;
    virtual void onPromiseResolved(PromisePtr promise) = 0;
    virtual void onPromiseRejected(PromisePtr promise) = 0;
  };

  template <class Closure>
  interaction IPromiseClosureDelegate : public IPromiseDelegate
  {
    explicit IPromiseClosureDelegate(const Closure &closure) noexcept : mClosure(closure) {}

    virtual void onPromiseSettled(PromisePtr promise) { mClosure(); }
    virtual void onPromiseResolved(PromisePtr promise) {}
    virtual void onPromiseRejected(PromisePtr promise) {}

    Closure mClosure;
  };

  template <class ClosureResolve, class ClosureReject>
  interaction IPromiseClosureResolveAndRejectDelegate : public IPromiseDelegate
  {
    explicit IPromiseClosureResolveAndRejectDelegate(
                                                     const ClosureResolve &closureResolve,
                                                     const ClosureResolve &closureReject
                                                     ) noexcept :
      mClosureResolve(closureResolve),
      mClosureReject(closureReject)
    {}

    virtual void onPromiseSettled(PromisePtr promise) {}
    virtual void onPromiseResolved(PromisePtr promise) { mClosureResolve(); }
    virtual void onPromiseRejected(PromisePtr promise) { mClosureReject(); }

    ClosureResolve mClosureResolve;
    ClosureReject mClosureReject;
  };
}

#include <zsLib/internal/zsLib_Promise.h>

namespace zsLib
{
  class Promise : public internal::Promise
  {
  public:
    typedef std::list<PromisePtr> PromiseList;
    typedef std::list<PromiseWeakPtr> PromiseWeakList;

  public:
    Promise(
            const make_private &,
            IMessageQueuePtr queue
            ) noexcept;
    Promise(
            const make_private &,
            const std::list<PromisePtr> &promises,
            IMessageQueuePtr queue
            ) noexcept;

  public:
    ~Promise() noexcept;

    static PromisePtr create(IMessageQueuePtr queue = IMessageQueuePtr()) noexcept;
    static PromisePtr all(
                          const PromiseList &promises,
                          IMessageQueuePtr queue = IMessageQueuePtr()
                          ) noexcept;
    static PromisePtr allSettled(
                                 const PromiseList &promises,
                                 IMessageQueuePtr queue = IMessageQueuePtr()
                                 ) noexcept;
    static PromisePtr race(
                           const PromiseList &promises,
                           IMessageQueuePtr queue = IMessageQueuePtr()
                           ) noexcept;
    static PromisePtr broadcast(
                                const PromiseList &promises,
                                IMessageQueuePtr queue = IMessageQueuePtr()
                                ) noexcept;
    static PromisePtr broadcast(
                                const PromiseWeakList &promises,
                                IMessageQueuePtr queue = IMessageQueuePtr()
                                ) noexcept;

    static PromisePtr createResolved(IMessageQueuePtr queue) noexcept {return Promise::createResolved(AnyPtr(), queue);}
    static PromisePtr createResolved(
                                     AnyPtr value = AnyPtr(),
                                     IMessageQueuePtr queue = IMessageQueuePtr()
                                     ) noexcept;
    static PromisePtr createRejected(IMessageQueuePtr queue) noexcept {return Promise::createRejected(AnyPtr(), queue);}
    static PromisePtr createRejected(
                                     AnyPtr reason = AnyPtr(),
                                     IMessageQueuePtr queue = IMessageQueuePtr()
                                     ) noexcept;

    PUID getID() const noexcept {return mID;}

    void resolve(AnyPtr value = AnyPtr()) noexcept;
    void reject(AnyPtr reason = AnyPtr()) noexcept;

    static void resolveAll(
                           const PromiseList &promises,
                           AnyPtr value = AnyPtr()
                           ) noexcept;
    static void resolveAll(
                           const PromiseWeakList &promises,
                           AnyPtr value = AnyPtr()
                           ) noexcept;
    static void rejectAll(
                          const PromiseList &promises,
                          AnyPtr reason = AnyPtr()
                          ) noexcept;
    static void rejectAll(
                          const PromiseWeakList &promises,
                          AnyPtr reason = AnyPtr()
                          ) noexcept;

    void then(IPromiseDelegatePtr delegate) noexcept;
    void thenWeak(IPromiseDelegatePtr delegate) noexcept;
    template <class Closure>
    void thenClosure(const Closure &closure) noexcept             { then(std::make_shared< IPromiseClosureDelegate<Closure> >(closure)); }
    template <class ClosureResolve, class ClosureReject>
    void thenClosures(
                      const ClosureResolve &closureResolve,
                      const ClosureReject &closureReject
                      ) noexcept                                  { then(std::make_shared< IPromiseClosureResolveAndRejectDelegate<ClosureResolve, ClosureReject> >(closureResolve, closureReject)); }

    bool isSettled() const noexcept;
    bool isResolved() const noexcept;
    bool isRejected() const noexcept;

    void background() noexcept;

    const std::list<PromisePtr> &promises() const noexcept        {return mPromises;}

    template <typename data_type>
    std::shared_ptr<data_type> value() const noexcept             {return ZS_DYNAMIC_PTR_CAST(data_type, mValue);}

    template <typename data_type>
    std::shared_ptr<data_type> reason() const noexcept            {return ZS_DYNAMIC_PTR_CAST(data_type, mReason);}

    template <typename data_type>
    std::shared_ptr<data_type> userData() const noexcept          {return ZS_DYNAMIC_PTR_CAST(data_type, mUserData);}

    void setUserData(AnyPtr userData) noexcept                    {mUserData = userData;}

    template <typename data_type>
    std::shared_ptr<data_type> referenceHolder() const noexcept   {return ZS_DYNAMIC_PTR_CAST(data_type, mReferenceHolder);}

    void setReferenceHolder(AnyPtr referenceHolder) noexcept      {mReferenceHolder = referenceHolder;}

  protected:
    void onPromiseSettled(PromisePtr promise) override;
    void onPromiseResolved(PromisePtr promise) override;
    void onPromiseRejected(PromisePtr promise) override;
  };

  template <typename DataType, typename ReasonType = zsLib::Any, typename UserType = zsLib::Any>
  class PromiseWith : public Promise
  {
  public:
    typedef DataType UseDataType;
    typedef std::shared_ptr<UseDataType> UseDataTypePtr;
    typedef std::weak_ptr<UseDataType> UseDataTypeWeakPtr;

    typedef ReasonType UseReasonType;
    typedef std::shared_ptr<UseReasonType> UseReasonTypePtr;
    typedef std::weak_ptr<UseReasonType> UseReasonTypeWeakPtr;

    typedef UserType UseUserType;
    typedef std::shared_ptr<UseUserType> UseUserTypePtr;
    typedef std::weak_ptr<UseUserType> UseUserTypeWeakPtr;

    typedef PromiseWith<DataType, ReasonType, UserType> PromiseWithType;
    typedef std::shared_ptr<PromiseWithType> PromiseWithTypePtr;
    typedef std::weak_ptr<PromiseWithType> PromiseWithTypeWeakPtr;

  public:
    PromiseWith(
                const make_private &,
                IMessageQueuePtr queue = IMessageQueuePtr()
                ) noexcept : Promise(make_private {}, queue) {}

  public:
    static PromiseWithTypePtr create(IMessageQueuePtr queue = IMessageQueuePtr()) noexcept {
      PromiseWithTypePtr pThis(make_shared<PromiseWith>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      return pThis;
    }

    static PromiseWithTypePtr createFrom(PromisePtr genericPromise) noexcept {
      if (!genericPromise) return PromiseWithTypePtr();

      PromiseList promises;
      promises.push_back(genericPromise);
      IMessageQueuePtr queue = genericPromise->getAssociatedMessageQueue();
      PromiseWithTypePtr pThis(make_shared<PromiseWithType>(make_private{}, promises, queue));
      pThis->mThisWeak = pThis;
      genericPromise->thenWeak(pThis);
      return pThis;
    }

    static PromiseWithTypePtr createResolved(IMessageQueuePtr queue) noexcept {return PromiseWithType::createResolved(UseDataTypePtr(), queue);}
    static PromiseWithTypePtr createResolved(
                                             UseDataTypePtr value = UseDataTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(make_shared<PromiseWith>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->resolve(value);
      return pThis;
    }
    static PromiseWithTypePtr createRejected(IMessageQueuePtr queue) noexcept {return PromiseWithType::createRejected(UseReasonTypePtr(), queue);}
    static PromiseWithTypePtr createRejected(
                                             UseReasonTypePtr reason = UseReasonTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(make_shared<PromiseWith>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->reject(reason);
      return pThis;
    }

    UseDataTypePtr value() const noexcept {return Promise::value<DataType>();}
    UseReasonTypePtr reason() const noexcept {return Promise::reason<UseReasonType>();}
    UseUserTypePtr userData() const noexcept {return Promise::userData<UseUserType>();}
  };

  template <typename DataType, typename ReasonType = zsLib::Any, typename UserType = zsLib::Any>
  class PromiseWithHolder : public Promise
  {
  public:
    typedef DataType UseDataType;
    typedef std::shared_ptr<UseDataType> UseDataTypePtr;
    typedef std::weak_ptr<UseDataType> UseDataTypeWeakPtr;

    typedef AnyHolder<UseDataTypePtr> AnyHolderUseDataType;
    typedef std::shared_ptr<AnyHolderUseDataType> AnyHolderUseDataTypePtr;
    typedef std::weak_ptr<AnyHolderUseDataType> AnyHolderUseDataTypeWeakPtr;

    typedef ReasonType UseReasonType;
    typedef std::shared_ptr<UseReasonType> UseReasonTypePtr;
    typedef std::weak_ptr<UseReasonType> UseReasonTypeWeakPtr;

    typedef AnyHolder<UseReasonTypePtr> AnyHolderUseReasonType;
    typedef std::shared_ptr<AnyHolderUseReasonType> AnyHolderUseReasonTypePtr;
    typedef std::weak_ptr<AnyHolderUseReasonType> AnyHolderUseReasonTypeWeakPtr;

    typedef UserType UseUserType;
    typedef std::shared_ptr<UseUserType> UseUserTypePtr;
    typedef std::weak_ptr<UseUserType> UseUserTypeWeakPtr;

    typedef AnyHolder<UseUserTypePtr> AnyHolderUseUserType;
    typedef std::shared_ptr<AnyHolderUseUserType> AnyHolderUseUserTypePtr;
    typedef std::weak_ptr<AnyHolderUseUserType> AnyHolderUseUserTypeWeakPtr;

    typedef PromiseWithHolder<DataType, ReasonType, UserType> PromiseWithType;
    typedef std::shared_ptr<PromiseWithType> PromiseWithTypePtr;
    typedef std::weak_ptr<PromiseWithType> PromiseWithTypeWeakPtr;

  public:
    PromiseWithHolder(
                      const make_private &,
                      IMessageQueuePtr queue = IMessageQueuePtr()
                      ) noexcept : Promise(make_private {}, queue) {}

  public:
    static PromiseWithTypePtr create(IMessageQueuePtr queue = IMessageQueuePtr()) {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      return pThis;
    }

    static PromiseWithTypePtr createFrom(PromisePtr genericPromise) noexcept {
      if (!genericPromise) return PromiseWithTypePtr();

      PromiseList promises;
      promises.push_back(genericPromise);
      IMessageQueuePtr queue = genericPromise->getAssociatedMessageQueue();
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, promises, queue));
      pThis->mThisWeak = pThis;
      genericPromise->thenWeak(pThis);
      return pThis;
    }

    static PromiseWithTypePtr createResolved(IMessageQueuePtr queue) noexcept {return PromiseWithType::createResolved(UseDataTypePtr(), queue);}
    static PromiseWithTypePtr createResolved(
                                             UseDataTypePtr value = UseDataTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->resolve(value);
      return pThis;
    }
    static PromiseWithTypePtr createRejected(IMessageQueuePtr queue) noexcept {return PromiseWithType::createRejected(UseReasonTypePtr(), queue);}
    static PromiseWithTypePtr createRejected(
                                             UseReasonTypePtr reason = UseReasonTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->reject(reason);
      return pThis;
    }

    void resolve(UseDataTypePtr value = UseDataTypePtr()) noexcept { auto result = std::make_shared< AnyHolderUseDataType >(); result->value_ = value; Promise::resolve(result); }
    void reject(UseReasonTypePtr reason = UseReasonTypePtr()) noexcept { auto result = std::make_shared< AnyHolderUseReasonType >(); result->value_ = reason; Promise::reject(result); }
    void setUserData(UseUserTypePtr userData) noexcept { auto value = std::make_shared< AnyHolderUseUserTypePtr>(); value->value_ = userData; Promise::setUserData(value); }

    UseDataTypePtr value() const noexcept { auto result = Promise::value<AnyHolderUseDataType>(); if (result) return result->value_; return UseDataTypePtr(); }
    UseReasonTypePtr reason() const noexcept { auto result = Promise::reason<AnyHolderUseReasonType>(); if (result) return result->value_; return UseReasonTypePtr(); }
    UseUserTypePtr userData() const noexcept { auto result = Promise::userData<AnyHolderUseUserType>(); if (result) return result->value_; return UseUserTypePtr(); }
  };


  template <typename DataTypePtr, typename ReasonTypePtr = zsLib::AnyPtr, typename UserTypePtr = zsLib::AnyPtr>
  class PromiseWithHolderPtr : public Promise
  {
  public:
    typedef DataTypePtr UseDataTypePtr;

    typedef AnyHolder<UseDataTypePtr> AnyHolderUseDataType;
    typedef std::shared_ptr<AnyHolderUseDataType> AnyHolderUseDataTypePtr;
    typedef std::weak_ptr<AnyHolderUseDataType> AnyHolderUseDataTypeWeakPtr;

    typedef ReasonTypePtr UseReasonTypePtr;

    typedef AnyHolder<UseReasonTypePtr> AnyHolderUseReasonType;
    typedef std::shared_ptr<AnyHolderUseReasonType> AnyHolderUseReasonTypePtr;
    typedef std::weak_ptr<AnyHolderUseReasonType> AnyHolderUseReasonTypeWeakPtr;

    typedef UserTypePtr UseUserTypePtr;

    typedef AnyHolder<UseUserTypePtr> AnyHolderUseUserType;
    typedef std::shared_ptr<AnyHolderUseUserType> AnyHolderUseUserTypePtr;
    typedef std::weak_ptr<AnyHolderUseUserType> AnyHolderUseUserTypeWeakPtr;

    typedef PromiseWithHolderPtr<DataTypePtr, ReasonTypePtr, UserTypePtr> PromiseWithType;
    typedef std::shared_ptr<PromiseWithType> PromiseWithTypePtr;
    typedef std::weak_ptr<PromiseWithType> PromiseWithTypeWeakPtr;

  public:
    PromiseWithHolderPtr(
                         const make_private &,
                         IMessageQueuePtr queue = IMessageQueuePtr()
                         ) noexcept : Promise(make_private {}, queue) {}

  public:
    static PromiseWithTypePtr create(IMessageQueuePtr queue = IMessageQueuePtr()) noexcept {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      return pThis;
    }

    static PromiseWithTypePtr createFrom(PromisePtr genericPromise) noexcept {
      if (!genericPromise) return PromiseWithTypePtr();

      PromiseList promises;
      promises.push_back(genericPromise);
      IMessageQueuePtr queue = genericPromise->getAssociatedMessageQueue();
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, promises, queue));
      pThis->mThisWeak = pThis;
      genericPromise->thenWeak(pThis);
      return pThis;
    }

    static PromiseWithTypePtr createResolved(IMessageQueuePtr queue) noexcept {return PromiseWithType::createResolved(UseDataTypePtr(), queue);}
    static PromiseWithTypePtr createResolved(
                                             UseDataTypePtr value = UseDataTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->resolve(value);
      return pThis;
    }
    static PromiseWithTypePtr createRejected(IMessageQueuePtr queue) noexcept {return PromiseWithType::createRejected(UseReasonTypePtr(), queue);}
    static PromiseWithTypePtr createRejected(
                                             UseReasonTypePtr reason = UseReasonTypePtr(),
                                             IMessageQueuePtr queue = IMessageQueuePtr()
                                             ) noexcept {
      PromiseWithTypePtr pThis(std::make_shared<PromiseWithType>(make_private{}, queue));
      pThis->mThisWeak = pThis;
      pThis->reject(reason);
      return pThis;
    }

    void resolve(UseDataTypePtr value = UseDataTypePtr()) noexcept { auto result = std::make_shared< AnyHolderUseDataType >(); result->value_ = value; Promise::resolve(result); }
    void reject(UseReasonTypePtr reason = UseReasonTypePtr()) noexcept { auto result = std::make_shared< AnyHolderUseReasonType >(); result->value_ = reason; Promise::reject(result); }
    void setUserData(UseUserTypePtr userData) noexcept { auto value = std::make_shared< AnyHolderUseUserType >(); value->value_ = userData; Promise::setUserData(value); }

    UseDataTypePtr value() const noexcept { auto result = Promise::value<AnyHolderUseDataType>(); if (result) return result->value_; return UseDataTypePtr(); }
    UseReasonTypePtr reason() const noexcept { auto result = Promise::reason<AnyHolderUseReasonType>(); if (result) return result->value_; return UseReasonTypePtr(); }
    UseUserTypePtr userData() const noexcept { auto result = Promise::userData<AnyHolderUseUserType>(); if (result) return result->value_; return UseUserTypePtr(); }
  };


  interaction IPromiseSettledDelegate : public IPromiseDelegate
  {
    void onPromiseSettled(PromisePtr promise) override = 0;
    void onPromiseResolved(PromisePtr promise) override; // filtered
    void onPromiseRejected(PromisePtr promise) override; // filtered
  };

  interaction IPromiseResolutionDelegate : public IPromiseDelegate
  {
    void onPromiseSettled(PromisePtr promise) override; // filtered
    void onPromiseResolved(PromisePtr promise) override = 0;
    void onPromiseRejected(PromisePtr promise) override = 0;
  };

  interaction IPromiseCatchDelegate : public IPromiseDelegate
  {
    void onPromiseSettled(PromisePtr promise) override;  // filtered
    void onPromiseResolved(PromisePtr promise) override; // filtered
    void onPromiseRejected(PromisePtr promise) override = 0;
  };
}

ZS_DECLARE_PROXY_WITH_DELEGATE_MESSAGE_QUEUE_OPTIONAL_BEGIN(zsLib::IPromiseDelegate)
ZS_DECLARE_PROXY_METHOD(onPromiseSettled, zsLib::PromisePtr)
ZS_DECLARE_PROXY_METHOD(onPromiseResolved, zsLib::PromisePtr)
ZS_DECLARE_PROXY_METHOD(onPromiseRejected, zsLib::PromisePtr)
ZS_DECLARE_PROXY_END()
