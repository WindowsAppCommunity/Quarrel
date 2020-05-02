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
#include <zsLib/Singleton.h>

namespace zsLib
{
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //-------------------------------------------------------------------------
  //
  // IFactory<XFACTORYINTERFACE>
  //

  template<typename XFACTORYINTERFACE>
  interaction IFactory : public XFACTORYINTERFACE,
                         public zsLib::Any
  {
  public:
    ZS_DECLARE_TYPEDEF_PTR(XFACTORYINTERFACE, UseFactoryInterface);
    ZS_DECLARE_TYPEDEF_PTR(IFactory, UseFactory);

  public:
    static void override(UseFactoryInterfacePtr override) noexcept
    {
      singletonFactory().mOverride = override;
    }

    static UseFactoryInterface &singleton() noexcept
    {
      UseFactory &factory = singletonFactory();
      if (factory.mOverride) return (*factory.mOverride);
      return factory;
    }

  private:
    static UseFactory &singletonFactory() noexcept
    {
      static Singleton<UseFactory, false> factory;
      return factory.singleton();
    }

    UseFactoryInterfacePtr mOverride;
  };

} // namespace zsLib
