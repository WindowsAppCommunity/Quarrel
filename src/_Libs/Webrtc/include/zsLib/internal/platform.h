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

#ifndef ZSLIB_INTERNAL_PLATFORM_H_ae1ca1614cb82fd6e3e9751af73f2658
#define ZSLIB_INTERNAL_PLATFORM_H_ae1ca1614cb82fd6e3e9751af73f2658

// these are the predefines that might be available on various platforms (start out undefined)
#undef HAVE_IPHLPAPI_H
#undef HAVE_NET_IF_H
#undef HAVE_PTHREAD_H
#undef HAVE_WINDOWS_H

#undef HAVE_IF_NAMETOINDEX
#undef HAVE_SOCKADDR_IN_LEN
#undef HAVE_PTHREAD_SETNAME_WITH_1
#undef HAVE_PTHREAD_SETNAME_WITH_2
#undef HAVE_RAISEEXCEPTION
#undef HAVE_SPRINTF_S
#undef HAVE_STRCPY_S

#ifdef _WIN32

// WIN32 platforms have these defined
#define HAVE_IPHLPAPI_H 1
#define HAVE_WINDOWS_H 1
#define HAVE_IF_NAMETOINDEX 1
#define HAVE_RAISEEXCEPTION 1
#define HAVE_SPRINTF_S 1
#define HAVE_STRCPY_S 1

#if defined(WINUWP) || defined(WIN32_RX64)

// WINUWP has these defined

// WINUWP does not support these features (but WIN32 does)
#undef HAVE_IF_NAMETOINDEX

#if defined(WINAPI_FAMILY) && (WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP || defined(WIN32_RX64))

// WINUWP phone has these defined

// WINUWP phone odes not support these features (but WINUWP does)
#undef HAVE_IPHLPAPI_H
#endif //defined(WINAPI_FAMILY) && WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP

#endif //WINUWP

#endif //_WIN32


#ifdef __QNX__
// QNX supports these features
#define HAVE_NET_IF_H 1
#define HAVE_PTHREAD_H 1
#define HAVE_IF_NAMETOINDEX 1
#define HAVE_PTHREAD_SETNAME_WITH_2 1
#endif //__QNX__


#ifdef __APPLE__

// Mac / iOS support these features
#define HAVE_NET_IF_H 1
#define HAVE_PTHREAD_H 1
#define HAVE_SOCKADDR_IN_LEN 1
#define HAVE_IF_NAMETOINDEX 1
#define HAVE_PTHREAD_SETNAME_WITH_1 1

#endif //__APPPLE__


#ifdef __linux__

// Linux has these options
#define HAVE_NET_IF_H 1
#define HAVE_PTHREAD_H 1
#define HAVE_IF_NAMETOINDEX 1
#define HAVE_PTHREAD_SETNAME_WITH_2 1

#ifdef ANDROID

// Android supports these additional features

// Android does not support these features

#endif //ANDROID
#endif //__linux__

#endif //ZSLIB_INTERNAL_PLATFORM_H_ae1ca1614cb82fd6e3e9751af73f2658
