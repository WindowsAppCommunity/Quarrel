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
#include <zsLib/Proxy.h>

namespace zsLib
{
  template <typename XINTERFACE, typename XDATATYPE>
  class TearAway
  {
  public:

    ZS_DECLARE_TYPEDEF_PTR(XINTERFACE, TearAwayInterface);
    ZS_DECLARE_TYPEDEF_PTR(XDATATYPE, TearAwayData);
    ZS_DECLARE_TYPEDEF_PTR(TearAway, TearAwayType);

  public:
    //------------------------------------------------------------------------
    // PURPOSE: Create an object tear away wrapper to wrap an original
    //          interface with optional additional associated data
    static TearAwayInterfacePtr create(
                                       TearAwayInterfacePtr original,
                                       TearAwayDataPtr data = TearAwayDataPtr()
                                       )                                                                                                                      {return original->tear_away_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Get the tear away interface associated to the tear away
    //          instance. Functionally identical to static original(...)
    //          method.
    TearAwayInterfacePtr getDelegate() const                                                                                                                  { return TearAwayInterfacePtr()->tear_away_implementation_for_this_interface_is_not_defined(); }

    //------------------------------------------------------------------------
    // PURPOSE: Set the tear away interface associated to the tear away
    //          instance.
    void setDelegate(TearAwayInterfacePtr original)                                                                                                           { original->tear_away_implementation_for_this_interface_is_not_defined(); }

    //------------------------------------------------------------------------
    // PURPOSE: Gets the data associated with the tear away instance.
    TearAwayDataPtr getData() const                                                                                                                           { TearAwayDataPtr()->tear_away_implementation_for_this_interface_is_not_defined(); }

    //------------------------------------------------------------------------
    // PURPOSE: Returns true if the interface passed in is a tear away
    static bool isTearAway(TearAwayInterfacePtr tearAway)                                                                                                     {return tearAway->tear_away_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns the original inteface from a tear away (or if the
    //          object is not a tear away just returns the object passed in
    //          since it is the original).
    static TearAwayInterfacePtr original(TearAwayInterfacePtr tearAway)                                                                                       {return tearAway->tear_away_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns pointer to data structure if the interface passed
    //          in is a tear away otherwise returns TearAwayDataPtr()
    static TearAwayDataPtr data(TearAwayInterfacePtr tearAway)                                                                                                {return tearAway->tear_away_implementation_for_this_interface_is_not_defined();}

    //------------------------------------------------------------------------
    // PURPOSE: Returns the tear away inteface from an interface type (or if
    //          the object is not a tear away then returns a nullptr).
    static TearAwayTypePtr tearAway(TearAwayInterfacePtr tearAway)                                                                                            { return tearAway->tear_away_implementation_for_this_interface_is_not_defined(); }
  };
}

#include <zsLib/internal/zsLib_TearAway.h>

#if 0
#define ZS_DECLARE_TEMPLATE_GENERATE_IMPLEMENTATION 1
#endif //0

#define ZS_DECLARE_INTERACTION_TEAR_AWAY(xInteractionName, xDataType)                                                                                         ZS_INTERNAL_DECLARE_INTERACTION_TEAR_AWAY(xInteractionName, xDataType)
#define ZS_DECLARE_TYPEDEF_TEAR_AWAY(xOriginalType, xNewTypeName, xDataType)                                                                                  ZS_INTERNAL_DECLARE_TYPEDEF_TEAR_AWAY(xOriginalType, xNewTypeName, xDataType)
#define ZS_DECLARE_USING_TEAR_AWAY(xNamespace, xExistingType)                                                                                                 ZS_INTERNAL_DECLARE_USING_TEAR_AWAY(xNamespace, xExistingType)

#define ZS_DECLARE_TEAR_AWAY_IMPLEMENT(xInterface, xDataType)                                                                                                 ZS_INTERNAL_DECLARE_TEAR_AWAY_IMPLEMENT(xInterface, xDataType)

#define ZS_DECLARE_TEAR_AWAY_BEGIN(xInterface, xDataType)                                                                                                     ZS_INTERNAL_DECLARE_TEAR_AWAY_BEGIN(xInterface, xDataType)
#define ZS_DECLARE_TEAR_AWAY_END()                                                                                                                            ZS_INTERNAL_DECLARE_TEAR_AWAY_END()

#define ZS_DECLARE_TEAR_AWAY_TYPEDEF(xOriginalType, xTypeAlias)                                                                                               ZS_INTERNAL_DECLARE_TEAR_AWAY_TYPEDEF(xOriginalType, xTypeAlias)

#define ZS_DECLARE_TEAR_AWAY_METHOD(xMethod, ...)                                                                                                             ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW_DECLARE, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_SYNC(xMethod, ...)                                                                                                        ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_CONST(xMethod, ...)                                                                                                       ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_RETURN(xMethod, xReturnType, ...)                                                                                         ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_RETURN_CONST(xMethod, xReturnType, ...)                                                                                   ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_NO_THROW, xMethod, __VA_ARGS__)

#define ZS_DECLARE_TEAR_AWAY_METHOD_THROWS(xMethod, ...)                                                                                                      ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_SYNC_THROWS(xMethod, ...)                                                                                                 ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_CONST_THROWS(xMethod, ...)                                                                                                ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROYY_NO_RETURN_KEYWORD, void, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_RETURN_THROWS(xMethod, xReturnType, ...)                                                                                  ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_NO_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
#define ZS_DECLARE_TEAR_AWAY_METHOD_RETURN_CONST_THROWS(xMethod, xReturnType, ...)                                                                            ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(ZS_INTERNAL_DECLARE_PROXY_METHOD_SYNC, ZS_INTERNAL_PROXY_RETURN_KEYWORD, xReturnType, ZS_INTERNAL_PROXY_NO_IGNORE_CHECK, ZS_INTERNAL_PROXY_CONST, ZS_INTERNAL_PROXY_THROW, xMethod, __VA_ARGS__)
