// Quarrel © 2022

using Windows.UI.Xaml.Media;

namespace Quarrel.Converters
{
    public static class StatusToBrushConverter
    {
        public static SolidColorBrush Convert(string status)
            => new SolidColorBrush(StatusToColorConverter.Convert(status));
    }
}
