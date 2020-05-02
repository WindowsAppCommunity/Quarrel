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

#include <zsLib/internal/zsLib_Numeric.h>

#ifdef _WIN32
#pragma warning(push)
#pragma warning(disable:4290)
#endif // _WIN32

namespace zsLib
{
  template<typename t_type>
  class Numeric
  {
  public:
    ZS_DECLARE_CUSTOM_EXCEPTION(ValueOutOfRange)

    typedef t_type NumericType;

  public:
    Numeric(bool ignoreWhitespace = true, size_t base = 10) noexcept : mIngoreWhitespace(ignoreWhitespace), mBase(base)                                          {}
    Numeric(CSTR value, bool ignoreWhitespace = true, size_t base = 10) noexcept : mData(value), mIngoreWhitespace(ignoreWhitespace), mBase(base)                {}
    Numeric(const String &value, bool ignoreWhitespace = true, size_t base = 10) noexcept : mData(value), mIngoreWhitespace(ignoreWhitespace), mBase(base)       {}
    Numeric(const Numeric &value, bool ignoreWhitespace = true, size_t base = 10) noexcept : mData(value.mData),mIngoreWhitespace(ignoreWhitespace), mBase(base) {}

    Numeric &operator=(const String &value) noexcept    {mData = value; return *this;}
    Numeric &operator=(const Numeric &value) noexcept   {mData = value.mData; return *this;}

    void get(t_type &outValue) const noexcept(false);   // throws ValueOutOfRange

    bool ignoreSpace() const noexcept                   {return mIngoreWhitespace;}
    int getBase() const noexcept                        {return mBase;}

    operator t_type() const noexcept(false)             {t_type value; get(value); return value;}

  private:

    String mData;
    bool mIngoreWhitespace;
    size_t mBase;
  };
}

#ifdef _WIN32
#pragma warning(pop)
#endif // _WIN32

#include <zsLib/internal/zsLib_Numeric.h>
