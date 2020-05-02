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

#ifndef ZSLIB_INTERNAL_PROXYPACK_H_3d1989252cf7499ccb86cef6b9c76857
#define ZSLIB_INTERNAL_PROXYPACK_H_3d1989252cf7499ccb86cef6b9c76857

#include <zsLib/types.h>
#include <zsLib/String.h>

namespace zsLib
{
  namespace internal
  {
    template <typename TYPE>
    inline void ProxyPack(TYPE &member, TYPE &value) noexcept
    {
      member = value;
    }

    template <typename TYPE>
    inline void ProxyClean(ZS_MAYBE_USED() TYPE &member) noexcept
    {
      ZS_MAYBE_USED(member);
    }

    template <>
    inline void ProxyPack<CSTR>(CSTR &member, CSTR &value) noexcept
    {
      if (value) {
        size_t length = strlen(value);
        STR temp = new char[length+1];
        memcpy(temp, value, sizeof(char)*(length+1));
        member = (CSTR)temp;
      } else
        member = NULL;
    }

    template <>
    inline void ProxyClean<CSTR>(CSTR &member) noexcept
    {
      delete [] ((char *)member);
      member = NULL;
    }

    template <>
    inline void ProxyPack<CWSTR>(CWSTR &member, CWSTR &value) noexcept
    {
      if (value) {
        size_t length = wcslen(value);
        WSTR temp = new WCHAR[length+1];
        memcpy(temp, value, sizeof(WCHAR)*(length+1));
        member = (CWSTR)temp;
      } else
        member = NULL;
    }

    template <>
    inline void ProxyClean<CWSTR>(CWSTR &member) noexcept
    {
      delete [] ((WCHAR *)member);
      member = NULL;
    }

    template <>
    inline void ProxyPack<std::string>(std::string &member, std::string &value) noexcept
    {
      member = value.c_str();
    }

    template <>
    inline void ProxyClean<std::string>(std::string &member) noexcept
    {
      member.clear();
    }

    template <>
    inline void ProxyPack<std::wstring>(std::wstring &member, std::wstring &value) noexcept
    {
      member = value.c_str();
    }

    template <>
    inline void ProxyClean<std::wstring>(std::wstring &member) noexcept
    {
      member.clear();
    }

    template <>
    inline void ProxyPack<String>(String &member, String &value) noexcept
    {
      member = value.c_str();
    }

    template <>
    inline void ProxyClean<String>(String &member) noexcept
    {
      member.clear();
    }

  }
}

#endif //ZSLIB_INTERNAL_PROXYPACK_H_3d1989252cf7499ccb86cef6b9c76857
