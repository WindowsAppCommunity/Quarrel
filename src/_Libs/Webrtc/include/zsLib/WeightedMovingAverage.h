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

namespace zsLib
{
  template <typename XAVERAGETYPE>
  class WeightedMovingAverage
  {
  public:
    typedef XAVERAGETYPE AverageType;

    WeightedMovingAverage(AverageType weightOfNewValues = 0.20) noexcept : mWeightOfNewValues(weightOfNewValues) {}

    WeightedMovingAverage &operator+=(float value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(double value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }

    WeightedMovingAverage &operator+=(bool value) noexcept { adjust(static_cast<AverageType>(value ? 1.0 : 0.0)); return (*this); }
    WeightedMovingAverage &operator+=(CHAR value) noexcept { (*this) += static_cast<int>(value); return (*this); }
    WeightedMovingAverage &operator+=(UCHAR value) noexcept { (*this) += static_cast<unsigned int>(value); return (*this); }
    WeightedMovingAverage &operator+=(SHORT value) noexcept { (*this) += static_cast<int>(value); return (*this); }
    WeightedMovingAverage &operator+=(USHORT value) noexcept { (*this) += static_cast<unsigned int>(value); return (*this); }
    WeightedMovingAverage &operator+=(int value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(unsigned int value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(long value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(unsigned long value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(long long value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }
    WeightedMovingAverage &operator+=(unsigned long long value) noexcept { adjust(static_cast<AverageType>(value)); return (*this); }

    template <typename data_type>
    data_type value() const noexcept { return static_cast<data_type>(mLastValue); }

  protected:
    void adjust(AverageType input) noexcept
    {
      if (mFirstAdjustment) {
        mLastValue = input;
        mFirstAdjustment = false;
      } else {
        mLastValue = (mLastValue * (static_cast<AverageType>(1.0) - mWeightOfNewValues)) + (input * mWeightOfNewValues);
      }
    }

  public:
  protected:

    bool mFirstAdjustment {true};
    AverageType mWeightOfNewValues {};
    AverageType mLastValue {};
  };

  typedef WeightedMovingAverage<float> WeightedMovingAverageFloat;
  typedef WeightedMovingAverage<double> WeightedMovingAverageDouble;

  template <typename XAVERAGETYPE>
  class WeightedMovingAverageUsingTotal
  {
  public:
    typedef XAVERAGETYPE AverageType;

    WeightedMovingAverageUsingTotal(AverageType weightOfNewValues = 0.20) noexcept : mCalculator(weightOfNewValues) {}

    WeightedMovingAverageUsingTotal &operator+=(float newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(double newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }

    WeightedMovingAverageUsingTotal &operator+=(bool newTotal) noexcept { adjust(static_cast<AverageType>(newTotal ? 1.0 : 0.0)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(CHAR newTotal) noexcept { (*this) += static_cast<int>(newTotal); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(UCHAR newTotal) noexcept { (*this) += static_cast<unsigned int>(newTotal); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(SHORT newTotal) noexcept { (*this) += static_cast<int>(newTotal); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(USHORT newTotal) noexcept { (*this) += static_cast<unsigned int>(newTotal); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(int newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(unsigned int newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(long newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(unsigned long newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(long long newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }
    WeightedMovingAverageUsingTotal &operator+=(unsigned long long newTotal) noexcept { adjust(static_cast<AverageType>(newTotal)); return (*this); }

    template <typename data_type>
    data_type value() const noexcept { return mCalculator.template value<data_type>(); }

  protected:
    void adjust(AverageType newTotal) noexcept
    {
      AverageType nextValue = (newTotal - mLastTotal);
      mLastTotal = newTotal;
      if (!mFirstAdjustment) {
        mCalculator += nextValue;
      }
      mFirstAdjustment = false;
    }

  protected:
    bool mFirstAdjustment {true};
    WeightedMovingAverage<AverageType> mCalculator;
    AverageType mLastTotal {};
  };

  typedef WeightedMovingAverageUsingTotal<float> WeightedMovingAverageUsingTotalFloat;
  typedef WeightedMovingAverageUsingTotal<double> WeightedMovingAverageUsingTotalDouble;

} // namespace zsLib
