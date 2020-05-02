/*

 Copyright (c) 2017, Robin Raymond
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

#include <zsLib/internal/zsLib_RangeSelection.h>

namespace zsLib
{
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // RangeSelection
  //

  template <
    typename RangeType,
    typename LargeUnsignedType = unsigned long long,
    typename LargeSignedType = signed long long
  >
  class RangeSelection
  {
  public:
    typedef RangeType UseRangeType;
    typedef LargeUnsignedType UseLargeUnsignedType;
    typedef LargeSignedType UseLargeSignedType;

  public:
    //-------------------------------------------------------------------------
    RangeSelection(const RangeSelection &op2) noexcept :
      range_(op2.range_)
    {
    }

    //-------------------------------------------------------------------------
    RangeSelection() noexcept {}

    //-------------------------------------------------------------------------
    ~RangeSelection() noexcept {}

    //-------------------------------------------------------------------------
    static RangeSelection createFromString(const char *previousExportString) noexcept(false)
    {
      RangeSelection result;
      result.importFromString(previousExportString);
      return result;
    }

    //-------------------------------------------------------------------------
    static RangeSelection createFromSetting(const char *key) noexcept(false)
    {
      RangeSelection result;
      result.importFromSetting(key);
      return result;
    }

    //-------------------------------------------------------------------------
    RangeSelection &operator=(const RangeSelection &op2) noexcept
    {
      range_ = op2.range_;
      return *this;
    }

    //-------------------------------------------------------------------------
    void importFromSetting(const char *key) noexcept(false) // throws zsLib::Exceptions::InvalidArgument
    {
      range_.importFromSetting(key);
    }

    //-------------------------------------------------------------------------
    void importFromString(const char *previousExportString) noexcept(false) // throws zsLib::Exceptions::InvalidArgument
    {
      range_.importFromString(previousExportString);
    }

    //-------------------------------------------------------------------------
    void exportToSetting(const char *key) const noexcept
    {
      range_.exportToSetting(key);
    }

    //-------------------------------------------------------------------------
    String exportToString() const noexcept
    {
      return range_.exportToString();
    }

    //-------------------------------------------------------------------------
    void reset() noexcept
    {
      range_.reset();
    }
    
    //-------------------------------------------------------------------------
    void allow(
                UseRangeType from,
                UseRangeType to
                ) noexcept
    {
      range_.allow(from, to);
    }

    //-------------------------------------------------------------------------
    void deny(
              UseRangeType from,
              UseRangeType to
              ) noexcept
    {
      range_.deny(from, to);
    }

    //-------------------------------------------------------------------------
    void removeAllow(
                      UseRangeType from,
                      UseRangeType to
                      ) noexcept
    {
      range_.removeAllow(from, to);
    }
    
    //-------------------------------------------------------------------------
    void removeDeny(
                    UseRangeType from,
                    UseRangeType to
                    ) noexcept
    {
      range_.removeDeny(from, to);
    }

    //-------------------------------------------------------------------------
    UseRangeType getRandomPosition(UseLargeUnsignedType randomInputValue) noexcept(false) // throws ::zsLib::Exceptions::BadState
    {
      return range_.getRandomPosition(randomInputValue);
    }

    //-------------------------------------------------------------------------
    bool isAllowed(UseRangeType value) noexcept
    {
      return range_.isAllowed(value);
    }

  protected:
    internal::RangeSelection<UseRangeType, UseLargeUnsignedType> range_;
  };
}
