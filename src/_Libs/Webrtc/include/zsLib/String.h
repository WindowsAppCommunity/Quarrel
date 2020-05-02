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

#include <string>

#include <zsLib/types.h>

namespace zsLib
{
  class String : public std::string {

  public:
    String() noexcept;
    String(const String &) noexcept;
    String(String &&) noexcept;

    String(CSTR) noexcept;
    String(CWSTR) noexcept;
    String(const std::string &) noexcept;
    String(CSTR, size_t) noexcept;
    String(CWSTR, size_t) noexcept;
    explicit String(const std::wstring &) noexcept;

    static String copyFrom(CSTR, size_t maxCharacters) noexcept;
    static String copyFromUnicodeSafe(CSTR, size_t maxCharacters) noexcept;

    std::wstring wstring() const noexcept;

    bool isEmpty() const noexcept;
    bool hasData() const noexcept;
    size_t getLength() const noexcept;

    operator CSTR() const noexcept;

    String &operator=(const std::string &) noexcept;
    String &operator=(const std::wstring &) noexcept;
    String &operator=(const String &) noexcept;
    String &operator=(String &&) noexcept = default;
    String &operator=(CSTR) noexcept;
    String &operator=(CWSTR) noexcept;

    int compareNoCase(CSTR) const noexcept;
    int compareNoCase(const String &) const noexcept;

    void toLower() noexcept;
    void toUpper() noexcept;

    void trim(CSTR chars = " \t\r\n\v\f") noexcept;
    void trimLeft(CSTR chars = " \t\r\n\v\f") noexcept;
    void trimRight(CSTR chars = " \t\r\n\v\f") noexcept;

    size_t lengthUnicodeSafe() const noexcept;
    String substrUnicodeSafe(size_t pos = 0, size_t n = std::string::npos ) const noexcept;
    WCHAR atUnicodeSafe(size_t pos) const noexcept(false);

    void replaceAll(CSTR findStr, CSTR replaceStr, size_t totalOccurances = std::string::npos) noexcept;
  };
}
