// Adam Dernis © 2022

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Converters
{
    /// <summary>
    /// A converter that changes the casing of a string.
    /// </summary>
    public class CaseConverter
    {
        /// <summary>
        /// Gets or sets the character casing to convert an input string to.
        /// </summary>
        public CharacterCasing Case { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CaseConverter"/> class.
        /// </summary>
        public CaseConverter()
        {
            Case = CharacterCasing.Upper;
        }

        /// <summary>
        /// Converts a string to a certain character casing.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="characterCasing">The result's character casing.</param>
        /// <returns>String <paramref name="value"/> in the specified character casing.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Convert(string? value, CharacterCasing characterCasing)
        {
            if (value != null)
            {
                switch (characterCasing)
                {
                    case CharacterCasing.Upper:
                        return value.ToUpper();

                    case CharacterCasing.Lower:
                        return value.ToLower();

                    case CharacterCasing.Normal:
                    default:
                        return value;
                }
            }

            return string.Empty;
        }
    }
}
