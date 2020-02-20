// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns a <see cref="Visibility.Visible"/> if the user has a blocked relation status.
    /// </summary>
    public sealed class RelationToUnblockConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
