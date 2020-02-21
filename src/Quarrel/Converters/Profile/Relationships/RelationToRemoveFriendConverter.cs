// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns <see cref="Visibility.Visible"/> if the user has a friend relation status.
    /// </summary>
    public sealed class RelationToRemoveFriendConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
