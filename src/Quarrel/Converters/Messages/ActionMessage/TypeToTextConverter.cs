// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns text based on the ActionMessage type.
    /// </summary>
    public sealed class TypeToTextConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // TODO: Localization
            if (value is int iValue)
            {
                switch (iValue)
                {
                    case 1:
                        return "Added a user";
                    case 2:
                        return "Removed a user";
                    case 3:
                        return "Called";
                    case 4:
                        return "Changed channel name";
                    case 5:
                        return "Changed channel icon";
                    case 6:
                        return "Pinned a message";
                    case 7:
                        return "Joined the server";
                    default:
                        return "Did something";
                }
            }

            return "Did something";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
