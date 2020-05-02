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

#include <zsLib/internal/zsLib_Singleton.h>

namespace zsLib
{
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // BoxedAllocation<T>
  //

  template <typename T, bool allowDestroy = true>
  class BoxedAllocation
  {
  public:
    BoxedAllocation() noexcept
    {
      BYTE *buffer = (&(mBuffer[0]));
      auto alignment = alignof(T);
      auto misalignment = reinterpret_cast<uintptr_t>(buffer) % alignment;
      // alignment = 8, misalignment = 1, add alignment - misalignmnet (8-1 = +7)
      // alignment = 8, misalignment = 7, add alignment - misalignmnet (8-7 = +1)
      // alignment = 8, misalignment = 0, add 0 to alignment
      BYTE *alignedBuffer = buffer + (misalignment == 0 ? 0 : (alignment - misalignment));
      mObject = new (alignedBuffer) T;
    }

    ~BoxedAllocation() noexcept
    {
      if (!allowDestroy) return;

      mObject->~T();
    }

    T &ref() noexcept
    {
      return *mObject;
    }

    const T &ref() const noexcept
    {
      return *mObject;
    }

  private:
    BYTE mBuffer[sizeof(T) + alignof(T)];
    T *mObject;
  };

  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // Singleton<T>
  //

  template <typename T, bool allowDestroy = true>
  class Singleton : BoxedAllocation<T, allowDestroy>
  {
  public:
    T &singleton() noexcept
    {
      return BoxedAllocation<T, allowDestroy>::ref();
    }

  private:
  };

  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // SingletonLazySharedPtr<T, bool allowDestroy>
  //

  template <typename T, bool allowDestroy = true>
  class SingletonLazySharedPtr : BoxedAllocation< std::weak_ptr<T>, false >
  {
  public:
    ZS_DECLARE_PTR(T)

  public:
    SingletonLazySharedPtr(TPtr pThis) noexcept
    {
      mThis = pThis;
      weakRef() = pThis;

      if (!allowDestroy) {
        // throw away an extra reference to "pThis" intentionally
        BoxedAllocation<TPtr, false> bogusReference;
        (bogusReference.ref()) = pThis;
      }
    }

    TPtr singleton() noexcept
    {
      return weakRef().lock();
    }

  private:
    TWeakPtr &weakRef() noexcept
    {
      return BoxedAllocation< TWeakPtr, false >::ref();
    }
    
  private:
    TPtr mThis;
  };
  
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // SingletonManager
  //

  class SingletonManager : public internal::SingletonManager
  {
  public:

    //-------------------------------------------------------------------------
    //
    // Initializer
    //

    class Initializer : public internal::SingletonManager::Initializer
    {
    public:
      //-----------------------------------------------------------------------
      // PURPOSE: Initialize the singleton manager
      // NOTES:   Declare this an instance of this type whereever the
      //          SingletonManager is used. At the end of all instance's
      //          lifetime all singletons will be notified to clean-up.
      //          Typically an instance of this class is declared inside
      //          "main(...)"
      Initializer() noexcept;
      ~Initializer() noexcept;
    };

    //-------------------------------------------------------------------------
    //
    // Register
    //

    class Register : public internal::SingletonManager::Register
    {
    public:
      //-----------------------------------------------------------------------
      // PURPOSE: Register a singleton for cleanup at the end of the
      //          application lifecycle.
      // NOTES:   This ensures the singleton is cleaned up without relying on
      //          the global space cleanup. Declare this class statically after
      //          creating the singleton. This will ensure at least one
      //          reference to the object exists outside the singleton to
      //          ensure a reference exists during the cleanup phase.
      Register(
               const char *uniqueNamespacedID,
               ISingletonManagerDelegatePtr singleton
               ) noexcept;
      ~Register() noexcept;

      //-----------------------------------------------------------------------
      // PURPOSE: Find a previously registered singleton
      static ISingletonManagerDelegatePtr find(const char *uniqueNamespacedID) noexcept;
    };
  };

  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //---------------------------------------------------------------------------
  //
  // ISingletonManagerDelegate
  //

  interaction ISingletonManagerDelegate
  {
    virtual void notifySingletonCleanup() noexcept = 0;
  };

} // namespace zsLib
