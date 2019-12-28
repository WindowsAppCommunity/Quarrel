using Quarrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Windows.UI
{
    public static class ColorExtensions
    {
        public static Color GetDiscriminatorColor(string discriminator)
        {
            switch (Convert.ToInt32(discriminator) % 5)
            {
                case 0: //Blurple
                    return Color.FromArgb(255, 114, 137, 218);
                case 1: //Grey
                    return Color.FromArgb(255, 116, 127, 141);
                case 2: //Green
                    return Color.FromArgb(255, 67, 181, 129);
                case 3: //Yellow
                    return Color.FromArgb(255, 250, 166, 26);
                case 4: //Red
                    return Color.FromArgb(255, 250, 71, 71);
            }
            return Color.FromArgb(255, 114, 137, 218);
        }

        public static Color IntToColor(int color)
        {
            if (color == -1)
                return (App.Current.Resources["Foreground"] as SolidColorBrush).Color;
            else
            {
                if (color != 0)
                {
                    byte a = (byte)(255);
                    byte r = (byte)(color >> 16);
                    byte g = (byte)(color >> 8);
                    byte b = (byte)(color >> 0);
                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    return (App.Current.Resources["Foreground"] as SolidColorBrush).Color;
                }
            }
        }

        public static int ColorToInt(Color color)
        {
            return color.ToInt();
        }

        public static int ToInt(this Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            return (b << 0) +
                   (g << 8) +
                   (r << 16);
        }
    }
}
