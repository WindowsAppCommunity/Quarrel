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

#include <zsLib/internal/zsLib_Socket.h>

#ifdef _WIN32
#pragma warning(push)
#pragma warning(disable: 4290)
#endif // _WIN32

#define ZSLIB_SETTING_SOCKET_MONITOR_THREAD_PRIORITY "zsLib/socket-monitor/thread-priority"

namespace zsLib
{
  ZS_DECLARE_INTERACTION_PROXY(ISocketDelegate)

  class Socket  : public internal::Socket
  {
  public:
    struct Create {
      enum Family {
        IPv4 =      AF_INET,
        IPv6 =      AF_INET6
      };
      enum Type {
        Stream =    SOCK_STREAM,
        Datagram =  SOCK_DGRAM,
        Raw =       SOCK_RAW
      };
      enum Protocol {
        None = 0,
        TCP =       IPPROTO_TCP,
        UDP =       IPPROTO_UDP
      };
    };

    struct Exceptions {
      ZS_DECLARE_CUSTOM_EXCEPTION(InvalidSocket);
      ZS_DECLARE_CUSTOM_EXCEPTION(DelegateNotSet);

      ZS_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_1(Unspecified, int, errorCode);

      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(WouldBlock, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(ConnectionReset, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(AddressInUse, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(ConnectionRefused, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(NetworkNotReachable, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(HostNotReachable, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(Timeout, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(Shutdown, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(ConnectionAborted, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(BufferTooSmall, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(BufferTooBig, Unspecified, int)
      ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(UnsupportedSocketOption, Unspecified, int)
    };

    struct Receive {
      enum Options {
        None     = 0,
        Peek     = MSG_PEEK,
        OOB      = MSG_OOB,
        WaitAll  = MSG_WAITALL
      };
    };

    struct Send {
      enum Options {
        None = 0,
        OOB  = MSG_OOB
      };
    };

    struct Shutdown {
      enum Options {
        Send =      SD_SEND,
        Receive =   SD_RECEIVE,
        Both =      SD_BOTH
      };
    };

    struct SetOptionFlag {
      enum Options {
        NonBlocking          = FIONBIO,
        IgnoreSigPipe        = SO_NOSIGPIPE,
        Broadcast            = SO_BROADCAST,
        Debug                = SO_DEBUG,
        DontRoute            = SO_DONTROUTE,
        KeepAlive            = SO_KEEPALIVE,
        OOBInLine            = SO_OOBINLINE,
        ReuseAddress         = SO_REUSEADDR,
        BSDState             = SO_WINDOWS_BSD_STATE,
        ConditionalAccept    = SO_WINDOWS_CONDITIONAL_ACCEPT,
        ExclusiveAddressUse  = SO_WINDOWS_EXCLUSIVEADDRUSE,
        TCPNoDelay           = TCP_NODELAY,
      };
    };

    struct SetOptionValue {
      enum Options {
        ReceiverBufferSizeInBytes = SO_RCVBUF,
        SendBufferSizeInBytes     = SO_SNDBUF,
        LingerTimeInSeconds       = SO_LINGER, // specifying a time of 0 will disable linger
      };
    };

    struct GetOptionFlag {
      enum Options {
        IsListening             = SO_ACCEPTCONN,
        IsBroadcast             = SO_BROADCAST,
        IsDebugging             = SO_DEBUG,
        IsDontRoute             = SO_DONTROUTE,
        IsKeepAlive             = SO_KEEPALIVE,
        IsOOBInLine             = SO_OOBINLINE,
        IsReuseAddress          = SO_REUSEADDR,
        IsOOBAllRead            = SIOCATMARK,
        IsConditionalAccept     = SO_WINDOWS_CONDITIONAL_ACCEPT,
        IsLingering             = SO_WINDOWS_DONTLINGER,
        IsExclusiveAddressUse   = SO_EXCLUSIVEADDRUSE,
        IsTCPNoDelay            = TCP_NODELAY,
      };
    };

    struct GetOptionValue {
      enum Options {
        ErrorCode                  = SO_ERROR,
        ReceiverBufferSizeInBytes  = SO_RCVBUF,
        SendBufferSizeInBytes      = SO_SNDBUF,
        ReadyToReadSizeInBytes     = FIONREAD,    // how much data is available in a single recv method for stream types (more data may be pending), or how much data is in the buffer total for message oriented socket (however read will return the message size even if more data is available)
        Type                       = SO_TYPE,
        MaxMessageSizeInBytes      = SO_WINDOWS_MAX_MSG_SIZE
      };
    };

    struct Monitor {
      enum Options {
        Read      = 0x01,
        Write     = 0x02,
        Exception = 0x04,
        
        All       = (Read | Write | Exception),
      };
    };

  public:
    static void ignoreSIGPIPEOnThisThread();

    static SocketPtr create() noexcept(false); // throws Exceptions::Unspecified
    static SocketPtr createUDP(Create::Family inFamily = Create::IPv4) noexcept(false); // throws Exceptions::Unspecified
    static SocketPtr createTCP(Create::Family inFamily = Create::IPv4) noexcept(false); // throws Exceptions::Unspecified
    static SocketPtr create(Create::Family inFamily, Create::Type inType, Create::Protocol inProtocol) noexcept(false); // throws Exceptions::Unspecified

    ~Socket() noexcept(false); // throws Exceptions::WouldBlock, Exceptions::Unspecified

    SOCKET getSocket() const noexcept;
    SOCKET orphan() noexcept;                                                      // orphaning a socket will automatically remove any socket monitor
    void adopt(SOCKET inSocket) noexcept;

    virtual bool isValid() const noexcept;

    // socket must be valid in order to monitor the socket or an exception will be thrown
    virtual void setDelegate(ISocketDelegatePtr delegate = ISocketDelegatePtr()) noexcept(false); // throws Exceptions::InvalidSocket
    virtual void monitor(Monitor::Options options = Monitor::All) noexcept;

    virtual void close() noexcept(false); // throws Exceptions::WouldBlock, Exceptions::Unspecified  // closing a socket will automaticlaly remove any socket monitor

    virtual IPAddress getLocalAddress() const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::Unspecified
    virtual IPAddress getRemoteAddress() const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::Unspecified

    void bind(
              const IPAddress &inBindIP,
              int *noThrowErrorResult = NULL
              ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::AddressInUse, Exceptions::Unspecified

    virtual void listen() const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::AddressInUse, Exceptions::Unspecified

    virtual SocketPtr accept(
                             IPAddress &outRemoteIP,
                             bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                             int *noThrowErrorResult = NULL
                             ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::ConnectionReset, Exceptions::Unspecified

    virtual void connect(
                         const IPAddress &inDestination,     // destination of the connection
                         bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                         int *noThrowErrorResult = NULL
                         ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::WouldBlock, Exceptions::AddressInUse, Exceptions::NetworkNotReachable, Exceptions::HostNotReachable, Exceptions::Timeout, Exceptions::Unspecified

    virtual size_t receive(
                           BYTE *ioBuffer,
                           size_t inBufferLengthInBytes,
                           bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                           ULONG flags = (ULONG)(Receive::None),
                           int *noThrowErrorResult = NULL
                           ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::WouldBlock, Exceptions::Shutdown, Exceptions::ConnectionReset, Exceptions::ConnectionAborted, Exceptions::Timeout, Exceptions::BufferTooSmall, Exceptions::Unspecified

    virtual size_t receiveFrom(
                               IPAddress &outRemoteIP,
                               BYTE *ioBuffer,
                               size_t inBufferLengthInBytes,
                               bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                               ULONG flags = (ULONG)(Receive::None),
                               int *noThrowErrorResult = NULL
                               ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::WouldBlock, Exceptions::Shutdown, Exceptions::ConnectionReset, Exceptions::Timeout, Exceptions::BufferTooSmall, Exceptions::Unspecified

    virtual size_t send(
                        const BYTE *inBuffer,
                        size_t inBufferLengthInBytes,
                        bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                        ULONG flags = (ULONG)(Send::None),
                        int *noThrowErrorResult = NULL
                        ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::WouldBlock, Exceptions::Shutdown, Exceptions::HostNotReachable, Exceptions::ConnectionAborted, Exceptions::ConnectionReset, Exceptions::Timeout, Exceptions::BufferTooSmall, Exceptions::Unspecified

    virtual size_t sendTo(
                          const IPAddress &inDestination,
                          const BYTE *inBuffer,
                          size_t inBufferLengthInBytes,
                          bool *outWouldBlock = NULL,         // if this param is used, will return the "would block" as a result rather than throwing an exception
                          ULONG flags = (ULONG)(Send::None),
                          int *noThrowErrorResult = NULL
                          ) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::WouldBlock, Exceptions::Shutdown, Exceptions::Timeout, Exceptions::HostNotReachable, Exceptions::ConnectionAborted, Exceptions::ConnectionReset, Exceptions::Timeout, Exceptions::BufferTooSmall, Exceptions::Unspecified

    virtual void shutdown(Shutdown::Options inOptions = Shutdown::Both) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::Unspecified

    virtual void setBlocking(bool enabled) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::Unspecified

    virtual void setOptionFlag(SetOptionFlag::Options inOption, bool inEnabled) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::UnsupportedSocketOption, Exceptions::Unspecified
    virtual void setOptionValue(SetOptionValue::Options inOption, ULONG inValue) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::UnsupportedSocketOption, Exceptions::Unspecified

    virtual bool getOptionFlag(GetOptionFlag::Options inOption) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::UnsupportedSocketOption, Exceptions::Unspecified
    virtual ULONG getOptionValue(GetOptionValue::Options inOption) const noexcept(false); // throws Exceptions::InvalidSocket, Exceptions::UnsupportedSocketOption, Exceptions::Unspecified

    virtual void onReadReadyReset() const noexcept(false); // throws Exceptions::DelegateNotSet, Exceptions::InvalidSocket, Exceptions::Unspecified
    virtual void onWriteReadyReset() const noexcept(false); // throws Exceptions::DelegateNotSet, Exceptions::InvalidSocket, Exceptions::Unspecified
    virtual void onExceptionReset() const noexcept(false); // throws Exceptions::DelegateNotSet, Exceptions::InvalidSocket, Exceptions::Unspecified

  protected:
    Socket() noexcept(false); // throws Exceptions::Unspecified

  protected:
    SOCKET mSocket;
  };

  interaction ISocketDelegate
  {
    virtual void onReadReady(SocketPtr socket) = 0;
    virtual void onWriteReady(SocketPtr socket) = 0;
    virtual void onException(SocketPtr socket) = 0;
  };
}

#ifdef _WIN32
#pragma warning(pop)
#endif // _WIN32

ZS_DECLARE_PROXY_BEGIN(zsLib::ISocketDelegate)
ZS_DECLARE_PROXY_TYPEDEF(zsLib::SocketPtr, SocketPtr)
ZS_DECLARE_PROXY_METHOD(onReadReady, SocketPtr)
ZS_DECLARE_PROXY_METHOD(onWriteReady, SocketPtr)
ZS_DECLARE_PROXY_METHOD(onException, SocketPtr)
ZS_DECLARE_PROXY_END()
