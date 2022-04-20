// Quarrel © 2022

using Windows.UI.Xaml;

namespace Quarrel.Converters
{
    public sealed class VisibleIfNotEqualConverter
    {
        public static Visibility Convert(object item1, object item2)
            => EqualityConverter.Convert(item1, item2) ? Visibility.Collapsed : Visibility.Visible;
    }
}
