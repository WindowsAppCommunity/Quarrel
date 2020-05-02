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


namespace zsLib
{
  //---------------------------------------------------------------------------
  PUID createPUID() noexcept;
  UUID createUUID() noexcept;

  //---------------------------------------------------------------------------
  void debugSetCurrentThreadName(const char *name) noexcept;

  // see: http://stackoverflow.com/questions/8357240/how-to-automatically-convert-strongly-typed-enum-into-int
  template <typename E>
  constexpr typename std::underlying_type<E>::type to_underlying(E e) noexcept {
    return static_cast<typename std::underlying_type<E>::type>(e);
  }

  //---------------------------------------------------------------------------
  Time now() noexcept;

  template <typename duration_type>
  inline duration_type timeSinceEpoch(Time time) noexcept
  {
    if (Time() == time) return duration_type();
    return std::chrono::duration_cast<duration_type>(time.time_since_epoch());
  }

  template <typename duration_type>
  inline Time timeSinceEpoch(duration_type duration) noexcept
  {
    if (decltype(duration)() == duration) return Time();
    return Time(duration);
  }

  inline Days toDays(const Days &v) noexcept {return v;}
  inline Days toDays(const Hours &v) noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Minutes &v) noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Seconds &v) noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Microseconds &v) noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Nanoseconds &v)noexcept {return std::chrono::duration_cast<Days>(v);}
  inline Days toDays(const Time &v) noexcept { return Days(std::chrono::time_point_cast<Days>(v).time_since_epoch()); }

  inline Hours toHours(const Days &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Hours &v) noexcept {return v;}
  inline Hours toHours(const Minutes &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Seconds &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Microseconds &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Nanoseconds &v) noexcept {return std::chrono::duration_cast<Hours>(v);}
  inline Hours toHours(const Time &v) noexcept { return Hours(std::chrono::time_point_cast<Hours>(v).time_since_epoch()); }

  inline Minutes toMinutes(const Days &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Hours &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Minutes &v) noexcept {return v;}
  inline Minutes toMinutes(const Seconds &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Microseconds &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Nanoseconds &v) noexcept {return std::chrono::duration_cast<Minutes>(v);}
  inline Minutes toMinutes(const Time &v) noexcept { return Minutes(std::chrono::time_point_cast<Minutes>(v).time_since_epoch()); }

  inline Seconds toSeconds(const Days &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Hours &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Minutes &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Seconds &v) noexcept {return v;}
  inline Seconds toSeconds(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Microseconds &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Nanoseconds &v) noexcept {return std::chrono::duration_cast<Seconds>(v);}
  inline Seconds toSeconds(const Time &v) noexcept { return Seconds(std::chrono::time_point_cast<Seconds>(v).time_since_epoch()); }

  inline Milliseconds toMilliseconds(const Days &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Hours &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Minutes &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Seconds &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Milliseconds &v) noexcept {return v;}
  inline Milliseconds toMilliseconds(const Microseconds &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Nanoseconds &v) noexcept {return std::chrono::duration_cast<Milliseconds>(v);}
  inline Milliseconds toMilliseconds(const Time &v) noexcept { return Milliseconds(std::chrono::time_point_cast<Milliseconds>(v).time_since_epoch()); }

  inline Microseconds toMicroseconds(const Days &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Hours &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Minutes &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Seconds &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Microseconds &v) noexcept {return v;}
  inline Microseconds toMicroseconds(const Nanoseconds &v) noexcept {return std::chrono::duration_cast<Microseconds>(v);}
  inline Microseconds toMicroseconds(const Time &v) noexcept { return Microseconds(std::chrono::time_point_cast<Microseconds>(v).time_since_epoch()); }

  inline Nanoseconds toNanoseconds(const Days &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Hours &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Minutes &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Seconds &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Milliseconds &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Microseconds &v) noexcept {return std::chrono::duration_cast<Nanoseconds>(v);}
  inline Nanoseconds toNanoseconds(const Nanoseconds &v) noexcept {return v;}
  inline Nanoseconds toNanoseconds(const Time &v) noexcept { return Nanoseconds(std::chrono::time_point_cast<Nanoseconds>(v).time_since_epoch()); }

} // namespace zsLib
