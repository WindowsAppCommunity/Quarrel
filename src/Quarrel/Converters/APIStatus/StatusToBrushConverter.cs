// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.APIStatus
{
    /// <summary>
    /// Converter for DiscordAPI operation status to a brush color.
    /// </summary>
    public class StatusToBrushConverter
    {
        /// <summary>
        /// Converts DiscordAPI operation status to a brush color.
        /// </summary>
        /// <param name="value">DiscordAPI operational status as a string.</param>
        /// <returns>A <see cref="SolidColorBrush"/> for the API status.</returns>
        public static SolidColorBrush Convert(string value)
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
    }
}
