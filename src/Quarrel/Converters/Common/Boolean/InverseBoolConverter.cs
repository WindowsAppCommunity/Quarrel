// Quarrel © 2022

namespace Quarrel.Converters.Common.Boolean
{
    public sealed class InverseBoolConverter
    {
        /// <summary>
        /// Gets the inverse of a bool.
        /// </summary>
        /// <param name="data">The bool to inverse.</param>
        /// <returns>An inversed bool.</returns>
        public static bool Convert(bool data) => !data;
    }
}
