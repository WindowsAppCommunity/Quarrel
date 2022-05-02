// Quarrel © 2022

using Windows.UI;

namespace Quarrel.Converters
{
    public static class StatusToColorConverter
    {
        public static Color Convert(string status)
        {
            string resource = status switch
            {
                "none" or "operational" => "OnlineColor",
                "partial_outage" or "minor" => "IdleColor",
                _ => "DndColor",
            };

            return (Color)App.Current.Resources[resource];
        }
    }
}
