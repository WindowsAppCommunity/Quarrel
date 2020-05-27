// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Extensions;

#nullable enable

namespace Microsoft.Toolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to throw exceptions
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1618", Justification = "Internal helper methods")]
    internal static partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotEmpty{T}(T[],string)"/> (or an overload) fails.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentExceptionForIsNotEmpty<T>(string name)
        {
            ThrowArgumentException(name, $"Parameter {name.ToAssertString()} ({typeof(T).ToTypeString()}) must not be empty");
        }
    }
}
