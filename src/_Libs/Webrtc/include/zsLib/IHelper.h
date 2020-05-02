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

#include <zsLib/types.h>
#include <zsLib/Log.h>

#ifdef __has_include
#if __has_include(<winrt/windows.ui.core.h>)
#include <winrt/windows.ui.core.h>
#endif //__has_include(<winrt/windows.ui.core.h>)
#endif //__has_include

namespace zsLib
{
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // IHelper
  //

  interaction IHelper
  {
    ZS_DECLARE_TYPEDEF_PTR(XML::Element, Element);
    ZS_DECLARE_TYPEDEF_PTR(XML::Document, Document);
    ZS_DECLARE_TYPEDEF_PTR(XML::Text, Text);

    typedef size_t Index;
    typedef std::map<Index, String> SplitMap;
    typedef std::list<String> StringList;
    typedef std::set<IPAddress> IPAddressSet;

    static void setup() noexcept;
#ifdef CPPWINRT_VERSION
    static void setup(winrt::Windows::UI::Core::CoreDispatcher dispatcher) noexcept;
#endif //CPPWINRT_VERSION

    static RecursiveLockPtr getGlobalLock() noexcept;

    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, bool &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, CHAR &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, UCHAR &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, SHORT &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, USHORT &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, LONG &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, ULONG &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, LONGLONG &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, ULONGLONG &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, INT &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, UINT &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, FLOAT &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, DOUBLE &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, String &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Time &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Milliseconds &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Microseconds &outValue) noexcept;

    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<bool> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<CHAR> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<UCHAR> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<SHORT> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<USHORT> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<LONG> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<ULONG> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<LONGLONG> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<ULONGLONG> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<INT> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<UINT> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<FLOAT> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<DOUBLE> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<String> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<Time> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<Milliseconds> &outValue) noexcept;
    static void getElementValue(ElementPtr elem, const char *logObjectName, const char *subElementName, Optional<Microseconds> &outValue) noexcept;

    static void adoptElementValue(ElementPtr elem, const char *subElementName, bool value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, CHAR value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, UCHAR value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, SHORT value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, USHORT value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, LONG value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, ULONG value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, LONGLONG value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, ULONGLONG value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, INT value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, UINT value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, FLOAT value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, DOUBLE value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const String &value, bool adoptEmptyValue) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Time &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Milliseconds &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Microseconds &value) noexcept;

    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<bool> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<CHAR> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<UCHAR> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<SHORT> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<USHORT> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<LONG> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<ULONG> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<LONGLONG> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<ULONGLONG> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<INT> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<UINT> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<FLOAT> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<DOUBLE> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<String> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<Time> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<Milliseconds> &value) noexcept;
    static void adoptElementValue(ElementPtr elem, const char *subElementName, const Optional<Microseconds> &value) noexcept;

    static void debugAppend(ElementPtr &parentEl, const char *name, const char *value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const String &value) noexcept;
    static void debugAppendNumber(ElementPtr &parentEl, const char *name, const String &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, bool value, bool ignoreIfFalse = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, CHAR value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, UCHAR value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, SHORT value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, USHORT value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, INT value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, UINT value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, LONG value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, ULONG value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, LONGLONG value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, ULONGLONG value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, FLOAT value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, DOUBLE value, bool ignoreIfZero = true) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Time &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Hours &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Minutes &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Seconds &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Milliseconds &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Microseconds &value) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, const Nanoseconds &value) noexcept;

    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<String> &value) noexcept       {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<bool> &value) noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<CHAR> &value) noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<UCHAR> &value)noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<SHORT> &value) noexcept        {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<USHORT> &value) noexcept       {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<INT> &value) noexcept          {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<UINT> &value) noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<LONG> &value) noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<ULONG> &value) noexcept        {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<LONGLONG> &value) noexcept     {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<ULONGLONG> &value) noexcept    {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<FLOAT> &value) noexcept        {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<DOUBLE> &value) noexcept       {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value(), false);}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Time> &value) noexcept         {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Hours> &value) noexcept        {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Minutes> &value) noexcept      {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Seconds> &value) noexcept      {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Milliseconds> &value) noexcept {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Microseconds> &value) noexcept {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}
    static void debugAppend(ElementPtr &parentEl, const char *name, const Optional<Nanoseconds> &value) noexcept  {if (!value.hasValue()) return; debugAppend(parentEl, name, value.value());}

    static void debugAppend(ElementPtr &parentEl, const Log::Param &param) noexcept;
    static void debugAppend(ElementPtr &parentEl, const char *name, ElementPtr childEl) noexcept;
    static void debugAppend(ElementPtr &parentEl, ElementPtr childEl) noexcept;

    static String toString(
                           ElementPtr el,
                           bool formatAsJson = true
                           ) noexcept;
    static ElementPtr toJSON(const char *str) noexcept;
    static ElementPtr toXML(const char *str) noexcept;

    static String getAttributeID(ElementPtr el) noexcept;
    static void setAttributeIDWithText(ElementPtr el, const String &value) noexcept;
    static void setAttributeIDWithNumber(ElementPtr el, const String &value) noexcept;

    static String getAttribute(
                                ElementPtr el,
                                const String &attributeName
                                ) noexcept;

    static void setAttributeWithText(
                                      ElementPtr el,
                                      const String &attrName,
                                      const String &value
                                      ) noexcept;

    static void setAttributeWithNumber(
                                        ElementPtr el,
                                        const String &attrName,
                                        const String &value
                                        ) noexcept;

    static ElementPtr createElement(const String &elName) noexcept;

    static ElementPtr createElementWithText(
                                            const String &elName,
                                            const String &textVal
                                            ) noexcept;
    static ElementPtr createElementWithNumber(
                                              const String &elName,
                                              const String &numberAsStringValue
                                              ) noexcept;
    static ElementPtr createElementWithTime(
                                            const String &elName,
                                            Time time
                                            ) noexcept;
    static ElementPtr createElementWithTextAndJSONEncode(
                                                          const String &elName,
                                                          const String &textVal
                                                          ) noexcept;
    static ElementPtr createElementWithTextID(
                                              const String &elName,
                                              const String &idValue
                                              ) noexcept;
    static ElementPtr createElementWithNumberID(
                                                const String &elName,
                                                const String &idValue
                                                ) noexcept;

    static TextPtr createText(const String &textVal) noexcept;

    static String getElementText(ElementPtr el) noexcept;
    static String getElementTextAndDecode(ElementPtr el) noexcept;

    static String timeToString(const Time &value) noexcept;
    static Time stringToTime(const String &str) noexcept;

    static WORD getBE16(const void* memory) noexcept;
    static DWORD getBE32(const void* memory) noexcept;
    static QWORD getBE64(const void* memory) noexcept;
    static void setBE16(void* memory, WORD v) noexcept;
    static void setBE32(void* memory, DWORD v) noexcept;
    static void setBE64(void* memory, QWORD v) noexcept;

    static ElementPtr cloneAsCanonicalJSON(ElementPtr element) noexcept;

    static void split(
                      const String &input,
                      SplitMap &outResult,
                      char splitChar
                      ) noexcept;

    static void split(
                      const String &input,
                      SplitMap &outResult,
                      const char *splitStr
                      ) noexcept;

    static void splitPruneEmpty(
                                SplitMap &ioResult,
                                bool reindex = true
                                ) noexcept;

    static void splitTrim(SplitMap &ioResult) noexcept;

    static String combine(
                          const SplitMap &input,
                          const char *combineStr
                          ) noexcept;
    static String combine(
                          const StringList &input,
                          const char *combineStr
                          ) noexcept;

    static const String &get(
                             const SplitMap &inResult,
                             Index index
                             ) noexcept;

    static String getDebugString(
                                 const BYTE *buffer,
                                 size_t bufferSizeInBytes,
                                 size_t bytesPerGroup = 4,
                                 size_t maxLineLength = 160
                                 ) noexcept;

    static void parseIPs(
                         const String &ipList,
                         IPAddressSet &outIPs,
                         char splitChar = ','
                         ) noexcept;
    static bool containsIP(
                           const IPAddressSet &inMap,
                           const IPAddress &ip,
                           bool emptySetReturns = true
                           ) noexcept;
  };

} // namespace zsLib
