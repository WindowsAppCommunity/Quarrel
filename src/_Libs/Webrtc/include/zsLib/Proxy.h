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

namespace zsLib
{
  class ProxyBase
  {
  public:
    struct Exceptions
    {
      ZS_DECLARE_CUSTOM_EXCEPTION(DelegateGone)
      ZS_DECLARE_CUSTOM_EXCEPTION(MissingDelegateMessageQueue)
    };
  };

  template <typename XINTERFACE>
  class Proxy : public ProxyBase
  {
  public:
    struct Exceptions
    {
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE(DelegateGone, ProxyBase::Exceptions::DelegateGone)
    };

    ZS_DECLARE_TYPEDEF_PTR(XINTERFACE, Delegate)

  public:
    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction by assuming the
    //          object that implemented the interaction derived from
    //          zsLib::MessageQueueAssociator.
    // EXAMPLE:
    //
    // class MyClassThatReceivesEvents : public IDelegate,
    //                                   public MessageQueueAssociator {
    //
    //     ...
    //     void receiveEvents() {
    //       mObject->subscribeEvents(mWeakThis.lock());
    //     }
    //     ...
    //
    //   protected:
    //     MyClassThatReceivesEventsWeakPtr mWeakThis;
    //     ObjectPtr mObject;
    // };
    //
    // void Object::subscribeEvents(IDelegatePtr delegate) {
    //   IDelegatePtr delegateProxy = zsLib::Proxy<IDelegate>::create(delegate);
    //
    static DelegatePtr create(DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__)                                 {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction. The object is first
    //          checked if it derived from the zsLib::MessageQueueAssociator
    //          and the associated message queue is used and the method will
    //          used the passed in IMessageQueuePtr as a fallback.
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IMessageQueuePtr queue, IDelegatePtr delegate) {
    //   IDelegatePtr delegateProxy = zsLib::Proxy<IDelegate>::create(queue, delegate);
    //
    static DelegatePtr create(IMessageQueuePtr queue, DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__)         {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction. The object's
    //          message queue from zsLib::MessageQueueAssociator is only used
    //          as a fallback if the queue passsed in is null.
    static DelegatePtr createUsingQueue(IMessageQueuePtr queue, DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__) { return delegate->proxy_implementation_for_this_interface_is_not_defined(); }

    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction but use a weak
    //          reference to the original delegate (thus the delegate might
    //          disappear at any time). Assumes the objec that implemented the
    //          delegate derived from zsLib::MessageQueueAssociator.
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IDelegatePtr delegate) {
    //   IDelegatePtr delegateProxy = zsLib::Proxy<IDelegate>::createWeak(delegate);
    //
    //   try {
    //     delegate->onEvent();
    //   } catch(zsLib::Proxy<IDelegate>::Exceptions::DelegateGone) {
    //     // NOTE: delegate is already destroyed - can clean up this delegate proxy now
    //   }
    //
    static DelegatePtr createWeak(DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__)                             {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction but use a weak
    //          reference to the original delegate (thus the delegate might
    //          disappear at any time). Checks if the delegate was derived
    //          from zsLib::MessageQueueAssociator and if not then uses the
    //          IMessageQueue passed in instead.
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IMessageQueuePtr queue, IDelegatePtr delegate) {
    //   IDelegatePtr delegateProxy = zsLib::Proxy<IDelegate>::createWeak(queue, delegate);
    //
    //   try {
    //     delegate->onEvent();
    //   } catch(zsLib::Proxy<IDelegate>::Exceptions::DelegateGone) {
    //     // NOTE: delegate is already destroyed - can clean up this delegate proxy now
    //   }
    //
    static DelegatePtr createWeak(IMessageQueuePtr queue, DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__)     {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Create a proxy for a delegate interaction but use a weak
    ///         reference to the original delegate. The object's message queue
    //          from zsLib::MessageQueueAssociator is only used as a fallback
    //          if the queue passsed in is null.
    static DelegatePtr createWeakUsingQueue(IMessageQueuePtr queue, DelegatePtr delegate, bool throwDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__) { return delegate->proxy_implementation_for_this_interface_is_not_defined(); }

    //------------------------------------------------------------------------
    // PURPOSE: Create a no operation proxy for a delegate interaction but
    //          does not reference any original delegate.
    // EXAMPLE:
    //
    // void Object::ignoreEvents(IMessageQueuePtr queue) {
    //   IDelegatePtr delegateProxy = zsLib::Proxy<IDelegate>::createNoop(queue);
    //
    //   try {
    //     delegate->onEvent();
    //   } catch(zsLib::Proxy<IDelegate>::Exceptions::DelegateGone) {
    //     // NOTE: delegate is already destroyed - can clean up this delegate proxy now
    //   }
    //
    static DelegatePtr createNoop(IMessageQueuePtr queue, bool throwsDelegateGone = false, int line = __LINE__, const char *fileName = __FILE__)                                    {return DelegatePtr()->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns true if the delegate passed in is a proxy.
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IDelegatePtr delegate) {
    //   bool isProxy = zsLib::Proxy<IDelegate>::isProxy(delegate);
    //
    static bool isProxy(DelegatePtr delegate) noexcept                                                                                                                              {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns the original delegate from a proxy (or if the object
    //          is not a proxy just returns the object passed in since it is
    //          the delegate).
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IDelegatePtr delegate) {
    //   IDelegatePtr originalDelegatePtr = zsLib::Proxy<IDelegate>::original(delegate);
    //
    static DelegatePtr original(DelegatePtr delegate, bool throwDelegateGone = false)                                                                                               {return delegate->proxy_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns the associated message queue from a proxy (or NULL if
    //          no message queue is associated).
    // EXAMPLE:
    //
    // void Object::subscribeEvents(IDelegatePtr delegate) {
    //   IMessageQueuePtr queue = zsLib::Proxy<IDelegate>::getAssociatedMessageQueue(delegate);
    //
    static IMessageQueuePtr getAssociatedMessageQueue(DelegatePtr delegate) noexcept                                                                                                {return delegate->proxy_implementation_for_this_interface_is_not_defined();}
  };
}

#include <zsLib/internal/zsLib_Proxy.h>

namespace zsLib
{
  // get the total number of proxies that are currently constructed and not destroyed
  ULONG proxyGetTotalConstructed();
  void proxyDump();
}

#if 0
#define ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION 1
#endif //0

#define ZS_DECLARE_INTERACTION_PROXY(xInteractionName)                                                                                                        ZS_INTERNAL_DECLARE_INTERACTION_PROXY(xInteractionName)
#define ZS_DECLARE_TYPEDEF_PROXY(xOriginalType, xNewTypeName)                                                                                                 ZS_INTERNAL_DECLARE_TYPEDEF_PROXY(xOriginalType, xNewTypeName)
#define ZS_DECLARE_USING_PROXY(xNamespace, xExistingType)                                                                                                     ZS_INTERNAL_DECLARE_USING_PROXY(xNamespace, xExistingType)

#define ZS_DECLARE_PROXY_IMPLEMENT(xInterface)                                                                                                                ZS_INTERNAL_DECLARE_PROXY_IMPLEMENT(xInterface)

#define ZS_DECLARE_PROXY_BEGIN(xInterface)                                                                                                                    ZS_INTERNAL_DECLARE_PROXY_BEGIN(xInterface, true)
#define ZS_DECLARE_PROXY_WITH_DELEGATE_MESSAGE_QUEUE_OPTIONAL_BEGIN(xInterface)                                                                               ZS_INTERNAL_DECLARE_PROXY_BEGIN(xInterface, false)
#define ZS_DECLARE_PROXY_END()                                                                                                                                ZS_INTERNAL_DECLARE_PROXY_END()

#define ZS_DECLARE_PROXY_TYPEDEF(xOriginalType, xTypeAlias)                                                                                                   ZS_INTERNAL_DECLARE_PROXY_TYPEDEF(xOriginalType, xTypeAlias)

#define ZS_DECLARE_PROXY_METHOD_SYNC(xMethod, ...)                                                                                                            ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_CONST(xMethod, ...)                                                                                                      ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_RETURN(xMethod, xReturnType, ...)                                                                                        ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_RETURN_CONST(xMethod, xReturnType, ...)                                                                                  ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)

#define ZS_DECLARE_PROXY_METHOD_SYNC_THROWS(xMethod, ...)                                                                                                     ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_CONST_THROWS(xMethod, ...)                                                                                               ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_RETURN_THROWS(xMethod, xReturnType, ...)                                                                                 ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_PROXY_METHOD_SYNC_RETURN_CONST_THROWS(xMethod, xReturnType, ...)                                                                           ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)

#define ZS_DECLARE_PROXY_METHOD(...)                                                                                                                          ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(ZS_INTERNAL_DECLARE_PROXY_METHOD, __VA_ARGS__)
