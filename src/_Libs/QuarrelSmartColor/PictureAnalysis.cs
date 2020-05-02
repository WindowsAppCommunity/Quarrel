// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using static QuarrelSmartColor.ColorDetection;

namespace QuarrelSmartColor
{
    /// <summary>
    /// An analysis of an image.
    /// </summary>
    public class PictureAnalysis
    {
        /// <summary>
        /// Gets the dominant colors from an analyzed image.
        /// </summary>
        public List<MMCQ.QuantizedColor> ColorList { get; private set; } = new List<MMCQ.QuantizedColor>();

        /// <summary>
        /// Convert RGB color to HSL (for Poster_Loaded).
        /// </summary>
        /// <param name="r">Red channel.</param>
        /// <param name="g">Green channel.</param>
        /// <param name="b">Blue channel.</param>
        /// <returns><see cref="HslColor"/>.</returns>
        public static MMCQ.HSLColor FromRGB(byte r, byte g, byte b)
        {
            float rF = r / 255f;
            float gF = g / 255f;
            float bF = b / 255f;

            float min = Math.Min(Math.Min(rF, gF), bF);
            float max = Math.Max(Math.Max(rF, gF), bF);
            float delta = max - min;

            float h = 0;
            float s = 0;
            float l = (float)((max + min) / 2.0f);

            if (delta != 0)
            {
                if (l < 0.5f)
                {
                    s = (float)(delta / (max + min));
                }
                else
                {
                    s = (float)(delta / (2.0f - max - min));
                }

                if (rF == max)
                {
                    h = (gF - bF) / delta;
                }
                else if (gF == max)
                {
                    h = 2f + ((bF - rF) / delta);
                }
                else if (bF == max)
                {
                    h = 4f + ((rF - gF) / delta);
                }
            }

            return new ColorDetection.MMCQ.HSLColor(h, s, l);
        }

        /// <summary>
        /// Analyzes a bitmap image into a color palette.
        /// </summary>
        /// <param name="image">Bitmap Image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Hight of the image.</param>
        public async void Analyse(BitmapImage image, int width = 96, int height = 66)
        {
            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(image.UriSource);
            IRandomAccessStream str = await random.OpenReadAsync();
            Analyse(str.AsStreamForRead(), width, height);
            str.Dispose();
        }

        /// <summary>
        /// Analyses a <see cref="Stream"/> of an image into a color palette.
        /// </summary>
        /// <param name="str">Image stream.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        public async void Analyse(Stream str, int width = 96, int height = 66)
        {
            WriteableBitmap wbm = new WriteableBitmap(width, height);
            await wbm.SetSourceAsync(str.AsRandomAccessStream());
            str.Dispose();
            Debug.WriteLine(DateTime.Now + "GOT IMAGE");

            List<MMCQ.QuantizedColor> palette1 = ColorThief.GetPalette(wbm, 12, 4, true);
            List<MMCQ.QuantizedColor> palette2 = new List<MMCQ.QuantizedColor>();
            List<MMCQ.QuantizedColor> palette3 = new List<MMCQ.QuantizedColor>();

            foreach (var v in palette1)
            {
                var hsl = FromRGB(v.Color.R, v.Color.G, v.Color.B);
                v.hsl = new MMCQ.HSLColor((float)Math.Round(hsl.H, 3), (float)Math.Round(hsl.S, 3), (float)Math.Round(hsl.L, 3));
                if (hsl.L > 0.35)
                {
                    palette2.Add(v);
                }
            }

            palette2 = palette2.OrderBy(x => x.hsl.S).Reverse().ToList();
            if (palette2.Count > 6)
            {
                palette2.RemoveRange(6, palette2.Count - 6);
            }

            ColorList = palette2;

            palette3.Add(palette2.First());
            palette3.Add(palette2.Last());

            var color = palette2.First();
            var color2 = palette2.ElementAt(1);
            palette3.Add(palette2.First());
            foreach (var c in palette2)
            {
                Debug.WriteLine("HSL.S (" + c.hsl.S + ") > 0.1");
                var dif1 = Math.Abs(color.hsl.H - color2.hsl.H);
                var dif2 = Math.Abs(color.hsl.H - c.hsl.H);
                if (dif2 > dif1 && c.hsl.S > 0.1)
                {
                    color2 = c;
                }
            }

            if (color2.hsl.L > color.hsl.L)
            {
                var dif = Math.Abs(color.hsl.L - color2.hsl.L);
                if (dif > 0.2)
                {
                    var altcolor = color;
                    color = color2;
                    color2 = altcolor;
                }
            }

            color.name = "Accent";
            color2.name = "Secondary";
            Color newcolor2 = color2.Color;
            if (color2.hsl.L < 0.5)
            {
                newcolor2 = Color.FromArgb(255, Convert.ToByte(color2.Color.R + 15), Convert.ToByte(color2.Color.G + 15), Convert.ToByte(color2.Color.B + 15));
            }

            Debug.WriteLine("Accent color HSL.L = " + color.hsl.L.ToString());
            Debug.WriteLine("Accent color HSL.S = " + color.hsl.S.ToString());

            Debug.WriteLine("Secondary color HSL.L = " + color2.hsl.L.ToString());
            Debug.WriteLine("Secondary color HSL.S = " + color2.hsl.S.ToString());
        }
    }
}
