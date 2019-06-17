// Credit to Sergio Pedri for extensions

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the numeric types
    /// </summary>
    internal static partial class NumericExtensions
    {
        /// <summary>
        /// Clamps the input value inside the specified range
        /// </summary>
        /// <param name="value">The input value to clamp</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Returns the absolute value of the input number
        /// </summary>
        /// <param name="value">The input number</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Abs(this int value) => value >= 0 ? value : -value;

        /// <summary>
        /// Returns the square of the input number
        /// </summary>
        /// <param name="value">The input value to square</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static double Square(this double value) => value * value;

        /// <summary>
        /// Creates an icon from the input value (see the Segoe MDL2 Assets font)
        /// </summary>
        /// <param name="value">The input icon value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure, NotNull]
        public static string ToIcon(this int value) => $"{Convert.ToChar(value)}";
    }
}
