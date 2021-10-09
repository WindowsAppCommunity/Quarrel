// Copyright (c) Quarrel. All rights reserved.

using System.Text.RegularExpressions;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A Converter that replaces multiple spaces with a new line.
    /// </summary>
    public sealed class MultipleWhiteSpaceCollapseConverter
    {
        /// <summary>
        /// Removes newlines from a string.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <returns>The string with new lines replaced.</returns>
        public static string Convert(string value)
        {
            if (value == null)
            {
                return null;
            }

            return new Regex("\n[ ]{1,}", RegexOptions.None).Replace(value, "\n");
        }
    }
}
