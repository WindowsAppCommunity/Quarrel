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

#ifndef ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f
#define ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f

#include <zsLib/String.h>
#include <zsLib/types.h>
#include <zsLib/Exception.h>

namespace zsLib
{
  namespace internal
  {
    bool convert(const String &input, LONGLONG &outResult, size_t size, bool ignoreWhiteSpace, size_t base);
    bool convert(const String &input, ULONGLONG &outResult, size_t size, bool ignoreWhiteSpace, size_t base);
    bool convert(const String &input, bool &outResult, bool ignoreWhiteSpace);
    bool convert(const String &input, float &outResult, bool ignoreWhiteSpace) noexcept;
    bool convert(const String &input, double &outResult, bool ignoreWhiteSpace) noexcept;
    bool convert(const String &input, UUID &outResult, bool ignoreWhiteSpace) noexcept;
    bool convert(const String &input, Time &outResult, bool ignoreWhiteSpace) noexcept;
  }
}

#endif //ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f

#ifdef ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f_SECOND_INCLUDE

#ifdef _WIN32
#pragma warning(push)
#pragma warning(disable:4290)
#endif // _WIN32

namespace zsLib
{
  template<>
  void Numeric<bool>::get(bool &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<float>::get(float &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<double>::get(double &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<UUID>::get(UUID &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Time>::get(Time &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Hours>::get(Hours &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Seconds>::get(Seconds &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Milliseconds>::get(Milliseconds &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Microseconds>::get(Microseconds &outValue) const noexcept(false);  // throws ValueOutOfRange

  template<>
  void Numeric<Nanoseconds>::get(Nanoseconds &outValue) const noexcept(false);  // throws ValueOutOfRange
}

#ifdef _WIN32
#pragma warning(pop)
#endif // _WIN32

#else
#define ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f_SECOND_INCLUDE
#endif //ZSLIB_INTERNAL_NUMERIC_H_f1973c558a596895b01b96e4a76de63f_SECOND_INCLUDE
