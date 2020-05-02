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

#include <zsLib/Exception.h>

#ifdef _WIN32
#pragma warning(push)
#pragma warning(disable:4290)
#endif //_WIN32

namespace zsLib
{
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //
  // ISettings
  //

  interaction ISettings
  {
    typedef zsLib::Exceptions::InvalidUsage InvalidUsage;

    ISettings() noexcept = default;
    ISettings(const ISettings &) noexcept = delete;
    ISettings &operator=(const ISettings &) = delete;

    static void setup(ISettingsDelegatePtr delegate) noexcept;

    static String getString(const char *key) noexcept;
    static LONG getInt(const char *key) noexcept;
    static ULONG getUInt(const char *key) noexcept;
    static bool getBool(const char *key) noexcept;
    static float getFloat(const char *key) noexcept;
    static double getDouble(const char *key) noexcept;

    static void setString(
                          const char *key,
                          const char *value
                          ) noexcept;
    static void setInt(
                        const char *key,
                        LONG value
                        ) noexcept;
    static void setUInt(
                        const char *key,
                        ULONG value
                        ) noexcept;
    static void setBool(
                        const char *key,
                        bool value
                        ) noexcept;
    static void setFloat(
                          const char *key,
                          float value
                          ) noexcept;
    static void setDouble(
                          const char *key,
                          double value
                          ) noexcept;

    static void clear(const char *key) noexcept;

    static bool apply(const char *jsonSettings) noexcept;

    static void applyDefaults() noexcept;
    static void installDefaults(ISettingsApplyDefaultsDelegatePtr defaultsDelegate) noexcept;
    static void removeDefaults(ISettingsApplyDefaultsDelegate &defaultsDelegate) noexcept;

    static void clearAll() noexcept;

    static void verifySettingExists(const char *key) noexcept(false); // throws InvalidUsage

    static void verifyRequiredSettings() noexcept(false); // throws InvalidUsage

    virtual ~ISettings() noexcept {} // to make settings polymorphic
  };

  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //
  // ISettingsDelegate
  //

  interaction ISettingsDelegate
  {
    // WARNING: These methods are called synchronously from any thread
    //          and must NOT block on any kind of lock that might be
    //          blocked calling inside to the SDK (directly or indirectly).

    virtual String getString(const char *key) const noexcept = 0;
    virtual LONG getInt(const char *key) const noexcept = 0;
    virtual ULONG getUInt(const char *key) const noexcept = 0;
    virtual bool getBool(const char *key) const noexcept = 0;
    virtual float getFloat(const char *key) const noexcept = 0;
    virtual double getDouble(const char *key) const noexcept = 0;

    virtual void setString(
                            const char *key,
                            const char *value
                            ) noexcept = 0;
    virtual void setInt(
                        const char *key,
                        LONG value
                        ) noexcept = 0;
    virtual void setUInt(
                          const char *key,
                          ULONG value
                          ) noexcept = 0;
    virtual void setBool(
                          const char *key,
                          bool value
                          ) noexcept = 0;
    virtual void setFloat(
                          const char *key,
                          float value
                          ) noexcept = 0;
    virtual void setDouble(
                            const char *key,
                            double value
                            ) noexcept = 0;

    virtual void clear(const char *key) noexcept = 0;

    virtual void clearAll() noexcept = 0;
  };

  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //
  // ISettingsApplyDefaultsDelegate
  //

  interaction ISettingsApplyDefaultsDelegate
  {
    virtual void notifySettingsApplyDefaults() noexcept = 0;
    virtual void notifyVerifyRequiredSettings() noexcept(false) {} // throws InvalidUsage
  };

} // namespace zsLib

#ifdef _WIN32
#pragma warning(pop)
#endif //_WIN32
