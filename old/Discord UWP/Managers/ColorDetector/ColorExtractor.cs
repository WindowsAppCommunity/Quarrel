using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static Quarrel.SmartColor.ColorDetection;

namespace Quarrel.SmartColor
{
    public class PictureAnalysis
    {
        public delegate void AnalysedHandler(object sender, bool IsLight);
        public event AnalysedHandler Analysed;

        //Find dominant colors from poster, and update the UI accordingly
        public List<ColorDetection.MMCQ.QuantizedColor> ColorList = new List<ColorDetection.MMCQ.QuantizedColor>();
        public async Task Analyse(StorageFile file, int width = 96, int height = 66)
        {
            var thumb = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView, 96, Windows.Storage.FileProperties.ThumbnailOptions.ResizeThumbnail);

            BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.SetSource(thumb);
            var str = thumb.AsStreamForRead();
            await Analyse(str, width, height);
            str.Dispose();
        }
        public async Task Analyse(BitmapImage image, int width = 96, int height = 66)
        {
            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(image.UriSour‌​ce);
            IRandomAccessStream str = await random.OpenReadAsync();
            await Analyse(str.AsStreamForRead(), width, height);
            str.Dispose();
        }
        public async Task Analyse(Stream str, int width = 96, int height=66)
        {
            WriteableBitmap wbm = new WriteableBitmap(width, height);
            await wbm.SetSourceAsync(str.AsRandomAccessStream());
            str.Dispose();
            Debug.WriteLine(DateTime.Now +  "GOT IMAGE");
            List<ColorDetection.MMCQ.QuantizedColor> Palette1 = ColorDetection.ColorThief.GetPalette(wbm,12, 4, true);
            List<ColorDetection.MMCQ.QuantizedColor> Palette2 = new List<ColorDetection.MMCQ.QuantizedColor>();
            List<ColorDetection.MMCQ.QuantizedColor> Palette3 = new List<ColorDetection.MMCQ.QuantizedColor>();
            
            foreach (var v in Palette1)
            {
                var hsl = FromRGB(v.Color.R,v.Color.G,v.Color.B);
                v.hsl = new ColorDetection.MMCQ.HSLColor((float)Math.Round(hsl.H, 3), (float)Math.Round(hsl.S,3), (float)Math.Round(hsl.L, 3));
                if(hsl.L>0.35)
                Palette2.Add(v);
            }
            bool IsLight = false;

            Palette2 = Palette2.OrderBy(x => x.hsl.S).Reverse().ToList();
            if (Palette2.Count > 6)
                Palette2.RemoveRange(6, Palette2.Count - 6);
            ColorList = Palette2;

            Palette3.Add(Palette2.First());
            Palette3.Add(Palette2.Last());

            var color = Palette2.First();
            var color2 = Palette2.ElementAt(1);
            Palette3.Add(Palette2.First());
            foreach (var c in Palette2)
            {

                    Debug.WriteLine("HSL.S (" + c.hsl.S + ") > 0.1");
                    var dif1 = Math.Abs(color.hsl.H - color2.hsl.H);
                    var dif2 = Math.Abs(color.hsl.H - c.hsl.H);
                    if (dif2 > dif1 && c.hsl.S>0.1)
                    {
                        color2 = c;
                    }
            }
            if(color2.hsl.L>color.hsl.L)
            {
                var dif = Math.Abs(color.hsl.L - color2.hsl.L);
                if(dif>0.2)
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
                newcolor2 = Color.FromArgb(255, Convert.ToByte(color2.Color.R + 15), Convert.ToByte(color2.Color.G + 15), Convert.ToByte(color2.Color.B + 15));

            Debug.WriteLine("Accent color HSL.L = " + color.hsl.L.ToString());
            Debug.WriteLine("Accent color HSL.S = " + color.hsl.S.ToString());

            Debug.WriteLine("Secondary color HSL.L = " + color2.hsl.L.ToString());
            Debug.WriteLine("Secondary color HSL.S = " + color2.hsl.S.ToString());
            //Analysed(null, IsLight);
        }

        //Convert HSL color to RGB (for Poster_Loaded)
        public static ColorDetection.MMCQ.HSLColor FromRGB(Byte R, Byte G, Byte B)
        {
            float _R = (R / 255f);
            float _G = (G / 255f);
            float _B = (B / 255f);

            float _Min = Math.Min(Math.Min(_R, _G), _B);
            float _Max = Math.Max(Math.Max(_R, _G), _B);
            float _Delta = _Max - _Min;

            float H = 0;
            float S = 0;
            float L = (float)((_Max + _Min) / 2.0f);

            if (_Delta != 0)
            {
                if (L < 0.5f)
                {
                    S = (float)(_Delta / (_Max + _Min));
                }
                else
                {
                    S = (float)(_Delta / (2.0f - _Max - _Min));
                }


                if (_R == _Max)
                {
                    H = (_G - _B) / _Delta;
                }
                else if (_G == _Max)
                {
                    H = 2f + (_B - _R) / _Delta;
                }
                else if (_B == _Max)
                {
                    H = 4f + (_R - _G) / _Delta;
                }
            }

            return new ColorDetection.MMCQ.HSLColor(H, S, L);
        }

    }

}
