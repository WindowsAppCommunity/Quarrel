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

#ifndef ZSLIB_INTERNAL_EXCEPTION_H_33206cbc33b745eb5c841eca0b34d550
#define ZSLIB_INTERNAL_EXCEPTION_H_33206cbc33b745eb5c841eca0b34d550

#include <zsLib/types.h>
#include <zsLib/String.h>
#include <zsLib/Log.h>

#define ZS_INTERNAL_DECLARE_EXCEPTION_PROPERTY(xClass, xPropertyType, xPropertyName) \
  xClass &set##xPropertyName(xPropertyType in##xPropertyName) {xPropertyName = in##xPropertyName; return *this;} \
  xPropertyType &get##xPropertyName() const {return xPropertyName;}

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION(xObject) \
  class xObject : public ::zsLib::Exception \
  { \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : Exception(subsystem, params, function, pathName, lineNumber, expression) \
    { \
    } \
  };

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_1(xObject, xType1, xName1) \
  class xObject : public ::zsLib::Exception \
  { \
    xType1 m_##xName1; \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : Exception(subsystem, params, function, pathName, lineNumber, expression), m_##xName1(inValue1) \
    { \
    } \
\
    xType1 xName1() const {return m_##xName1;} \
  };

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_2(xObject, xType1, xName1, xType2, xName2) \
  class xObject : public ::zsLib::Exception \
  { \
    xType1 m_##xName1; \
    xType2 m_##xName2; \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : Exception(subsystem, params, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2) \
    { \
    } \
\
    xType1 xName1() const {return m_##xName1;} \
    xType2 xName2() const {return m_##xName2;} \
  };


#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_3(xObject, xType1, xName1, xType2, xName2, xType3, xName3) \
  class xObject : public ::zsLib::Exception \
  { \
    xType1 m_##xName1; \
    xType2 m_##xName2; \
    xType3 m_##xName3; \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3 \
    ) noexcept : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2), m_##xName3(inValue3) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3 \
    ) : Exception(subsystem, message, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2), m_##xName3(inValue3) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3 \
    ) noexcept : Exception(subsystem, params, function, pathName, lineNumber, expression), m_##xName1(inValue1), m_##xName2(inValue2), m_##xName3(inValue3) \
    { \
    } \
\
    xType1 xName1() const {return m_##xName1;} \
    xType2 xName2() const {return m_##xName2;} \
    xType3 xName3() const {return m_##xName3;} \
  };

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE(xObject, xBase) \
  class xObject : public xBase \
  { \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression = NULL \
    ) noexcept : xBase(subsystem, params, function, pathName, lineNumber, expression) \
    { \
    } \
  };

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(xObject, xBase, xType1) \
  class xObject : public xBase \
  { \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1 \
    ) noexcept : xBase(subsystem, params, function, pathName, lineNumber, expression, inValue1) \
    { \
    } \
  };

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_2(xObject, xBase, xType1, xType2) \
  class xObject : public xBase \
  { \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1, inValue2) \
  { \
  } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1, inValue2) \
  { \
  } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2 \
    ) noexcept : xBase(subsystem, params, function, pathName, lineNumber, expression, inValue1, inValue2) \
  { \
  } \
};

#define ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_3(xObject, xBase, xType1, xType2, xType3) \
  class xObject : public xBase \
  { \
  public: \
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      ::zsLib::CSTR message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3, \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1, inValue2, inValue3) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::String &message, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3, \
    ) noexcept : xBase(subsystem, message, function, pathName, lineNumber, expression, inValue1, inValue2, inValue3) \
    { \
    } \
\
    xObject( \
      const ::zsLib::Subsystem &subsystem, \
      const ::zsLib::Log::Params &params, \
      ::zsLib::CSTR function, \
      ::zsLib::CSTR pathName, \
      ::zsLib::ULONG lineNumber, \
      ::zsLib::CSTR expression, \
      xType1 inValue1, \
      xType2 inValue2, \
      xType3 inValue3, \
    ) noexcept : xBase(subsystem, params, function, pathName, lineNumber, expression, inValue1, inValue2, inValue3) \
    { \
    } \
  };

#endif //ZSLIB_INTERNAL_EXCEPTION_H_33206cbc33b745eb5c841eca0b34d550

