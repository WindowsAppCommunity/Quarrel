using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns an <see cref="Visibility.Visible"/> value if the input <see langword="object"/> is <see langword="null"/>
    /// </summary>
    public sealed class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool v;
            if (value is string sValue)
            {
                v = !string.IsNullOrEmpty(sValue);
            }
            else if (value is int iValue)
            {
                v = iValue > 0;
            }
            else if (value is ICollection cValue)
            {
                v = cValue.Count > 0;
            }
            else if (value is IEnumerable<object> eValue)
            {
                v = eValue.Any();
            }
            else
            {
                v = value != null;
            }
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
