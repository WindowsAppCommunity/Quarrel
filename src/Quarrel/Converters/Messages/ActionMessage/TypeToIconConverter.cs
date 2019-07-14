using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value
    /// </summary>
    public sealed class TypeToIconConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
