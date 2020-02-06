using ColorThiefDotNet;
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.APIStatus
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = value.ToString();

            if (status == "operational" || status == "none")
            {
                return (SolidColorBrush)App.Current.Resources["online"];
            }
            else if (status == "partial_outage" | status == "minor")
            {
                return (SolidColorBrush)App.Current.Resources["idle"];
            }
            else
            {
                return (SolidColorBrush)App.Current.Resources["dnd"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
