// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Profile
{
    /// <summary>
    /// A converter that takes gets an activity type string.
    /// </summary>
    public class PresenceTypeToStatusConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // TODO: Localization
            if (value is int iValue)
            {
                switch (iValue)
                {
                    case 0:
                        return "Playing";
                    case 1:
                        return "Streaming";
                    case 2:
                        return "Listening to";
                    case 3:
                        return "Watching";
                    default:
                        return string.Empty;
                }
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
