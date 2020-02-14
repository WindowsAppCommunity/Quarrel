using DiscordAPI.API.Guild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class ChangeToVisibilityConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool hide = true;
            if (value is Change change)
            {
                hide = change.NewValue == null;
                hide = hide || change.Key == "inviter_id";
            }
            return hide ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
