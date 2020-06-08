// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns the username#disc for a uer.
    /// </summary>
    public sealed class UserToStringConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is User uValue)
            {
                return string.Format("{0}#{1}", uValue.Username, uValue.Discriminator);
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
