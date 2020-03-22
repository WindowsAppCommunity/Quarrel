// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Quarrel.Services.Voice
{
    /// <summary>
    /// An interface for accessing a stream as a byte array.
    /// </summary>
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMemoryBufferByteAccess
    {
        /// <summary>
        /// Gets <paramref name="buffer"/> from the stream.
        /// </summary>
        /// <param name="buffer">A byte array pointer.</param>
        /// <param name="capacity">The size of <paramref name="buffer"/>.</param>
        void GetBuffer(out byte* buffer, out uint capacity);
    }
}
