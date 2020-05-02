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

#ifndef ZSLIB_INTERNAL_PROXY_SUBSCRIPTIONS_H_cf7229753b441247ccece9f3b92317ed96045660
#define ZSLIB_INTERNAL_PROXY_SUBSCRIPTIONS_H_cf7229753b441247ccece9f3b92317ed96045660

#include <zsLib/types.h>
#include <zsLib/helpers.h>

#include <map>

namespace zsLib
{
  ZS_DECLARE_FORWARD_SUBSYSTEM(zslib)

  namespace internal
  {
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // ProxySubscriptions
    //

    template <typename XINTERFACE, typename SUBSCRIPTIONBASECLASS>
    class ProxySubscriptions
    {
    public:
      ZS_DECLARE_CLASS_PTR(Subscription)
      ZS_DECLARE_CLASS_PTR(DelegateImpl)

      ZS_DECLARE_TYPEDEF_PTR(SUBSCRIPTIONBASECLASS, SubscriptionBaseClass)

      ZS_DECLARE_TYPEDEF_PROXY(XINTERFACE, Delegate)

      typedef ProxySubscriptions<XINTERFACE, SUBSCRIPTIONBASECLASS> ProxySubscriptionsBaseType;
      typedef std::pair<SubscriptionWeakPtr, DelegatePtr> SubscriptionDelegatePair;

      typedef std::map<Subscription *, SubscriptionDelegatePair> SubscriptionDelegateMap;
      ZS_DECLARE_PTR(SubscriptionDelegateMap)
      typedef typename SubscriptionDelegateMap::size_type size_type;

      typedef bool Bogus;
      typedef std::map<SubscriptionPtr, Bogus> SubscriptionBackgroundMap;

      ZS_DECLARE_TYPEDEF_PTR(SUBSCRIPTIONBASECLASS, BaseSubscription)

    public:
      ProxySubscriptions(DelegateImplPtr delegate) noexcept :
        mDelegateImpl(delegate)
      {
      }

      ProxySubscriptions(const ProxySubscriptionsBaseType &source) noexcept :
        mDelegateImpl(source.mDelegateImpl)
      {
      }

      ~ProxySubscriptions() noexcept {}

      SubscriptionPtr subscribe(
                                DelegatePtr originalDelegate,
                                IMessageQueuePtr queue = IMessageQueuePtr(),
                                bool strongReferenceToDelgate = false
                                ) noexcept
      {
        SubscriptionPtr subscription(make_shared<Subscription>());
        subscription->mThisWeak = subscription;
        subscription->mDelegateImpl = mDelegateImpl;

        if (originalDelegate) {
          DelegatePtr delegate;
          if (strongReferenceToDelgate) {
            delegate = DelegateProxy::create(queue, originalDelegate);
          } else {
            delegate = DelegateProxy::createWeak(queue, originalDelegate);
          }
          mDelegateImpl->subscribe(subscription, delegate);
        }

        return subscription;
      }

      DelegatePtr delegate() const noexcept
      {
        return mDelegateImpl;
      }

      DelegatePtr delegate(
                           BaseSubscriptionPtr subscription,
                           bool strongReferenceToDelgate = false
                           ) const noexcept
      {
        if (!subscription) return DelegatePtr();

        DelegatePtr result = mDelegateImpl->find((Subscription *)(subscription.get()));
        if (!result) return result;

        if (strongReferenceToDelgate) {
          return DelegateProxy::create(result); //  create a strong reference
        }
        return DelegateProxy::createWeak(result); //  create a strong reference
      }

      void clear() noexcept
      {
        return mDelegateImpl->clear((Subscription *)NULL);
      }

      size_type size() const noexcept
      {
        return mDelegateImpl->size((Subscription *)NULL);
      }

    public:

      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //
      // ProxySubscriptions::Subscription
      //

      class Subscription : public SUBSCRIPTIONBASECLASS
      {
      public:
        Subscription() noexcept :
          mID(zsLib::createPUID())
        {
        }

        ~Subscription() noexcept
        {
          mThisWeak.reset();
          cancel();
        }

        virtual PUID getID() const noexcept {return mID;}

        virtual void cancel() noexcept
        {
          mDelegateImpl->cancel(this);
        }

        virtual void background() noexcept
        {
          mDelegateImpl->background(mThisWeak.lock());
        }

      public:
        PUID mID;
        SubscriptionWeakPtr mThisWeak;
        DelegateImplPtr mDelegateImpl;
      };
      
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //
      // ProxySubscriptions::DelegateImpl
      //

      class DelegateImpl : public XINTERFACE
      {
      public:
        typedef typename ProxySubscriptions::SubscriptionDelegateMap SubscriptionDelegateMap;

        DelegateImpl() noexcept : mSubscriptions(make_shared<SubscriptionDelegateMap>()) {}
        ~DelegateImpl() noexcept {}

        template<typename PARAM>
        static void fillWithSubscription(PARAM &, SubscriptionWeakPtr &, SubscriptionPtr &, ZS_MAYBE_USED() bool &filled) noexcept
        {
          ZS_MAYBE_USED(filled);
        }

        template<typename PARAM>
        static void fillWithSubscription(SubscriptionBaseClassPtr &result, SubscriptionWeakPtr &source, SubscriptionPtr &output, bool &filled) noexcept
        {
          output = source.lock();
          result = output;
          filled = true;
        }

        void subscribe(SubscriptionPtr &subscription, DelegatePtr &delegate) noexcept
        {
          AutoRecursiveLock lock(mLock);
          SubscriptionDelegateMapPtr temp(new SubscriptionDelegateMap(*mSubscriptions));
          (*temp)[subscription.get()] = SubscriptionDelegatePair(subscription, delegate);
          mSubscriptions = temp;
        }

        void cancel(Subscription *gone) noexcept
        {
          AutoRecursiveLock lock(mLock);
          SubscriptionDelegateMapPtr temp(make_shared<SubscriptionDelegateMap>(*mSubscriptions));
          typename SubscriptionDelegateMap::iterator found = temp->find(gone);
          if (found == temp->end()) return;
          temp->erase(found);
          mSubscriptions = temp;
        }

        void background(SubscriptionPtr subscription) noexcept
        {
          AutoRecursiveLock lock(mLock);
          mBackgroundSubscriptions[subscription] = true;
        }

        void clear(ZS_MAYBE_USED() Subscription *ignore) noexcept
        {
          ZS_MAYBE_USED(ignore);
          AutoRecursiveLock lock(mLock);
          SubscriptionDelegateMapPtr temp(make_shared<SubscriptionDelegateMap>());
          mSubscriptions = temp;
          mBackgroundSubscriptions.clear();
        }

        size_type size(ZS_MAYBE_USED() Subscription *ignore) noexcept
        {
          ZS_MAYBE_USED(ignore);
          AutoRecursiveLock lock(mLock);
          return mSubscriptions->size();
        }

        DelegatePtr find(Subscription *subscription) noexcept
        {
          AutoRecursiveLock lock(mLock);
          typename SubscriptionDelegateMap::iterator found = mSubscriptions->find(subscription);
          if (found == mSubscriptions->end()) return DelegatePtr();

          return (*found).second.second;
        }

      public:
        zsLib::RecursiveLock mLock;
        SubscriptionDelegateMapPtr mSubscriptions;

        SubscriptionBackgroundMap mBackgroundSubscriptions;
      };

    public:
      //-----------------------------------------------------------------------
      //
      // ProxySubscriptions => (data)
      //

      DelegateImplPtr mDelegateImpl;
    };
  }
}


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_IMPLEMENT(xInterface, xSubscriptionClass)                                         \
  namespace zsLib                                                                                                                 \
  {                                                                                                                               \
    void declareProxySubscriptionInterface(const xInterface &, const xSubscriptionClass &) noexcept                               \
    {                                                                                                                             \
      zsLib::ProxySubscriptions<xInterface, xSubscriptionClass> temp;                                                             \
      (void)temp;                                                                                                                 \
      zsLib::ProxySubscriptions<xInterface, xSubscriptionClass>::create();                                                        \
    }                                                                                                                             \
  }

#define ZS_INTERNAL_DECLARE_INTERACTION_PROXY_SUBSCRIPTION(xInteractionName, xDelegateName)                                       \
  interaction xInteractionName;                                                                                                   \
  typedef std::shared_ptr<xInteractionName> xInteractionName##Ptr;                                                                \
  typedef std::weak_ptr<xInteractionName> xInteractionName##WeakPtr;                                                              \
  typedef zsLib::ProxySubscriptions<xDelegateName, xInteractionName> xDelegateName##Subscriptions;


#ifndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_BEGIN(xInterface, xSubscriptionClass)                                             \
namespace zsLib                                                                                                                   \
{                                                                                                                                 \
  template<>                                                                                                                      \
  class ProxySubscriptions<xInterface, xSubscriptionClass> : public internal::ProxySubscriptions<xInterface, xSubscriptionClass>  \
  {                                                                                                                               \
  public:                                                                                                                         \
    typedef ProxySubscriptions<xInterface, xSubscriptionClass> ProxySubscriptionsType;                                            \
    class DerivedDelegateImpl;                                                                                                    \
                                                                                                                                  \
    ProxySubscriptions() noexcept;                                                                                                \
    ProxySubscriptions(const ProxySubscriptionsType &source) noexcept;                                                            \
                                                                                                                                  \
    static ProxySubscriptionsType create();                                                                                       \
                                                                                                                                  \
    class DerivedDelegateImpl : public DelegateImpl                                                                               \
    {                                                                                                                             \
    public:                                                                                                                       \


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_END()                                                                             \
    };                                                                                                                            \
  };                                                                                                                              \
}

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_TYPEDEF(xOriginalType, xTypeAlias)                                                \
    typedef xOriginalType xTypeAlias;

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_0(xConst,xThrow,xMethod)                                                                            \
    void xMethod() xConst xThrow override;                                                                                                                 \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_1(xConst,xThrow,xMethod,t1)                                                                         \
    void xMethod(t1 v1) xConst xThrow override;                                                                                                            \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_2(xConst,xThrow,xMethod,t1,t2)                                                                      \
    void xMethod(t1 v1, t2 v2) xConst xThrow override;                                                                                                     \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_3(xConst,xThrow,xMethod,t1,t2,t3)                                                                   \
    void xMethod(t1 v1, t2 v2, t3 v3) xConst xThrow override;                                                                                              \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_4(xConst,xThrow,xMethod,t1,t2,t3,t4)                                                                \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4) xConst xThrow override;                                                                                       \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_5(xConst,xThrow,xMethod,t1,t2,t3,t4,t5)                                                             \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5) xConst xThrow override;                                                                                \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_6(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6)                                                          \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6) xConst xThrow override;                                                                         \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_7(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7)                                                       \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7) xConst xThrow override;                                                                  \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_8(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8)                                                    \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8) xConst xThrow override;                                                           \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_9(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9)                                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9) xConst xThrow override;                                                    \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_10(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10)                                            \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10) xConst xThrow override;                                           \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_11(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11)                                        \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11) xConst xThrow override;                                  \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_12(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12)                                    \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12) xConst xThrow override;                         \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_13(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13)                                \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13) xConst xThrow override;                \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_14(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14)                                            \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14) xConst xThrow override;                       \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_15(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15)                                        \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15) xConst xThrow override;              \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_16(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16)                                                                                                            \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16) xConst xThrow override;                                                                             \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_17(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17)                                                                                                        \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17) xConst xThrow override;                                                                    \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_18(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18)                                                                                                    \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18) xConst xThrow override;                                                           \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_19(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19)                                                                                                \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19) xConst xThrow override;                                                  \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_20(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20)                                                                                            \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20) xConst xThrow override;                                         \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_21(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21)                                                                                        \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21) xConst xThrow override;                                \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_22(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22)                                                                                    \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22) xConst xThrow override;                       \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_23(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23)                                                                                \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23) xConst xThrow override;              \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_24(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24)                                                                                            \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23, t24 v24) xConst xThrow override;                     \

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_25(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24,t25)                                                                                        \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23, t24 v24, t25 v25) xConst xThrow override;            \


#else //ndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_BEGIN(xInterface, xSubscriptionClass)                                               \
namespace zsLib                                                                                                                     \
{                                                                                                                                   \
  template<>                                                                                                                        \
  class ProxySubscriptions<xInterface, xSubscriptionClass> : public internal::ProxySubscriptions<xInterface, xSubscriptionClass>    \
  {                                                                                                                                 \
  public:                                                                                                                           \
    typedef ProxySubscriptions<xInterface, xSubscriptionClass> ProxySubscriptionsType;                                              \
    typedef std::shared_ptr<ProxySubscriptionsType> ProxySubscriptionsTypePtr;                                                      \
    class DerivedDelegateImpl;                                                                                                      \
                                                                                                                                    \
    ProxySubscriptions() noexcept : internal::ProxySubscriptions<xInterface, xSubscriptionClass>(make_shared<DerivedDelegateImpl>()) \
    {                                                                                                                               \
    }                                                                                                                               \
                                                                                                                                    \
    ProxySubscriptions(const ProxySubscriptionsType &source) noexcept : internal::ProxySubscriptions<xInterface, xSubscriptionClass>(source) \
    {                                                                                                                               \
    }                                                                                                                               \
                                                                                                                                    \
    static ProxySubscriptionsType create() noexcept                                                                                 \
    {                                                                                                                               \
      ProxySubscriptionsType result;                                                                                                \
      return result;                                                                                                                \
    }                                                                                                                               \
                                                                                                                                    \
    class DerivedDelegateImpl : public DelegateImpl                                                                                 \
    {                                                                                                                               \
    public:                                                                                                                         \


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_END()                                                                             \
    };                                                                                                                            \
  };                                                                                                                              \
}

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_TYPEDEF(xOriginalType, xTypeAlias)                                                \
    typedef xOriginalType xTypeAlias;


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_TYPES_AND_VALUES(xSubscriptionsMapTypename, xSubscriptionsMapVariable, xSubscriptionsMapKeyTypename, xDelegatPtrTypename, xDelegateProxyTypename)  \
  typedef ProxySubscriptionsType::SubscriptionDelegateMap xSubscriptionsMapTypename;                                                                                                                      \
  typedef ProxySubscriptionsType::SubscriptionDelegateMap::key_type xSubscriptionsMapKeyTypename;                                                                                                         \
  typedef ProxySubscriptionsType::SubscriptionDelegateMap::value_type xDelegatPtrTypename;                                                                                                                \
  typedef ProxySubscriptionsType::DelegateProxy xDelegateProxyTypename;                                                                                                                                   \
  SubscriptionDelegateMapPtr _temp_subscriptions;                                                                                                                                                         \
  {                                                                                                                                                                                                       \
    AutoRecursiveLock lock(mLock);                                                                                                                                                                        \
    _temp_subscriptions = mSubscriptions;                                                                                                                                                                 \
  }                                                                                                                                                                                                       \
  xSubscriptionsMapTypename &xSubscriptionsMapVariable = (*_temp_subscriptions);

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ITERATOR_VALUES(xIterator, xKeyValueName, xSusbcriptionWeakPtrValueName, xDelegatePtrValueName)  \
  const ProxySubscriptionsType::SubscriptionDelegateMap::key_type &xKeyValueName = (*xIterator).first; (void)xKeyValueName;                             \
  ProxySubscriptionsType::SubscriptionWeakPtr &xSusbcriptionWeakPtrValueName = (*xIterator).second.first; (void)xSusbcriptionWeakPtrValueName;          \
  ProxySubscriptionsType::DelegatePtr &xDelegatePtrValueName = (*xIterator).second.second; (void)xDelegatePtrValueName;

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ERASE_KEY(xSubscriptionsMapKeyValue)                                                             \
  {DelegateImpl::cancel(xSubscriptionsMapKeyValue);}


#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_0(xConst,xThrow,xMethod)                                                                     \
    void xMethod() xConst xThrow override {                                                                                                         \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          (*current).second.second->xMethod();                                                                                                      \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_1(xConst,xThrow,xMethod,t1)                                                                  \
    void xMethod(t1 v1) xConst xThrow override {                                                                                                    \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1);                                                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_2(xConst,xThrow,xMethod,t1,t2)                                                               \
    void xMethod(t1 v1, t2 v2) xConst xThrow override {                                                                                             \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2);                                                                                                 \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_3(xConst,xThrow,xMethod,t1,t2,t3)                                                            \
    void xMethod(t1 v1, t2 v2, t3 v3) xConst xThrow override {                                                                                      \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3);                                                                                              \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_4(xConst,xThrow,xMethod,t1,t2,t3,t4)                                                         \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4) xConst xThrow override {                                                                               \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4);                                                                                           \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_5(xConst,xThrow,xMethod,t1,t2,t3,t4,t5)                                                      \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5) xConst xThrow override {                                                                        \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5);                                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_6(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6)                                                   \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6) xConst xThrow override {                                                                 \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6);                                                                                     \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_7(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7)                                                \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7) xConst xThrow override {                                                          \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7);                                                                                  \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_8(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8)                                             \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8) xConst xThrow override {                                                   \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8);                                                                               \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_9(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9)                                          \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9) xConst xThrow override {                                            \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                    \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9);                                                                            \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_10(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10)                                     \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10) xConst xThrow override {                                   \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10);                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_11(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11)                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11) xConst xThrow override {                          \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11);                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_12(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12)                             \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12) xConst xThrow override {                 \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12);                                                                \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_13(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13)                         \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13) xConst xThrow override {        \
      SubscriptionDelegateMapPtr subscription;                                                                                                      \
      {                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                              \
        subscription = mSubscriptions;                                                                                                              \
      }                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                   \
        try {                                                                                                                                       \
          bool filled = false;                                                                                                                      \
          SubscriptionPtr result;                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                  \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13);                                                            \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                        \
          cancel((*current).first);                                                                                                                 \
        }                                                                                                                                           \
      }                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_14(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14)                                     \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14) xConst xThrow override {               \
      SubscriptionDelegateMapPtr subscription;                                                                                                                      \
      {                                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                                              \
        subscription = mSubscriptions;                                                                                                                              \
      }                                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                   \
        try {                                                                                                                                                       \
          bool filled = false;                                                                                                                                      \
          SubscriptionPtr result;                                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14);                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                        \
          cancel((*current).first);                                                                                                                                 \
        }                                                                                                                                                           \
      }                                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_15(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15)                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15) xConst xThrow override {      \
      SubscriptionDelegateMapPtr subscription;                                                                                                                      \
      {                                                                                                                                                             \
        AutoRecursiveLock lock(mLock);                                                                                                                              \
        subscription = mSubscriptions;                                                                                                                              \
      }                                                                                                                                                             \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                         \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                   \
        try {                                                                                                                                                       \
          bool filled = false;                                                                                                                                      \
          SubscriptionPtr result;                                                                                                                                   \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                    \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                  \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                  \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                          \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15);                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                        \
          cancel((*current).first);                                                                                                                                 \
        }                                                                                                                                                           \
      }                                                                                                                                                             \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_16(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16)                                                                                                     \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16) xConst xThrow override {                                                                     \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16);                                                                                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_17(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17)                                                                                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17) xConst xThrow override {                                                            \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17);                                                                                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_18(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18)                                                                                             \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18) xConst xThrow override {                                                   \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18);                                                                                                                                \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_19(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19)                                                                                         \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19) xConst xThrow override {                                          \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19);                                                                                                                            \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_20(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20)                                                                                     \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20) xConst xThrow override {                                 \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20);                                                                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_21(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21)                                                                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21) xConst xThrow override {                        \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t21>(v21, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20,v21);                                                                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_22(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22)                                                                             \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22) xConst xThrow override {               \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t21>(v21, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t22>(v22, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20,v21,v22);                                                                                                                \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_23(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23)                                                                         \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23) xConst xThrow override {      \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                           \
        try {                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t21>(v21, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t22>(v22, (*current).second.first, result, filled);                                                                                                                                                          \
          fillWithSubscription<t23>(v23, (*current).second.first, result, filled);                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20,v21,v22,v23);                                                                                                            \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_24(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24)                                                                                     \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23, t24 v24) xConst xThrow override {             \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                                           \
        try {                                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t21>(v21, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t22>(v22, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t23>(v23, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t24>(v24, (*current).second.first, result, filled);                                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20,v21,v22,v23,v24);                                                                                                                        \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                                     \
    }

#define ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_25(xConst,xThrow,xMethod,t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24,t25)                                                                                 \
    void xMethod(t1 v1, t2 v2, t3 v3, t4 v4, t5 v5, t6 v6, t7 v7, t8 v8, t9 v9, t10 v10, t11 v11, t12 v12, t13 v13, t14 v14, t15 v15, t16 v16, t17 v17, t18 v18, t19 v19, t20 v20, t21 v21, t22 v22, t23 v23, t24 v24, t25 v25) xConst xThrow override {    \
      SubscriptionDelegateMapPtr subscription;                                                                                                                                                                                                              \
      {                                                                                                                                                                                                                                                     \
        AutoRecursiveLock lock(mLock);                                                                                                                                                                                                                      \
        subscription = mSubscriptions;                                                                                                                                                                                                                      \
      }                                                                                                                                                                                                                                                     \
      for (SubscriptionDelegateMap::iterator iter = subscription->begin(); iter != subscription->end(); ) {                                                                                                                                                 \
        SubscriptionDelegateMap::iterator current = iter; ++iter;                                                                                                                                                                                           \
        try {                                                                                                                                                                                                                                               \
          bool filled = false;                                                                                                                                                                                                                              \
          SubscriptionPtr result;                                                                                                                                                                                                                           \
          fillWithSubscription<t1>(v1, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t2>(v2, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t3>(v3, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t4>(v4, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t5>(v5, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t6>(v6, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t7>(v7, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t8>(v8, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t9>(v9, (*current).second.first, result, filled);                                                                                                                                                                            \
          fillWithSubscription<t10>(v10, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t11>(v11, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t12>(v12, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t13>(v13, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t14>(v14, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t15>(v15, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t16>(v16, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t17>(v17, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t18>(v18, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t19>(v19, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t20>(v20, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t21>(v21, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t22>(v22, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t23>(v23, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t24>(v24, (*current).second.first, result, filled);                                                                                                                                                                          \
          fillWithSubscription<t25>(v25, (*current).second.first, result, filled);                                                                                                                                                                          \
          if ((filled) && (!result)) {cancel((*current).first); continue;}                                                                                                                                                                                  \
          (*current).second.second->xMethod(v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17,v18,v19,v20,v21,v22,v23,v24,v25);                                                                                                                    \
        } catch(DelegateProxy::Exceptions::DelegateGone &) {                                                                                                                                                                                                \
          cancel((*current).first);                                                                                                                                                                                                                         \
        }                                                                                                                                                                                                                                                   \
      }                                                                                                                                                                                                                                                     \
    }

#endif //ndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION


#endif //ZSLIB_INTERNAL_PROXY_SUBSCRIPTIONS_H_cf7229753b441247ccece9f3b92317ed96045660
