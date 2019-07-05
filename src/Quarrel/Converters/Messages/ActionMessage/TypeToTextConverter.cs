using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Messages.ActionMessage
{

    public sealed class TypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
