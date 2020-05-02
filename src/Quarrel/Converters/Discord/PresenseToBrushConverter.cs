// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a <see cref="SolidColorBrush"/> for the presence of a user.
    /// </summary>
    public sealed class PresenseToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to use the system accent or offline color for offline users.
        /// </summary>
        public bool UseSystemAccentColor { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = (string)value ?? "offline";
            if (UseSystemAccentColor && (status == "offline" || status == "invisible"))
            {
                return new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
            }

            return App.Current.Resources[status];
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
