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

#ifndef ZSLIB_INTERNAL_SOCKET_H_6f504f01bdd331d0c835ccae3872ce91
#define ZSLIB_INTERNAL_SOCKET_H_6f504f01bdd331d0c835ccae3872ce91

#include <zsLib/Exception.h>
#include <zsLib/Proxy.h>

#ifdef _WIN32
#include <winsock2.h>
#include <ws2def.h>
#include <ws2tcpip.h>

namespace zsLib
{
  enum MissingSocketOptions
  {
    SO_NOSIGPIPE = -1,
  };
}

#else //!_WIN32
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <sys/ioctl.h>
#ifndef FIONREAD
#include <sys/filio.h>
#endif //ndef FIONREAD

namespace zsLib
{
  enum MissingSocketOptions
  {
    SD_SEND = SHUT_WR,
    SD_RECEIVE = SHUT_RD,
    SD_BOTH = SHUT_RDWR,

    SO_BSP_STATE = -1,
    SO_CONDITIONAL_ACCEPT = -1,
    SO_EXCLUSIVEADDRUSE = -1,
    SO_DONTLINGER = -1,
    SO_MAX_MSG_SIZE = -1,
    SO_PORT_SCALABILITY = -1,
#if (defined ANDROID || defined __QNX__ || defined __linux__ || defined __unix__)
    SO_NOSIGPIPE = -1,
#endif //(defined ANDROID || defined __QNX__ || defined __linux__)

    INVALID_SOCKET = -1,
    SOCKET_ERROR = -1,

    WSAEINPROGRESS = EINPROGRESS,
    WSAEWOULDBLOCK = EWOULDBLOCK,
    WSAEADDRINUSE = EADDRINUSE,
    WSAECONNRESET = ECONNRESET,
    WSAECONNREFUSED = ECONNREFUSED,
    WSAENETUNREACH = ENETUNREACH,
    WSAEHOSTUNREACH = EHOSTUNREACH,
    WSAETIMEDOUT = ETIMEDOUT,
    WSAESHUTDOWN = ESHUTDOWN,
    WSAECONNABORTED = ECONNABORTED,
    WSAENETRESET = ENETRESET,
    WSAEMSGSIZE = EMSGSIZE
  };
}

#endif //_WIN32

namespace zsLib
{
  enum WindowsSocketOptions
  {
    SO_WINDOWS_BSD_STATE = SO_BSP_STATE,
    SO_WINDOWS_CONDITIONAL_ACCEPT = SO_CONDITIONAL_ACCEPT,
    SO_WINDOWS_EXCLUSIVEADDRUSE = SO_EXCLUSIVEADDRUSE,
    SO_WINDOWS_DONTLINGER = SO_DONTLINGER,
    SO_WINDOWS_MAX_MSG_SIZE = SO_MAX_MSG_SIZE,
    SO_WINDOWS_PORT_SCALABILITY = SO_PORT_SCALABILITY,
  };
}

namespace zsLib
{
  ZS_DECLARE_CLASS_PTR(Socket)

  namespace internal
  {
    ZS_DECLARE_CLASS_PTR(SocketMonitor)

    class Socket
    {
    protected:
      Socket();

      Socket(const Socket &) = delete;

    public:
      ~Socket();

      void notifyReadReady();
      void notifyWriteReady();
      void notifyException();

      void linkSocketMonitor();
      void unlinkSocketMonitor();

    protected:
      ISocketDelegatePtr mDelegate;
      mutable RecursiveLock mLock;
      SocketWeakPtr mThis;

      SocketMonitorPtr mMonitor;

      std::atomic<bool> mMonitorReadReady {};
      std::atomic<bool> mMonitorWriteReady {};
      std::atomic<bool> mMonitorException {};
    };
  }
}

#ifndef _WIN32
typedef int SOCKET;
#endif //ndef _WIN32

#endif //ZSLIB_INTERNAL_SOCKET_H_6f504f01bdd331d0c835ccae3872ce91
