// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.Helpers.AudioProcessing
{
    /// <summary>
    /// Adds HelperMethods for audio processing.
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Gets the FFT of raw pcm data.
        /// </summary>
        /// <param name="pcmData">The raw pcm data.</param>
        /// <returns>The FFT data from the <paramref name="pcmData"/>.</returns>
        public static float[] GetFftChannelData(float[] pcmData)
        {
            Complex[] fftData = new Complex[512];
            for (int j = 0; j < fftData.Length; j++)
            {
                Complex c = default;
                c.Re = pcmData[j] * (float)FFT.HammingWindow(j, fftData.Length);
                fftData[j] = c;
            }

            FFT.RunFFT(fftData, FFT.Direction.Forward);
            float[] fftResult = new float[512];
            for (int j = 0; j < fftResult.Length; j++)
            {
                fftResult[j] = Math.Abs((float)Math.Sqrt(Math.Pow(fftData[j].Re, 2) + Math.Pow(fftData[j].Im, 2)));
            }

            return fftResult;
        }

        /// <summary>
        /// Finds the max value in the array.
        /// </summary>
        /// <param name="array">Array to check.</param>
        /// <param name="start">Position to start at.</param>
        /// <param name="stop">Position to stop at.</param>
        /// <returns>The largest value in the array between <paramref name="start"/> and <paramref name="stop"/> indexes.</returns>
        public static float Max(float[] array, int start, int stop)
        {
            float max = 0;
            for (int i = start; i < stop; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Calculates power of 2.
        /// </summary>
        /// <param name="power">Power to raise in.</param>
        /// <returns>Returns specified power of 2 in the case if power is in the range of
        /// [0, 30]. Otherwise returns 0.</returns>
        public static int Pow2(int power)
        {
            return ((power >= 0) && (power <= 30)) ? (1 << power) : 0;
        }

        /// <summary>
        /// Checks if the specified integer is power of 2.
        /// </summary>
        /// <param name="x">Integer number to check.</param>
        /// <returns>Returns <b>true</b> if the specified number is power of 2.
        /// Otherwise returns <b>false</b>.</returns>
        public static bool IsPowerOf2(int x)
        {
            return (x > 0) ? ((x & (x - 1)) == 0) : false;
        }

        /// <summary>
        /// Get base of binary logarithm.
        /// </summary>
        /// <param name="x">Source integer number.</param>
        /// <returns>Power of the number (base of binary logarithm).</returns>
        public static int Log2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                {
                                    return 0;
                                }

                                return 1;
                            }

                            return 2;
                        }

                        if (x <= 8)
                        {
                            return 3;
                        }

                        return 4;
                    }

                    if (x <= 64)
                    {
                        if (x <= 32)
                        {
                            return 5;
                        }

                        return 6;
                    }

                    if (x <= 128)
                    {
                        return 7;
                    }

                    return 8;
                }

                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                        {
                            return 9;
                        }

                        return 10;
                    }

                    if (x <= 2048)
                    {
                        return 11;
                    }

                    return 12;
                }

                if (x <= 16384)
                {
                    if (x <= 8192)
                    {
                        return 13;
                    }

                    return 14;
                }

                if (x <= 32768)
                {
                    return 15;
                }

                return 16;
            }

            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                        {
                            return 17;
                        }

                        return 18;
                    }

                    if (x <= 524288)
                    {
                        return 19;
                    }

                    return 20;
                }

                if (x <= 4194304)
                {
                    if (x <= 2097152)
                    {
                        return 21;
                    }

                    return 22;
                }

                if (x <= 8388608)
                {
                    return 23;
                }

                return 24;
            }

            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                    {
                        return 25;
                    }

                    return 26;
                }

                if (x <= 134217728)
                {
                    return 27;
                }

                return 28;
            }

            if (x <= 1073741824)
            {
                if (x <= 536870912)
                {
                    return 29;
                }

                return 30;
            }

            return 31;
        }
    }
}
