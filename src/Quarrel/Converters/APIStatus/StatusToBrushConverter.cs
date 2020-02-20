// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.APIStatus
{
    /// <summary>
    /// Converter for DiscordAPI operation status to a brush color.
    /// </summary>
    public class StatusToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts DiscordAPI operation status to a brush color.
        /// </summary>
        /// <param name="value">DiscordAPI operational status as a string.</param>
        /// <param name="targetType">Requested out type.</param>
        /// <param name="parameter">Extra info.</param>
        /// <param name="language">What language the user is using.</param>
        /// <returns>A SolidColorBrush for the API status.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = value.ToString();

            if (status == "operational" || status == "none")
            {
                return (SolidColorBrush)App.Current.Resources["online"];
            }
            else if (status == "partial_outage" | status == "minor")
            {
                return (SolidColorBrush)App.Current.Resources["idle"];
            }
            else
            {
                return (SolidColorBrush)App.Current.Resources["dnd"];
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
