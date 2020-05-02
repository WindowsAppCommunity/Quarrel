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

#include <zsLib/internal/types.h>

#define ZS_BUILD_NOTE(xFieldName, xMsg)                                             ZS_INTERNAL_BUILD_NOTE(xFieldName, xMsg)

#define ZS_MACRO_GET_ARG_COUNT(...)                                                 ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(__VA_ARGS__)
#define ZS_MACRO_SELECT(NAME, ...)                                                  ZS_INTERNAL_MACRO_SELECT(NAME, __VA_ARGS__)

// These macros are defined internally but are not shown here due to an issue with the different behaviours between compilers regarding __VA_ARGS__
//#define ZS_MACRO_SELECT_WITH_PROPERTY_1(NAME, PROP1, ...)
//#define ZS_MACRO_SELECT_WITH_PROPERTY_2(NAME, PROP1, PROP2, ...)
//#define ZS_MACRO_SELECT_WITH_PROPERTY_3(NAME, PROP1, PROP2, PROP3, ...)
//#define ZS_MACRO_SELECT_WITH_PROPERTY_4(NAME, PROP1, PROP2, PROP3, PROP4, ...)
//#define ZS_MACRO_SELECT_WITH_PROPERTY_5(NAME, PROP1, PROP2, PROP3, PROP4, PROP5, ...)
//#define ZS_MACRO_SELECT_WITH_PROPERTY_6(NAME, PROP1, PROP2, PROP3, PROP4, PROP5, PROP6, ...)

#define ZS_MAYBE_USED(...)                                                          ZS_INTERNAL_MAYBE_USED(__VA_ARGS__)
#define ZS_NO_DISCARD()                                                             ZS_INTERNAL_NO_DISCARD()

#define ZS_ASSERT(xCondition)                                                       ZS_INTERNAL_ASSERT(xCondition)
#define ZS_ASSERT_MESSAGE(xCondition, xMsg)                                         ZS_INTERNAL_ASSERT_MESSAGE(xCondition, xMsg)
#define ZS_ASSERT_FAIL(xMsg)                                                        ZS_INTERNAL_ASSERT_FAIL(xMsg)

#define ZS_DECLARE_PTR(xExistingType)                                               ZS_INTERNAL_DECLARE_PTR(xExistingType)
#define ZS_DECLARE_USING_PTR(xNamespace, xExistingType)                             ZS_INTERNAL_DECLARE_USING_PTR(xNamespace, xExistingType)
#define ZS_DECLARE_CLASS_PTR(xClassName)                                            ZS_INTERNAL_DECLARE_CLASS_PTR(xClassName)
#define ZS_DECLARE_STRUCT_PTR(xStructName)                                          ZS_INTERNAL_DECLARE_STRUCT_PTR(xStructName)
#define ZS_DECLARE_INTERACTION_PTR(xInteractionName)                                ZS_INTERNAL_DECLARE_STRUCT_PTR(xInteractionName)
#define ZS_DECLARE_TYPEDEF_PTR(xOriginalType, xNewTypeName)                         ZS_INTERNAL_DECLARE_TYPEDEF_PTR(xOriginalType, xNewTypeName)
#define ZS_DECLARE_NOOP(xExistingType)
#define ZS_DYNAMIC_PTR_CAST(xType, xObject)                                         ZS_INTERNAL_DYNAMIC_PTR_CAST(xType, xObject)


namespace zsLib
{
  using std::make_shared;
  using std::size_t;
  
  ZS_DECLARE_TYPEDEF_PTR(std::thread, Thread)
  ZS_DECLARE_TYPEDEF_PTR(std::mutex, Lock)
  ZS_DECLARE_TYPEDEF_PTR(std::recursive_mutex, RecursiveLock)

  typedef std::lock_guard<Lock> AutoLock;
  typedef std::lock_guard<RecursiveLock> AutoRecursiveLock;

  typedef std::chrono::system_clock::time_point Time;
  typedef std::chrono::duration<std::chrono::hours::rep, std::ratio<3600 * 24> > Days;
  typedef std::chrono::hours Hours;
  typedef std::chrono::minutes Minutes;
  typedef std::chrono::seconds Seconds;
  typedef std::chrono::milliseconds Milliseconds;
  typedef std::chrono::microseconds Microseconds;
  typedef std::chrono::nanoseconds Nanoseconds;

  typedef CHAR ZsDeclareCHAR;
  typedef UCHAR ZsDeclareUCHAR;
  typedef SHORT ZsDeclareSHORT;
  typedef USHORT ZsDeclareUSHORT;
  typedef INT ZsDeclareINT;
  typedef UINT ZsDeclareUINT;
  typedef ULONG ZsDeclareULONG;
  typedef LONGLONG ZsDeclareLONGLONG;
  typedef ULONGLONG ZsDeclareULONGLONG;

  typedef FLOAT ZsDeclareFLOAT;
  typedef DOUBLE ZsDeclareDOUBLE;

  typedef BYTE ZsDeclareBYTE;
  typedef WORD ZsDeclareWORD;
  typedef DWORD ZsDeclareDWORD;
  typedef QWORD ZsDeclareQWORD;

  typedef PTRNUMBER ZsDeclarePTRNUMBER;
  typedef USERPARAM ZsDeclareUSERPARAM;

  typedef PUID ZsDeclarePUID;
  typedef UUID ZsDeclareUUID;

  typedef TCHAR ZsDeclareTCHAR;
  typedef WCHAR ZsDeclareWCHAR;

  typedef STR ZsDeclareSTR;
  typedef CSTR ZsDeclareCSTR;
  typedef TSTR ZsDeclareTSTR;
  typedef CTSTR ZsDeclareCTSTR;
  typedef WSTR ZsDeclareWSTR;
  typedef CWSTR ZsDeclareCWSTR;

  typedef LONGEST zsDeclareLONGEST;
  typedef ULONGEST zsDeclareULONGEST;

  class PrivateGlobalLock;

  ZS_DECLARE_CLASS_PTR(Event);
  ZS_DECLARE_CLASS_PTR(Exception);
  ZS_DECLARE_CLASS_PTR(IPAddress);

  ZS_DECLARE_CLASS_PTR(Log);
  ZS_DECLARE_INTERACTION_PTR(ILogOutputDelegate);
  ZS_DECLARE_INTERACTION_PTR(ILogEventingProviderDelegate);
  ZS_DECLARE_INTERACTION_PTR(ILogEventingDelegate);

  ZS_DECLARE_CLASS_PTR(SingletonManager);
  ZS_DECLARE_INTERACTION_PTR(ISingletonManagerDelegate);

  ZS_DECLARE_INTERACTION_PTR(IMessageQueue);
  ZS_DECLARE_INTERACTION_PTR(IMessageQueueMessage);
  ZS_DECLARE_INTERACTION_PTR(IMessageQueueNotify);
  ZS_DECLARE_INTERACTION_PTR(IMessageQueueManager);
  ZS_DECLARE_INTERACTION_PTR(IMessageQueueThread);
  ZS_DECLARE_INTERACTION_PTR(IMessageQueueDispatcher);

  ZS_DECLARE_INTERACTION_PTR(IMessageQueueThreadPool);

  ZS_DECLARE_CLASS_PTR(Promise);
  ZS_DECLARE_INTERACTION_PTR(IPromiseDelegate);
  ZS_DECLARE_INTERACTION_PTR(IPromiseSettledDelegate);
  ZS_DECLARE_INTERACTION_PTR(IPromiseResolutionDelegate);
  ZS_DECLARE_INTERACTION_PTR(IPromiseCatchDelegate);

  ZS_DECLARE_INTERACTION_PTR(ISocketDelegate);

  ZS_DECLARE_CLASS_PTR(Socket);
  ZS_DECLARE_CLASS_PTR(String);

  ZS_DECLARE_INTERACTION_PTR(ISettings);
  ZS_DECLARE_INTERACTION_PTR(ISettingsDelegate);
  ZS_DECLARE_INTERACTION_PTR(ISettingsApplyDefaultsDelegate);
  ZS_DECLARE_INTERACTION_PTR(ITimer);
  ZS_DECLARE_INTERACTION_PTR(ITimerDelegate);
  ZS_DECLARE_INTERACTION_PTR(IWakeDelegate);

  ZS_DECLARE_STRUCT_PTR(Any);

  struct Any
  {
    virtual ~Any() noexcept {}
  };

  template <typename type>
  struct AnyHolder : public Any
  {
    typedef type UseType;
    UseType value_;
  };

  struct Noop
  {
    Noop(bool noop = false) noexcept : mNoop(noop) {}
    Noop(const Noop &noop) noexcept : mNoop(noop.mNoop) {}

    bool isNoop() const noexcept {return mNoop;}

    bool mNoop;
  };

  class AutoInitializedPUID
  {
  public:
    AutoInitializedPUID() noexcept;

    operator PUID() const noexcept {return mValue;}

    void reset(PUID value) noexcept {mValue = value;}

  private:
    PUID mValue;
  };

  typedef AutoInitializedPUID AutoPUID;

#ifdef ZS__LATER__HAS_STD_OPTIONAL
  using Optional = std::optional;
#else
  template <typename type>
  class Optional
  {
  public:
    typedef type UseType;
    typedef type value_type;
    
  public:
    Optional() noexcept {}

    Optional(const UseType &value) noexcept :
      mHasValue(true),
      mType(value)
    {}

    Optional(const Optional &op2) noexcept :
      mHasValue(op2.mHasValue),
      mType(op2.mType)
    {}

    Optional &operator=(const Optional &op2) noexcept
    {
      mHasValue = op2.mHasValue;
      mType = op2.mType;
      return *this;
    }

    Optional &operator=(const UseType &op2) noexcept
    {
      mHasValue = true;
      mType = op2;
      return *this;
    }

    bool hasValue() const noexcept {return mHasValue;}
    bool has_value() const noexcept {return mHasValue;}
    UseType &value() noexcept {return mType;}
    const UseType &value() const noexcept {return mType;}
    operator UseType() const noexcept {return mType;}

  public:
    bool mHasValue {false};
    UseType mType {};
  };

#endif //ZS_HAS_STD_OPTIONAL

  namespace Exceptions
  {
    ZS_DECLARE_CLASS_PTR(InvalidArgument);
    ZS_DECLARE_CLASS_PTR(BadState);
    ZS_DECLARE_CLASS_PTR(SyntaxError);
    ZS_DECLARE_CLASS_PTR(ResourceError);
    ZS_DECLARE_CLASS_PTR(UnexpectedError);
    ZS_DECLARE_CLASS_PTR(InvalidUsage);
    ZS_DECLARE_CLASS_PTR(InvalidAssumption);
    ZS_DECLARE_CLASS_PTR(NotImplemented);
  }

  namespace XML
  {
    ZS_DECLARE_CLASS_PTR(Node);
    ZS_DECLARE_CLASS_PTR(Document);
    ZS_DECLARE_CLASS_PTR(Element);
    ZS_DECLARE_CLASS_PTR(Attribute);
    ZS_DECLARE_CLASS_PTR(Text);
    ZS_DECLARE_CLASS_PTR(Comment);
    ZS_DECLARE_CLASS_PTR(Declaration);
    ZS_DECLARE_CLASS_PTR(Unknown);
    ZS_DECLARE_CLASS_PTR(Parser);
    ZS_DECLARE_CLASS_PTR(Generator);

    class ParserPos;
    class ParserWarning;
    class WalkSink;
  }

  namespace JSON = zsLib::XML;

} // namespace zsLib
