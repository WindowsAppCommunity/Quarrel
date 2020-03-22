// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    /// <summary>
    /// Converter to check if change needs to be shown.
    /// </summary>
    public class ChangeToVisibilityConveter : IValueConverter
    {
        /// <summary>
        /// Determines if the Change should be visilble.
        /// </summary>
        /// <param name="value">Change to be checked for visibility.</param>
        /// <param name="targetType">Windows.UI.Xaml.Visibility.</param>
        /// <param name="parameter">Extra info.</param>
        /// <param name="language">What language the user is using.</param>
        /// <returns>Whether or not the change should show.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool hide = true;
            if (value is Change change)
            {
                hide = change.NewValue == null;
                hide = hide || change.Key == "inviter_id";
            }

            return hide ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
