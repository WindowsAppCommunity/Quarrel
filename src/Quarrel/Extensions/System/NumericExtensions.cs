// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the numeric types.
    /// </summary>
    internal static partial class NumericExtensions
    {
        /// <summary>
        /// Clamps the input value inside the specified range.
        /// </summary>
        /// <param name="value">The input value to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns><paramref name="value"/> if it's between <paramref name="min"/> and <paramref name="max"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Returns the absolute value of the input number.
        /// </summary>
        /// <param name="value">The input number.</param>
        /// <returns>The absolute value of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Abs(this int value) => value >= 0 ? value : -value;

        /// <summary>
        /// Returns the square of the input number.
        /// </summary>
        /// <param name="value">The input value to square.</param>
        /// <returns><paramref name="value"/> to the power of 2.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static double Square(this double value) => value * value;
    }
}
