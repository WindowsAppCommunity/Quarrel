// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A Converter that replaces multiple spaces with a new line.
    /// </summary>
    public sealed class MultipleWhiteSpaceCollapseConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Regex("\n[ ]{1,}", RegexOptions.None).Replace((string)value, "\n");
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
