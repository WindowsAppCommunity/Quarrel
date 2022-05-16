// Quarrel © 2022

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Converters.Common.Text
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
                return characterCasing switch
                {
                    CharacterCasing.Upper => value.ToUpper(),
                    CharacterCasing.Lower => value.ToLower(),
                    _ => value,
                };
            }

            return string.Empty;
        }
    }
}
