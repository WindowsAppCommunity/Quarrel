// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns a bool indicating whether or not the value is 3.
    /// </summary>
    public sealed class VoiceTypeToBoolConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // TODO: Localization
            if (value is int iValue)
            {
                return iValue == 3;
            }

            return false;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
