using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Discord
{
    public sealed class DiscrimatorPrefixConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "#" + value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
