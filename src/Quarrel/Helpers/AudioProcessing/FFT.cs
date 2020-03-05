// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.Helpers.AudioProcessing
{
    /// <summary>
    /// Runs a FFT.
    /// </summary>
    public static class FFT
    {
        private const int MinLength = 2;
        private const int MaxLength = 16384;
        private const int MinBits = 1;
        private const int MaxBits = 14;
        private static int[][] _reversedBits = new int[MaxBits][];
        private static Complex[,][] _complexRotation = new Complex[MaxBits, 2][];

        /// <summary>
        /// Fourier transformation direction.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Forward direction of Fourier transformation.
            /// </summary>
            Forward = 1,

            /// <summary>
            /// Backward direction of Fourier transformation.
            /// </summary>
            Backward = -1,
        }

        /// <summary>
        /// One dimensional Fast Fourier Transform.
        /// </summary>
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// <remarks><para><note>The method accepts <paramref name="data"/> array of 2<sup>n</sup> size
        /// only, where <b>n</b> may vary in the [1, 14] range.</note></para></remarks>
        /// <exception cref="ArgumentException">Incorrect data length.</exception>
        public static void RunFFT(Complex[] data, Direction direction)
        {
            int n = data.Length;
            int m = HelperMethods.Log2(n);

            // reorder data first
            ReorderData(data);

            // compute FFT
            int tn = 1, tm;

            for (int k = 1; k <= m; k++)
            {
                Complex[] rotation = GetComplexRotation(k, direction);

                tm = tn;
                tn <<= 1;

                for (int i = 0; i < tm; i++)
                {
                    Complex t = rotation[i];

                    for (int even = i; even < n; even += tn)
                    {
                        int odd = even + tm;
                        Complex ce = data[even];
                        Complex co = data[odd];

                        double tr = (co.Re * t.Re) - (co.Im * t.Im);
                        double ti = (co.Re * t.Im) + (co.Im * t.Re);

                        data[even].Re += tr;
                        data[even].Im += ti;

                        data[odd].Re = ce.Re - tr;
                        data[odd].Im = ce.Im - ti;
                    }
                }
            }

            if (direction == Direction.Forward)
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Re /= (double)n;
                    data[i].Im /= (double)n;
                }
            }
        }

        /// <summary>
        /// Applies a Hamming Window.
        /// </summary>
        /// <param name="n">Index into frame.</param>
        /// <param name="frameSize">Frame size (e.g. 1024).</param>
        /// <returns>Multiplier for Hamming window.</returns>
        public static double HammingWindow(int n, int frameSize)
        {
            return 0.54 - (0.46 * Math.Cos((2 * Math.PI * n) / (frameSize - 1)));
        }

        /// <summary>
        /// Applies a Hann Window.
        /// </summary>
        /// <param name="n">Index into frame.</param>
        /// <param name="frameSize">Frame size (e.g. 1024).</param>
        /// <returns>Multiplier for Hann window.</returns>
        public static double HannWindow(int n, int frameSize)
        {
            return 0.5 * (1 - Math.Cos((2 * Math.PI * n) / (frameSize - 1)));
        }

        /// <summary>
        /// Applies a Blackman-Harris Window.
        /// </summary>
        /// <param name="n">Index into frame.</param>
        /// <param name="frameSize">Frame size (e.g. 1024).</param>
        /// <returns>Multiplier for Blackmann-Harris window.</returns>
        public static double BlackmannHarrisWindow(int n, int frameSize)
        {
            return 0.35875 - (0.48829 * Math.Cos((2 * Math.PI * n) / (frameSize - 1))) + (0.14128 * Math.Cos((4 * Math.PI * n) / (frameSize - 1))) - (0.01168 * Math.Cos((6 * Math.PI * n) / (frameSize - 1)));
        }

        private static int[] GetReversedBits(int numberOfBits)
        {
            if ((numberOfBits < MinBits) || (numberOfBits > MaxBits))
            {
                throw new ArgumentOutOfRangeException();
            }

            // check if the array is already calculated
            if (_reversedBits[numberOfBits - 1] == null)
            {
                int n = HelperMethods.Pow2(numberOfBits);
                int[] rBits = new int[n];

                // calculate the array
                for (int i = 0; i < n; i++)
                {
                    int oldBits = i;
                    int newBits = 0;

                    for (int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits = oldBits >> 1;
                    }

                    rBits[i] = newBits;
                }

                _reversedBits[numberOfBits - 1] = rBits;
            }

            return _reversedBits[numberOfBits - 1];
        }

        private static Complex[] GetComplexRotation(int numberOfBits, Direction direction)
        {
            int directionIndex = (direction == Direction.Forward) ? 0 : 1;

            // check if the array is already calculated
            if (_complexRotation[numberOfBits - 1, directionIndex] == null)
            {
                int n = 1 << (numberOfBits - 1);
                double uR = 1.0;
                double uI = 0.0;
                double angle = System.Math.PI / n * (int)direction;
                double wR = System.Math.Cos(angle);
                double wI = System.Math.Sin(angle);
                double t;
                Complex[] rotation = new Complex[n];

                for (int i = 0; i < n; i++)
                {
                    rotation[i] = new Complex(uR, uI);
                    t = (uR * wI) + (uI * wR);
                    uR = (uR * wR) - (uI * wI);
                    uI = t;
                }

                _complexRotation[numberOfBits - 1, directionIndex] = rotation;
            }

            return _complexRotation[numberOfBits - 1, directionIndex];
        }

        /// <summary>
        /// Reorder data for FFT.
        /// </summary>
        /// <param name="data">Complex data to reorder.</param>
        private static void ReorderData(Complex[] data)
        {
            int len = data.Length;

            // check data length
            if ((len < MinLength) || (len > MaxLength) || (!HelperMethods.IsPowerOf2(len)))
            {
                throw new ArgumentException("Incorrect data length.");
            }

            int[] rBits = GetReversedBits(HelperMethods.Log2(len));

            for (int i = 0; i < len; i++)
            {
                int s = rBits[i];

                if (s > i)
                {
                    Complex t = data[i];
                    data[i] = data[s];
                    data[s] = t;
                }
            }
        }
    }
}
