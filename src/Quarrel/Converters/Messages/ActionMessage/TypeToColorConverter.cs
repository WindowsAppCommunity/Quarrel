// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns a <see cref="SolidColorBrush"/> based on the ActionMessage type.
    /// </summary>
    public sealed class TypeToColorConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int iValue)
            {
                switch (iValue)
                {
                    case 1:
                    case 7:
                        return App.Current.Resources["online"];
                    case 4:
                    case 5:
                        return App.Current.Resources["idle"];
                    case 2:
                        return App.Current.Resources["dnd"];
                    case 3:
                    case 6:
                    default:
                        return App.Current.Resources["Foreground"];
                }
            }

            return App.Current.Resources["Foreground"];
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
