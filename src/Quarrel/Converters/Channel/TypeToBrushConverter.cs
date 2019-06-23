using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 4 ? App.Current.Resources["SystemControlBackgroundAccentBrush"] : App.Current.Resources["Foreground"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
