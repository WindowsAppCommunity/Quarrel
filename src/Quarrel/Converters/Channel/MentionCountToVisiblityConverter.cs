using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MentionCountToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
