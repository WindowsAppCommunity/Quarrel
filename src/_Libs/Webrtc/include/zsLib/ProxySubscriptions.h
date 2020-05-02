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

#include <zsLib/Exception.h>
#include <zsLib/IMessageQueue.h>

#include <zsLib/Proxy.h>

namespace zsLib
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
    typedef size_t size_type;

    ZS_DECLARE_TYPEDEF_PTR(XINTERFACE, Delegate)
    ZS_DECLARE_TYPEDEF_PTR(SUBSCRIPTIONBASECLASS, BaseSubscription)

    ZS_DECLARE_CLASS_PTR(Subscription)

  public:
    //-------------------------------------------------------------------------
    // PURPOSE: subscribe to all events fired on the "delegate()" object
    // NOTE:    will auto-create a proxy and return weak (or strong)
    //          reference to the original proxy.
    SubscriptionPtr subscribe(
                              DelegatePtr delgate,
                              IMessageQueuePtr queue = IMessageQueuePtr(),
                              bool strongReferenceToDelgate = false
                              )                           {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

    //-------------------------------------------------------------------------
    // PURPOSE: obtain the delegate where events fired on the delegate
    //          cause all the attached subscriptions to fire as well.
    DelegatePtr delegate() const                          {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

    //-------------------------------------------------------------------------
    // PURPOSE: obtain a strong (or weak) proxy reference to the original
    //          delegate given a subscription.
    DelegatePtr delegate(
                         BaseSubscriptionPtr subscription,
                         bool strongReferenceToDelgate = true
                         ) const                          {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

    //-------------------------------------------------------------------------
    // PURPOSE: clear out any subscriptions and prevent any delegates from
    //          receiving events.
    void clear()                                          {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

    //-------------------------------------------------------------------------
    // PURPOSE: obtain the total number of subscribers to the delegate being
    //          fired.
    size_t size() const                                   {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

  public:
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Subscription
    //

    class Subscription : public BaseSubscription
    {
      //-----------------------------------------------------------------------
      // PURPOSE: cancel the active subscription to delegate events
      virtual PUID getID() const noexcept = 0;

      //-----------------------------------------------------------------------
      // PURPOSE: cancel the active subscription to delegate events
      virtual void cancel() noexcept = 0;

      //-----------------------------------------------------------------------
      // PURPOSE: cause events to continue to fire for the subscribed delegate
      //          even if a reference to the subscription object is not
      //          mantained.
      virtual void background() noexcept = 0;
    };
  };
}

#include <zsLib/internal/zsLib_ProxySubscriptions.h>

#define ZS_DECLARE_INTERACTION_PROXY_SUBSCRIPTION(xInteractionName, xDelegateName)                                                                                            ZS_INTERNAL_DECLARE_INTERACTION_PROXY_SUBSCRIPTION(xInteractionName, xDelegateName)

//#define ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_IMPLEMENT(xInterface, xSubscriptionClass)                                                                                              ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_IMPLEMENT(xInterface, xSubscriptionClass)

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_BEGIN(xInterface, xSubscriptionClass)                                                                                                  ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_BEGIN(xInterface, xSubscriptionClass)
#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_END()                                                                                                                                  ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_END()

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_TYPEDEF(xOriginalType, xTypeAlias)                                                                                                     ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_TYPEDEF(xOriginalType, xTypeAlias)

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_TYPES_AND_VALUES(xSubscriptionsMapTypename, xSubscriptionsMapVariable, xSubscriptionsMapKeyTypename, xDelegatPtrTypename, xDelegateProxyTypename)     ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_TYPES_AND_VALUES(xSubscriptionsMapTypename, xSubscriptionsMapVariable, xSubscriptionsMapKeyTypename, xDelegatPtrTypename, xDelegateProxyTypename)
#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ITERATOR_VALUES(xIterator, xKeyValueName, xSusbcriptionWeakPtrValueName, xDelegatePtrValueName)                                                       ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ITERATOR_VALUES(xIterator, xKeyValueName, xSusbcriptionWeakPtrValueName, xDelegatePtrValueName)
#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ERASE_KEY(xSubscriptionsMapKeyValue)                                                                                                                  ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_ERASE_KEY(xSubscriptionsMapKeyValue)


#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD(xMethod, /* types */ ...)                                                                                                       ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW_DECLARE, xMethod, __VA_ARGS__)

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_SYNC(xMethod, /* types */ ...)                                                                                                  ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_SYNC_CONST(xMethod, /* types */ ...)                                                                                            ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)

#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_SYNC_THROWS(xMethod, /* types */ ...)                                                                                           ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_SUBSCRIPTIONS_METHOD_SYNC_CONST_THROWS(xMethod, /* types */ ...)                                                                                     ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_DECLARE_PROXY_SUBSCRIPTIONS_METHOD, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
