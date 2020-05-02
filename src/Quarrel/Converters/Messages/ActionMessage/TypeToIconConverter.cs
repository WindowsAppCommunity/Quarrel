// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns an MDL2 character Glyph based on ActionMessage type.
    /// </summary>
    public sealed class TypeToIconConverter : IValueConverter
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
                        return App.Current.Resources["RecipientAddIcon"];
                    case 2:
                        return App.Current.Resources["RecipientRemoveIcon"];
                    case 3:
                        return App.Current.Resources["CallIcon"];
                    case 4:
                    case 5:
                        return App.Current.Resources["EditIcon"];
                    case 6:
                        return App.Current.Resources["PinIcon"];
                    default:
                        return "?";
                }
            }

            return "?";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
