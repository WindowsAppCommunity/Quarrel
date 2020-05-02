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

#ifndef ZSLIB_INTERNAL_TEAR_AWAY_H_b93ee5a7a6bee3a329b1e47d719ceccd154e44ec
#define ZSLIB_INTERNAL_TEAR_AWAY_H_b93ee5a7a6bee3a329b1e47d719ceccd154e44ec

#include <zsLib/internal/zsLib_Proxy.h>

namespace zsLib
{
  template <typename XINTERFACE, typename XDATATYPE>
  class TearAway;

  namespace internal
  {
    template <typename XINTERFACE, typename XDATATYPE>
    class TearAway : public XINTERFACE
    {
    public:
      ZS_DECLARE_TYPEDEF_PTR(XINTERFACE, TearAwayInterface);
      ZS_DECLARE_TYPEDEF_PTR(XDATATYPE, TearAwayData);
      ZS_DECLARE_TYPEDEF_PTR(TearAway, TearAwayType);

    public:
      TearAway(
               TearAwayInterfacePtr original,
               TearAwayDataPtr data = TearAwayDataPtr()
               ) :
        mOriginal(original),
        mData(data)
      {}

      virtual ~TearAway() {}

      void setDelegate(TearAwayInterfacePtr original) {mOriginal = original;}
      TearAwayInterfacePtr getDelegate() const {return mOriginal;}

      bool ignoreMethodCall() const {return false;}

      TearAwayDataPtr getData() const {return mData;}

    protected:
      TearAwayInterfacePtr mOriginal;
      TearAwayDataPtr mData;
    };
  }
}

#define ZS_INTERNAL_DECLARE_INTERACTION_TEAR_AWAY(xInteractionName, xDataType)                                \
  interaction xInteractionName;                                                                               \
  typedef std::shared_ptr<xInteractionName> xInteractionName##Ptr;                                            \
  typedef std::weak_ptr<xInteractionName> xInteractionName##WeakPtr;                                          \
  typedef zsLib::TearAway<xInteractionName, xDataType> xInteractionName##TearAway;

#define ZS_INTERNAL_DECLARE_TYPEDEF_TEAR_AWAY(xOriginalType, xNewTypeName, xDataType)                         \
  typedef xOriginalType xNewTypeName;                                                                         \
  typedef std::shared_ptr<xNewTypeName> xNewTypeName##Ptr;                                                    \
  typedef std::weak_ptr<xNewTypeName> xNewTypeName##WeakPtr;                                                  \
  typedef zsLib::TearAway<xNewTypeName, xDataType> xNewTypeName##TearAway;

#define ZS_INTERNAL_DECLARE_USING_TEAR_AWAY(xNamespace, xExistingType)                                        \
  using xNamespace::xExistingType;                                                                            \
  using xNamespace::xExistingType##Ptr;                                                                       \
  using xNamespace::xExistingType##WeakPtr;                                                                   \
  using xNamespace::xExistingType##TearAway;



#define ZS_INTERNAL_DECLARE_TEAR_AWAY_IMPLEMENT(xInterface, xDataType)                                        \
  namespace zsLib                                                                                             \
  {                                                                                                           \
    void declareTearAwayInterface(const xInterface &)                                                         \
    {                                                                                                         \
      typedef std::shared_ptr<xInterface> TearAwayInterfacePtr;                                               \
      zsLib::TearAway<xInterface, xDataType>::create(TearAwayInterfacePtr());                                 \
      zsLib::TearAway<xInterface, xDataType>::isTearAway(TearAwayInterfacePtr());                             \
      zsLib::TearAway<xInterface, xDataType>::original(TearAwayInterfacePtr());                               \
      zsLib::TearAway<xInterface, xDataType>::data(TearAwayInterfacePtr());                                   \
      zsLib::TearAway<xInterface, xDataType>::tearAway(TearAwayInterfacePtr());                               \
    }                                                                                                         \
  }


#ifndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION

#define ZS_INTERNAL_DECLARE_TEAR_AWAY_BEGIN(xInterface, xDataType)                                            \
namespace zsLib                                                                                               \
{                                                                                                             \
  template<>                                                                                                  \
  class TearAway<xInterface, xDataType> : public internal::TearAway<xInterface, xDataType>                    \
  {                                                                                                           \
  public:                                                                                                     \
    ZS_DECLARE_TYPEDEF_PTR(xInterface, TearAwayInterface)                                                     \
    typedef TearAway<xInterface, xDataType> TearAwayType;                                                     \
    ZS_DECLARE_PTR(TearAwayType)                                                                              \
    ZS_DECLARE_TYPEDEF_PTR(xDataType, TearAwayData)                                                           \
                                                                                                              \
  public:                                                                                                     \
    TearAway(TearAwayInterfacePtr original, TearAwayDataPtr data = TearAwayDataPtr());                        \
                                                                                                              \
  public:                                                                                                     \
    static TearAwayInterfacePtr create(                                                                       \
                                      TearAwayInterfacePtr original,                                          \
                                      TearAwayDataPtr data = TearAwayDataPtr()                                \
                                      );                                                                      \
                                                                                                              \
    static bool isTearAway(TearAwayInterfacePtr tearAway);                                                    \
                                                                                                              \
    static TearAwayInterfacePtr original(TearAwayInterfacePtr tearAway);                                      \
                                                                                                              \
    static TearAwayDataPtr data(TearAwayInterfacePtr tearAway);                                               \
                                                                                                              \
    static TearAwayTypePtr tearAway(TearAwayInterfacePtr tearAwayInterface);                                  \

#define ZS_INTERNAL_DECLARE_TEAR_AWAY_END()                                                                   \
  };                                                                                                          \
}

#else //ndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION

#define ZS_INTERNAL_DECLARE_TEAR_AWAY_BEGIN(xInterface, xDataType)                                            \
namespace zsLib                                                                                               \
{                                                                                                             \
  template<>                                                                                                  \
  class TearAway<xInterface, xDataType> : public internal::TearAway<xInterface, xDataType>                    \
  {                                                                                                           \
  public:                                                                                                     \
    ZS_DECLARE_TYPEDEF_PTR(xInterface, TearAwayInterface)                                                     \
    typedef TearAway<xInterface, xDataType> TearAwayType;                                                     \
    ZS_DECLARE_PTR(TearAwayType)                                                                              \
    ZS_DECLARE_TYPEDEF_PTR(xDataType, TearAwayData)                                                           \
                                                                                                              \
  public:                                                                                                     \
    TearAway(TearAwayInterfacePtr original, TearAwayDataPtr data = TearAwayDataPtr()) : internal::TearAway<xInterface, xDataType>(original, data) {} \
                                                                                                              \
  public:                                                                                                     \
    static TearAwayInterfacePtr create(                                                                       \
                                      TearAwayInterfacePtr original,                                          \
                                      TearAwayDataPtr data = TearAwayDataPtr()                                \
                                      )                                                                       \
    {                                                                                                         \
      if (!original)                                                                                          \
        return original;                                                                                      \
                                                                                                              \
      return make_shared<TearAwayType>(original, data);                                                       \
    }                                                                                                         \
                                                                                                              \
    static bool isTearAway(TearAwayInterfacePtr tearAway)                                                     \
    {                                                                                                         \
      if (!tearAway)                                                                                          \
        return false;                                                                                         \
                                                                                                              \
      TearAwayType *wrapper = dynamic_cast<TearAwayType *>(tearAway.get());                                   \
      return (wrapper ? true : false);                                                                        \
    }                                                                                                         \
                                                                                                              \
    static TearAwayInterfacePtr original(TearAwayInterfacePtr tearAway)                                       \
    {                                                                                                         \
      if (!tearAway)                                                                                          \
        return tearAway;                                                                                      \
                                                                                                              \
      TearAwayType *wrapper = dynamic_cast<TearAwayType *>(tearAway.get());                                   \
      if (wrapper) {                                                                                          \
        return wrapper->getDelegate();                                                                        \
      }                                                                                                       \
      return tearAway;                                                                                        \
    }                                                                                                         \
                                                                                                              \
    static TearAwayDataPtr data(TearAwayInterfacePtr tearAway)                                                \
    {                                                                                                         \
      if (!tearAway)                                                                                          \
        return TearAwayDataPtr();                                                                             \
                                                                                                              \
      TearAwayType *wrapper = dynamic_cast<TearAwayType *>(tearAway.get());                                   \
      if (wrapper) {                                                                                          \
        return wrapper->getData();                                                                            \
      }                                                                                                       \
      return TearAwayDataPtr();                                                                               \
    }                                                                                                         \
                                                                                                              \
    static TearAwayTypePtr tearAway(TearAwayInterfacePtr tearAwayInterface)                                   \
    {                                                                                                         \
      return ZS_DYNAMIC_PTR_CAST(TearAwayType, tearAwayInterface);                                            \
    }

#define ZS_INTERNAL_DECLARE_TEAR_AWAY_END()                                                                   \
  };                                                                                                          \
}

#endif //ndef ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION

#define ZS_INTERNAL_DECLARE_TEAR_AWAY_TYPEDEF(xOriginalType, xTypeAlias)                                      \
    typedef xOriginalType xTypeAlias;


#endif //ZSLIB_INTERNAL_TEAR_AWAY_H_b93ee5a7a6bee3a329b1e47d719ceccd154e44ec
