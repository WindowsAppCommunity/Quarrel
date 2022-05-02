// Quarrel © 2022

using Windows.UI.Xaml;

namespace Quarrel.Converters
{
    public class VisibleWhenNotNullConverter
    {
        public static Visibility Convert(object? item)
        {
            return item is null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
