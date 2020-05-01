// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace QuarrelSmartColor
{
    /// <summary>
    /// Defines classes used for color detection.
    /// </summary>
    public class ColorDetection
    {
        public class ColorThief
        {
            private const int DefaultColorCount = 5;
            private const int DefaultQuality = 10;
            private const bool DefaultIgnoreWhite = true;

            /// <summary>
            ///     Use the median cut algorithm to cluster similar colors and return the base color from the largest cluster.
            /// </summary>
            /// <param name="sourceImage">The source image.</param>
            /// <param name="quality">
            ///     0 is the highest quality settings. 10 is the default. There is
            ///     a trade-off between quality and speed. The bigger the number,
            ///     the faster a color will be returned but the greater the
            ///     likelihood that it will not be the visually most dominant color.
            /// </param>
            /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
            /// <returns>The most prominent color.</returns>
            public static MMCQ.QuantizedColor GetColor(WriteableBitmap sourceImage, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
            {
                var palette = GetPalette(sourceImage, DefaultColorCount, quality, ignoreWhite);
                var dominantColor = palette?[0];
                return dominantColor;
            }

            /// <summary>
            ///     Use the median cut algorithm to cluster similar colors.
            /// </summary>
            /// <param name="sourceImage">The source image.</param>
            /// <param name="colorCount">The color count.</param>
            /// <param name="quality">
            ///     0 is the highest quality settings. 10 is the default. There is
            ///     a trade-off between quality and speed. The bigger the number,
            ///     the faster a color will be returned but the greater the
            ///     likelihood that it will not be the visually most dominant color.
            /// </param>
            /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
            /// <returns></returns>
            /// <code>true</code>
            public static List<MMCQ.QuantizedColor> GetPalette(
                WriteableBitmap sourceImage,
                int colorCount = DefaultColorCount,
                int quality = DefaultQuality,
                bool ignoreWhite = DefaultIgnoreWhite)
            {
                var cmap = GetColorMap(sourceImage, colorCount, quality, ignoreWhite);
                return cmap?.GeneratePalette();
            }

            /// <summary>
            ///     Use the median cut algorithm to cluster similar colors.
            /// </summary>
            /// <param name="sourceImage">The source image.</param>
            /// <param name="colorCount">The color count.</param>
            /// <returns></returns>
            public static MMCQ.CMap GetColorMap(WriteableBitmap sourceImage, int colorCount)
            {
                return GetColorMap(
                    sourceImage,
                    colorCount,
                    DefaultQuality,
                    DefaultIgnoreWhite);
            }

            /// <summary>
            ///     Use the median cut algorithm to cluster similar colors.
            /// </summary>
            /// <param name="sourceImage">The source image.</param>
            /// <param name="colorCount">The color count.</param>
            /// <param name="quality">
            ///     0 is the highest quality settings. 10 is the default. There is
            ///     a trade-off between quality and speed. The bigger the number,
            ///     the faster a color will be returned but the greater the
            ///     likelihood that it will not be the visually most dominant color.
            /// </param>
            /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
            /// <returns></returns>
            public static MMCQ.CMap GetColorMap(
                WriteableBitmap sourceImage,
                int colorCount,
                int quality,
                bool ignoreWhite)
            {
                var pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);

                // Send array to quantize function which clusters values using median
                // cut algorithm
                var cmap = MMCQ.Quantize(pixelArray, colorCount);
                return cmap;
            }

            private static int[][] GetPixelsFast(
                WriteableBitmap sourceImage,
                int quality,
                bool ignoreWhite)
            {
                var imageData = sourceImage.PixelBuffer;
                var pixels = imageData.ToArray();
                var pixelCount = sourceImage.PixelWidth * sourceImage.PixelHeight;

                var colorDepth = 4;

                var expectedDataLength = pixelCount * colorDepth;
                if (expectedDataLength != pixels.Length)
                {
                    throw new ArgumentException("(expectedDataLength = "
                                                + expectedDataLength + ") != (pixels.length = "
                                                + pixels.Length + ")");
                }

                // Store the RGB values in an array format suitable for quantize
                // function

                // numRegardedPixels must be rounded up to avoid an
                // ArrayIndexOutOfBoundsException if all pixels are good.
                var numRegardedPixels = (pixelCount + quality - 1) / quality;

                var numUsedPixels = 0;
                var pixelArray = new int[numRegardedPixels][];


                for (var i = 0; i < pixelCount; i += quality)
                {
                    var offset = i * 4;
                    int b = pixels[offset];
                    int g = pixels[offset + 1];
                    int r = pixels[offset + 2];
                    int a = pixels[offset + 3];

                    // If pixel is mostly opaque and not white
                    if (a >= 125 && !(ignoreWhite && r > 250 && g > 250 && b > 250))
                    {
                        pixelArray[numUsedPixels] = new[] { r, g, b };
                        numUsedPixels++;
                    }
                }

                // Remove unused pixels from the array
                var copy = new int[numUsedPixels][];
                Array.Copy(pixelArray, copy, numUsedPixels);
                return copy;
            }
        }

        public class MMCQ
        {
            private const int Sigbits = 5;
            private const int Rshift = 8 - Sigbits;
            private const int Mult = 1 << Rshift;
            private const int Histosize = 1 << (3 * Sigbits);
            private const int VboxLength = 1 << Sigbits;
            private const double FractByPopulation = 0.75;
            private const int MaxIterations = 1000;
            private const double WeightSaturation = 3f;
            private const double WeightLuma = 6f;
            private const double WeightPopulation = 1f;
            private static readonly VBoxComparer ComparatorProduct = new VBoxComparer();
            private static readonly VBoxCountComparer ComparatorCount = new VBoxCountComparer();

            private static int GetColorIndex(int r, int g, int b)
            {
                return (r << (2 * Sigbits)) + (g << Sigbits) + b;
            }

            /// <summary>
            ///     Gets the histo.
            /// </summary>
            /// <param name="pixels">The pixels.</param>
            /// <returns>Histo (1-d array, giving the number of pixels in each quantized region of color space), or null on error.</returns>
            private static int[] GetHisto(IReadOnlyList<int[]> pixels)
            {
                var histo = new int[Histosize];

                var numPixels = pixels.Count;
                for (var i = 0; i < numPixels; i++)
                {
                    var pixel = pixels[i];
                    var rval = pixel[0] >> Rshift;
                    var gval = pixel[1] >> Rshift;
                    var bval = pixel[2] >> Rshift;
                    var index = GetColorIndex(rval, gval, bval);
                    histo[index]++;
                }
                return histo;
            }

            private static VBox VboxFromPixels(IReadOnlyList<int[]> pixels, int[] histo)
            {
                int rmin = 1000000, rmax = 0;
                int gmin = 1000000, gmax = 0;
                int bmin = 1000000, bmax = 0;

                // find min/max
                var numPixels = pixels.Count;
                for (var i = 0; i < numPixels; i++)
                {
                    var pixel = pixels[i];
                    var rval = pixel[0] >> Rshift;
                    var gval = pixel[1] >> Rshift;
                    var bval = pixel[2] >> Rshift;

                    if (rval < rmin)
                    {
                        rmin = rval;
                    }
                    else if (rval > rmax)
                    {
                        rmax = rval;
                    }

                    if (gval < gmin)
                    {
                        gmin = gval;
                    }
                    else if (gval > gmax)
                    {
                        gmax = gval;
                    }

                    if (bval < bmin)
                    {
                        bmin = bval;
                    }
                    else if (bval > bmax)
                    {
                        bmax = bval;
                    }
                }

                return new VBox(rmin, rmax, gmin, gmax, bmin, bmax, histo);
            }

            private static VBox[] DoCut(
                char color,
                VBox vbox,
                IReadOnlyList<int> partialsum,
                IReadOnlyList<int> lookaheadsum,
                int total)
            {
                int vboxDim1;
                int vboxDim2;

                switch (color)
                {
                    case 'r':
                        vboxDim1 = vbox.R1;
                        vboxDim2 = vbox.R2;
                        break;
                    case 'g':
                        vboxDim1 = vbox.G1;
                        vboxDim2 = vbox.G2;
                        break;
                    default:
                        vboxDim1 = vbox.B1;
                        vboxDim2 = vbox.B2;
                        break;
                }

                for (var i = vboxDim1; i <= vboxDim2; i++)
                {
                    if (partialsum[i] > total / 2)
                    {
                        var vbox1 = vbox.Clone();
                        var vbox2 = vbox.Clone();

                        var left = i - vboxDim1;
                        var right = vboxDim2 - i;

                        var d2 = left <= right
                            ? Math.Min(vboxDim2 - 1, ~~(i + (right / 2)))
                            : Math.Max(vboxDim1, ~~((int)(i - 1 - (left / 2.0))));

                        // avoid 0-count boxes
                        while (d2 < 0 || partialsum[d2] <= 0)
                        {
                            d2++;
                        }

                        var count2 = lookaheadsum[d2];
                        while (count2 == 0 && d2 > 0 && partialsum[d2 - 1] > 0)
                        {
                            count2 = lookaheadsum[--d2];
                        }

                        // set dimensions
                        switch (color)
                        {
                            case 'r':
                                vbox1.R2 = d2;
                                vbox2.R1 = d2 + 1;
                                break;
                            case 'g':
                                vbox1.G2 = d2;
                                vbox2.G1 = d2 + 1;
                                break;
                            default:
                                vbox1.B2 = d2;
                                vbox2.B1 = d2 + 1;
                                break;
                        }

                        return new[] { vbox1, vbox2 };
                    }
                }

                throw new Exception("VBox can't be cut");
            }

            private static VBox[] MedianCutApply(IReadOnlyList<int> histo, VBox vbox)
            {
                if (vbox.Count(false) == 0)
                {
                    return null;
                }

                if (vbox.Count(false) == 1)
                {
                    return new[] { vbox.Clone(), null };
                }

                // only one pixel, no split
                var rw = vbox.R2 - vbox.R1 + 1;
                var gw = vbox.G2 - vbox.G1 + 1;
                var bw = vbox.B2 - vbox.B1 + 1;
                var maxw = Math.Max(Math.Max(rw, gw), bw);

                // Find the partial sum arrays along the selected axis.
                var total = 0;
                var partialsum = new int[VboxLength];

                // -1 = not set / 0 = 0
                for (var l = 0; l < partialsum.Length; l++)
                {
                    partialsum[l] = -1;
                }

                // -1 = not set / 0 = 0
                var lookaheadsum = new int[VboxLength];
                for (var l = 0; l < lookaheadsum.Length; l++)
                {
                    lookaheadsum[l] = -1;
                }

                int i, j, k, sum, index;

                if (maxw == rw)
                {
                    for (i = vbox.R1; i <= vbox.R2; i++)
                    {
                        sum = 0;
                        for (j = vbox.G1; j <= vbox.G2; j++)
                        {
                            for (k = vbox.B1; k <= vbox.B2; k++)
                            {
                                index = GetColorIndex(i, j, k);
                                sum += histo[index];
                            }
                        }

                        total += sum;
                        partialsum[i] = total;
                    }
                }
                else if (maxw == gw)
                {
                    for (i = vbox.G1; i <= vbox.G2; i++)
                    {
                        sum = 0;
                        for (j = vbox.R1; j <= vbox.R2; j++)
                        {
                            for (k = vbox.B1; k <= vbox.B2; k++)
                            {
                                index = GetColorIndex(j, i, k);
                                sum += histo[index];
                            }
                        }

                        total += sum;
                        partialsum[i] = total;
                    }
                }
                else
                {
                    for (i = vbox.B1; i <= vbox.B2; i++)
                    {
                        sum = 0;
                        for (j = vbox.R1; j <= vbox.R2; j++)
                        {
                            for (k = vbox.G1; k <= vbox.G2; k++)
                            {
                                index = GetColorIndex(j, k, i);
                                sum += histo[index];
                            }
                        }

                        total += sum;
                        partialsum[i] = total;
                    }
                }

                for (i = 0; i < VboxLength; i++)
                {
                    if (partialsum[i] != -1)
                    {
                        lookaheadsum[i] = total - partialsum[i];
                    }
                }

                // determine the cut planes
                return maxw == rw
                    ? DoCut('r', vbox, partialsum, lookaheadsum, total)
                    : maxw == gw
                        ? DoCut('g', vbox, partialsum, lookaheadsum, total)
                        : DoCut('b', vbox, partialsum, lookaheadsum, total);
            }

            /// <summary>
            ///     Inner function to do the iteration.
            /// </summary>
            /// <param name="lh">The lh.</param>
            /// <param name="comparator">The comparator.</param>
            /// <param name="target">The target.</param>
            /// <param name="histo">The histo.</param>
            /// <exception cref="System.Exception">vbox1 not defined; shouldn't happen.</exception>
            private static void Iter(
                List<VBox> lh,
                IComparer<VBox> comparator,
                int target,
                IReadOnlyList<int> histo)
            {
                var ncolors = 1;
                var niters = 0;

                while (niters < MaxIterations)
                {
                    var vbox = lh[lh.Count - 1];
                    if (vbox.Count(false) == 0)
                    {
                        lh.Sort(comparator);
                        niters++;
                        continue;
                    }

                    lh.RemoveAt(lh.Count - 1);

                    // do the cut
                    var vboxes = MedianCutApply(histo, vbox);
                    var vbox1 = vboxes[0];
                    var vbox2 = vboxes[1];

                    if (vbox1 == null)
                    {
                        throw new Exception(
                            "vbox1 not defined; shouldn't happen!");
                    }

                    lh.Add(vbox1);
                    if (vbox2 != null)
                    {
                        lh.Add(vbox2);
                        ncolors++;
                    }

                    lh.Sort(comparator);

                    if (ncolors >= target)
                    {
                        return;
                    }

                    if (niters++ > MaxIterations)
                    {
                        return;
                    }
                }
            }

            public static CMap Quantize(int[][] pixels, int maxcolors)
            {
                // short-circuit
                if (pixels.Length == 0 || maxcolors < 2 || maxcolors > 256)
                {
                    return null;
                }

                var histo = GetHisto(pixels);

                // get the beginning vbox from the colors
                var vbox = VboxFromPixels(pixels, histo);
                var pq = new List<VBox> { vbox };

                // Round up to have the same behaviour as in JavaScript
                var target = (int)Math.Ceiling(FractByPopulation * maxcolors);

                // first set of colors, sorted by population
                Iter(pq, ComparatorCount, target, histo);

                // Re-sort by the product of pixel occupancy times the size in color
                // space.
                pq.Sort(ComparatorProduct);

                // next set - generate the median cuts using the (npix * vol) sorting.
                Iter(pq, ComparatorProduct, maxcolors - pq.Count, histo);

                // Reverse to put the highest elements first into the color map
                pq.Reverse();

                // calculate the actual colors
                var cmap = new CMap();
                foreach (var vb in pq)
                {
                    cmap.Push(vb);
                }

                return cmap;
            }

            private static double CreateComparisonValue(
                double saturation,
                double targetSaturation,
                double luma,
                double targetLuma,
                int population,
                int highestPopulation)
            {
                return WeightedMean(
                    InvertDiff(saturation, targetSaturation),
                    WeightSaturation,
                    InvertDiff(luma, targetLuma),
                    WeightLuma,
                    population / (double)highestPopulation,
                    WeightPopulation);
            }

            private static double WeightedMean(params double[] values)
            {
                double sum = 0;
                double sumWeight = 0;

                for (var i = 0; i < values.Length; i += 2)
                {
                    var value = values[i];
                    var weight = values[i + 1];

                    sum += value * weight;
                    sumWeight += weight;
                }

                return sum / sumWeight;
            }

            private static double InvertDiff(double value, double targetValue)
            {
                return 1 - Math.Abs(value - targetValue);
            }

            /// <summary>
            /// 3D color space box.
            /// </summary>
            public class VBox
            {
                private readonly int[] _histo;
                private int[] _avg;
                private int? _count;
                private int? _volume;

                public int B1 { get; set; }
                public int B2 { get; set; }
                public int G1 { get; set; }
                public int G2 { get; set; }
                public int R1 { get; set; }
                public int R2 { get; set; }

                public VBox(int r1, int r2, int g1, int g2, int b1, int b2, int[] histo)
                {
                    R1 = r1;
                    R2 = r2;
                    G1 = g1;
                    G2 = g2;
                    B1 = b1;
                    B2 = b2;

                    _histo = histo;
                }

                public int Volume(bool force)
                {
                    if (_volume == null || force)
                    {
                        _volume = ((R2 - R1 + 1) * (G2 - G1 + 1) * (B2 - B1 + 1));
                    }

                    return _volume.Value;
                }

                public int Count(bool force)
                {
                    if (_count == null || force)
                    {
                        var npix = 0;
                        int i;

                        for (i = R1; i <= R2; i++)
                        {
                            int j;
                            for (j = G1; j <= G2; j++)
                            {
                                int k;
                                for (k = B1; k <= B2; k++)
                                {
                                    var index = GetColorIndex(i, j, k);
                                    npix += _histo[index];
                                }
                            }
                        }

                        _count = npix;
                    }

                    return _count.Value;
                }

                public VBox Clone()
                {
                    return new VBox(R1, R2, G1, G2, B1, B2, _histo);
                }

                public int[] Avg(bool force)
                {
                    if (_avg == null || force)
                    {
                        var ntot = 0;

                        var rsum = 0;
                        var gsum = 0;
                        var bsum = 0;

                        int i;

                        for (i = R1; i <= R2; i++)
                        {
                            int j;
                            for (j = G1; j <= G2; j++)
                            {
                                int k;
                                for (k = B1; k <= B2; k++)
                                {
                                    var histoindex = GetColorIndex(i, j, k);
                                    var hval = _histo[histoindex];
                                    ntot += hval;
                                    rsum += (int)(hval * (i + 0.5) * Mult);
                                    gsum += (int)(hval * (j + 0.5) * Mult);
                                    bsum += (int)(hval * (k + 0.5) * Mult);
                                }
                            }
                        }

                        if (ntot > 0)
                        {
                            _avg = new[]
                            {
                            ~~(rsum/ntot), ~~(gsum/ntot),
                            ~~(bsum/ntot)
                        };
                        }
                        else
                        {
                            _avg = new[]
                            {
                            ~~(Mult*(R1 + R2 + 1)/2),
                            ~~(Mult*(G1 + G2 + 1)/2),
                            ~~(Mult*(B1 + B2 + 1)/2)
                        };
                        }
                    }

                    return _avg;
                }

                public bool Contains(int[] pixel)
                {
                    var rval = pixel[0] >> Rshift;
                    var gval = pixel[1] >> Rshift;
                    var bval = pixel[2] >> Rshift;

                    return (rval >= R1 && rval <= R2 && gval >= G1 && gval <= G2
                            && bval >= B1 && bval <= B2);
                }
            }

            public class QuantizedColor
            {
                public QuantizedColor(Color color, int population)
                {
                    Color = color;
                    Population = population;
                    IsDark = ColorUtility.CalculateYiqLuma(color) < 80;
                }

                public Color Color { get; }
                public HSLColor hsl { get; set; }
                public int Population { get; }
                public bool IsDark { get; }
                public string name { get; set; }
            }
            public class HSLColor
            {
                public HSLColor(float h, float s, float l)
                {
                    H = h;
                    S = s;
                    L = l;
                }

                public float H { get; set; }
                public float S { get; set; }
                public float L { get; set; }
            }
            /// <summary>
            /// Color map
            /// </summary>
            public class CMap
            {
                private readonly List<VBox> _vboxes = new List<VBox>();
                private List<QuantizedColor> _palette;

                public void Push(VBox box)
                {
                    _palette = null;
                    _vboxes.Add(box);
                }

                public List<QuantizedColor> GeneratePalette()
                {
                    return _palette ?? (_palette = (from vBox in _vboxes
                                                    let rgb = vBox.Avg(false)
                                                    let color = ColorUtility.FromRgb(rgb[0], rgb[1], rgb[2])
                                                    select new QuantizedColor(color, vBox.Count(false))).ToList());
                }

                public int Size()
                {
                    return _vboxes.Count;
                }

                public int[] Map(int[] color)
                {
                    var numVBoxes = _vboxes.Count;
                    for (var i = 0; i < numVBoxes; i++)
                    {
                        var vbox = _vboxes[i];
                        if (vbox.Contains(color))
                        {
                            return vbox.Avg(false);
                        }
                    }
                    return Nearest(color);
                }

                public int[] Nearest(int[] color)
                {
                    var d1 = double.MaxValue;
                    int[] pColor = null;

                    var numVBoxes = _vboxes.Count;
                    for (var i = 0; i < numVBoxes; i++)
                    {
                        var vbColor = _vboxes[i].Avg(false);
                        var d2 = Math.Sqrt(Math.Pow(color[0] - vbColor[0], 2)
                                           + Math.Pow(color[1] - vbColor[1], 2)
                                           + Math.Pow(color[2] - vbColor[2], 2));
                        if (d2 < d1)
                        {
                            d1 = d2;
                            pColor = vbColor;
                        }
                    }
                    return pColor;
                }

                public VBox FindColor(double targetLuma, double minLuma, double maxLuma,
                    double targetSaturation, double minSaturation, double maxSaturation)
                {
                    VBox max = null;
                    double maxValue = 0;
                    var highestPopulation = _vboxes.Select(p => p.Count(false)).Max();

                    foreach (var swatch in _vboxes)
                    {
                        var avg = swatch.Avg(false);
                        var hsl = ColorUtility.FromRgb(avg[0], avg[1], avg[2]).ToHsl();
                        var sat = hsl.S;
                        var luma = hsl.L;

                        if (sat >= minSaturation && sat <= maxSaturation &&
                            luma >= minLuma && luma <= maxLuma)
                        {
                            var thisValue = CreateComparisonValue(sat, targetSaturation, luma, targetLuma, swatch.Count(false), highestPopulation);
                            if (max == null || thisValue > maxValue)
                            {
                                max = swatch;
                                maxValue = thisValue;
                            }
                        }
                    }

                    return max;
                }
            }

            internal class VBoxCountComparer : IComparer<VBox>
            {
                public int Compare(VBox x, VBox y)
                {
                    var a = x.Count(false);
                    var b = y.Count(false);
                    return (a < b) ? -1 : ((a > b) ? 1 : 0);
                }
            }

            internal class VBoxComparer : IComparer<VBox>
            {
                public int Compare(VBox x, VBox y)
                {
                    var aCount = x.Count(false);
                    var bCount = y.Count(false);
                    var aVolume = x.Volume(false);
                    var bVolume = y.Volume(false);

                    // Otherwise sort by products
                    var a = aCount * aVolume;
                    var b = bCount * bVolume;
                    return (a < b) ? -1 : ((a > b) ? 1 : 0);
                }
            }
        }
    }
}
