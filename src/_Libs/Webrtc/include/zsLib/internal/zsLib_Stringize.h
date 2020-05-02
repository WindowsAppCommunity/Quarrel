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

#ifndef ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58
#define ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58

#include <zsLib/helpers.h>
#include <zsLib/types.h>
#include <zsLib/String.h>

#ifdef _WIN32
#include <objbase.h>
#endif //_WIN32

namespace zsLib
{
  namespace internal
  {
    String convert(ULONGLONG value, size_t base) noexcept;

    String timeToString(const Time &value) noexcept;

    String durationToString(
                            const Seconds &secPart,
                            std::intmax_t fractionalPart,
                            std::intmax_t den
                            ) noexcept;

    //template <typename duration_type>
    //constexpr bool biggerSize(decltype(duration_type::period::den) den, decltype(duration_type::period::num) num) noexcept { return den > num; }

    template <typename duration_type>
    String durationToString(const duration_type &value) noexcept
    {
      Seconds seconds = toSeconds(value);
      duration_type remainder = value - std::chrono::duration_cast<duration_type>(seconds);

      const auto den = duration_type::period::den;
      const auto num = duration_type::period::num;

      auto biggerConstExpr = [&] { return den > num; };

      if (biggerConstExpr()) {
        return durationToString(seconds, remainder.count(), duration_type::period::den);
      }

      return std::to_string(value.count());
    }

    void trimTrailingZeros(std::string &value) noexcept;
  }
} // namespace zsLib

#endif //ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58

#ifdef ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58_SECOND_INCLUDE
#undef ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58_SECOND_INCLUDE
namespace zsLib
{
  template<typename t_type>
  inline Stringize<t_type>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);

    return internal::convert((ULONGLONG)mValue, mBase);
  }

  template<>
  inline Stringize<bool>::operator String() const noexcept
  {
    return mValue ? String("true") : String("false");
  }

  template<>
  inline Stringize<const char *>::operator String() const noexcept
  {
    return mValue ? String(mValue) : String();
  }

  template<>
  inline Stringize<CHAR>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string((int)mValue);
    return internal::convert((ULONGLONG)((UCHAR)mValue), mBase);
  }

  template<>
  inline Stringize<UCHAR>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string((UINT)mValue);
    return internal::convert((ULONGLONG)((UINT)mValue), mBase);
  }

  template<>
  inline Stringize<SHORT>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string((INT)mValue);
    return internal::convert((ULONGLONG)((USHORT)mValue), mBase);
  }

  template<>
  inline Stringize<USHORT>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string((UINT)mValue);
    return internal::convert((ULONGLONG)mValue, mBase);
  }

  template<>
  inline Stringize<INT>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert((ULONGLONG)((UINT)mValue), mBase);
  }

  template<>
  inline Stringize<UINT>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert((ULONGLONG)mValue, mBase);
  }

  template<>
  inline Stringize<LONG>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert((ULONGLONG)((ULONG)mValue), mBase);
  }

  template<>
  inline Stringize<ULONG>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert((ULONGLONG)mValue, mBase);
  }

  template<>
  inline Stringize<LONGLONG>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert((ULONGLONG)mValue, mBase);
  }

  template<>
  inline Stringize<ULONGLONG>::operator String() const noexcept
  {
    if (10 == mBase)
      return std::to_string(mValue);
    return internal::convert(mValue, mBase);
  }

  template<>
  inline Stringize<float>::operator String() const noexcept
  {
    std::string result = std::to_string(mValue);
    internal::trimTrailingZeros(result);
    return result;
  }

  template<>
  inline Stringize<double>::operator String() const noexcept
  {
    std::string result = std::to_string(mValue);
    internal::trimTrailingZeros(result);
    return result;
  }

  template<>
  inline Stringize<long double>::operator String() const noexcept
  {
    std::string result = std::to_string(mValue);
    internal::trimTrailingZeros(result);
    return result;
  }

  template<>
  inline Stringize<UUID>::operator String() const noexcept
  {
#ifndef _WIN32
	char buffer[(sizeof(mValue)*3)+3];  // allow for 00-FF and '{', '}', '-' and nul at end
	uuid_unparse_lower(mValue.mUUID, buffer);
	return String((CSTR)buffer);
#else
	wchar_t buffer[(sizeof(mValue)*3)+3];
	int result = StringFromGUID2(mValue.mUUID, &(buffer[0]), sizeof(buffer));
  ZS_MAYBE_USED(result);
  ZS_ASSERT(0 != result);
  String output(buffer);
  output.trimLeft("{");
  output.trimRight("}");
  output.toLower();
	return output;
#endif //_WIN32
  }

  template<>
  inline Stringize<Time>::operator String() const noexcept
  {
    return internal::timeToString(mValue);
  }

  template<>
  inline Stringize<Hours>::operator String() const noexcept
  {
    return internal::durationToString<Hours>(mValue);
  }

  template<>
  inline Stringize<Minutes>::operator String() const noexcept
  {
    return internal::durationToString<Minutes>(mValue);
  }

  template<>
  inline Stringize<Seconds>::operator String() const noexcept
  {
    return internal::durationToString<Seconds>(mValue);
  }

  template<>
  inline Stringize<Milliseconds>::operator String() const noexcept
  {
    return internal::durationToString<Milliseconds>(mValue);
  }

  template<>
  inline Stringize<Microseconds>::operator String() const noexcept
  {
    return internal::durationToString<Microseconds>(mValue);
  }

  template<>
  inline Stringize<Nanoseconds>::operator String() const noexcept
  {
    return internal::durationToString<Nanoseconds>(mValue);
  }

}
#else
#define ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58_SECOND_INCLUDE
#endif //ZSLIB_INTERNAL_STRINGIZE_H_0c235f6defcccb275d602da44da60e58_SECOND_INCLUDE
