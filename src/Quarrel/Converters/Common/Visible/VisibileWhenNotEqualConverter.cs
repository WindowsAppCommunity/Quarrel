// Quarrel © 2022

using Quarrel.Converters.Common.Boolean;
using Windows.UI.Xaml;

namespace Quarrel.Converters.Common.Visible
{
    public sealed class VisibleWhenNotEqualConverter
    {
        public static Visibility Convert(object item1, object item2) => EqualityConverter.Convert(item1, item2) ? Visibility.Collapsed : Visibility.Visible;
    }
}
