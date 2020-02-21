// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility.Visible"/> if the FriendStatus is Incoming pending.
    /// </summary>
    public sealed class RelationToAcceptFriendConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 3 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
