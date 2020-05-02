// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Resources;

namespace QuarrelSmartColor.Extensions.Windows.UI
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the DateTime type.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Gets a <see cref="Color"/> <paramref name="discriminator"/>.
        /// </summary>
        /// <param name="discriminator">A user's discriminator.</param>
        /// <returns>The user's default color by <paramref name="discriminator"/>.</returns>
        public static Color GetDiscriminatorColor(string discriminator)
        {
            switch (Convert.ToInt32(discriminator) % 5)
            {
                case 0: // Blurple
                    return Color.FromArgb(255, 114, 137, 218);
                case 1: // Grey
                    return Color.FromArgb(255, 116, 127, 141);
                case 2: // Green
                    return Color.FromArgb(255, 67, 181, 129);
                case 3: // Yellow
                    return Color.FromArgb(255, 250, 166, 26);
                case 4: // Red
                    return Color.FromArgb(255, 250, 71, 71);
            }

            return Color.FromArgb(255, 114, 137, 218);
        }

        /// <summary>
        /// Gets a <see cref="Color"/> from the <paramref name="color"/>.
        /// </summary>
        /// <param name="color">An int color.</param>
        /// <returns>The <see cref="Color"/> for <paramref name="color"/>.</returns>
        public static Color IntToColor(int color)
        {
            if (color == -1)
            {
                return (SimpleIoc.Default.GetInstance<IResourceService>().GetResource("Foreground") as SolidColorBrush).Color;
            }
            else
            {
                if (color != 0)
                {
                    byte a = 255;
                    byte r = (byte)(color >> 16);
                    byte g = (byte)(color >> 8);
                    byte b = (byte)(color >> 0);
                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    return (SimpleIoc.Default.GetInstance<IResourceService>().GetResource("Foreground") as SolidColorBrush).Color;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="SolidColorBrush"/> from the <paramref name="color"/>.
        /// </summary>
        /// <param name="color">An int color.</param>
        /// <returns>The <see cref="SolidColorBrush"/> for the <paramref name="color"/>.</returns>
        public static SolidColorBrush IntToBrush(int color)
        {
            return new SolidColorBrush(IntToColor(color));
        }

        /// <summary>
        /// The int form of the <paramref name="color"/>.
        /// </summary>
        /// <param name="color">The color to find the <see langword="int"/> for.</param>
        /// <returns>The color in standard <see langword="int"/> form.</returns>
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
