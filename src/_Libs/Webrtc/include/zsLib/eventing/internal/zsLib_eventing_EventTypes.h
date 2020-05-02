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

#ifdef _WIN32
#include <Evntprov.h>
#endif //_WIN32

namespace zsLib
{
  namespace eventing
  {
    namespace internal
    {
#ifdef _WIN32
      typedef EVENT_DATA_DESCRIPTOR USE_EVENT_DATA_DESCRIPTOR;
      typedef EVENT_DESCRIPTOR USE_EVENT_DESCRIPTOR;
#else
      struct EventDataDescriptor
      {
        uintptr_t Ptr;
        size_t Size;
        size_t Type;
      };
      
      struct EventDescriptor
      {
        uint16_t Id;
        uint8_t Version;
        uint8_t Channel;
        uint8_t Level;
        uint8_t Opcode;
        uint16_t Task;
        uint64_t Keyword;
      };
      
      typedef EventDataDescriptor USE_EVENT_DATA_DESCRIPTOR;
      typedef EventDescriptor USE_EVENT_DESCRIPTOR;
#endif //_WIN32
      
      struct EventParameterDescriptor
      {
        EventParameterTypes Type;
      };
      
      typedef EventParameterDescriptor USE_EVENT_PARAMETER_DESCRIPTOR;
      typedef int USE_EVENT_DATA_BOOL_TYPE;
    }
  }
}
