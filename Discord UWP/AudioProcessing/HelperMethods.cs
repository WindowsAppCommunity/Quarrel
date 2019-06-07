using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;

namespace FFT.Processing
{
    public static class HelperMethods
    {
        public static double NextDouble(this Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        unsafe public static List<float[]> ProcessFrameOutput(AudioFrame frame)
        {
            using (AudioBuffer audioBuffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = audioBuffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);
                dataInFloat = (float*)dataInBytes;
                uint dataInFloatLength = audioBuffer.Length / sizeof(float);
                List<float[]> channelData = new List<float[]>();
                float[] leftChannel = new float[dataInFloatLength / 2];
                float[] rightChannel = new float[dataInFloatLength / 2];
                channelData.Add(leftChannel);
                channelData.Add(rightChannel);
                if (dataInFloatLength > 0)
                {
                    int channelCount = 0;
                    for (int i = 0; i < dataInFloatLength; i++)
                    {
                        float datum = dataInFloat[i];
                        if (i % 2 == 0)
                        {
                            leftChannel[channelCount] = datum;
                        }
                        else
                        {
                            rightChannel[channelCount] = datum;
                            channelCount++;
                        }
                    }
                }
                return channelData;
            }
        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        public static List<float[]> ConvertTo512(List<float[]> channelData, AudioGraph audioGraph)
        {
            List<float[]> newChannelData = new List<float[]>();
            float[] leftChannel = channelData[0];
            float[] rightChannel = channelData[1];
            for (int i = 0; i < leftChannel.Length / audioGraph.SamplesPerQuantum; i++)
            {
                float[] tmpLeftChannelData = new float[512];
                float[] tmpRightChannelData = new float[512];

                // copy the left and right channel data into a new array
                for (int j = i * audioGraph.SamplesPerQuantum; j < (i + 1) * audioGraph.SamplesPerQuantum; j++)
                {
                    tmpLeftChannelData[j % audioGraph.SamplesPerQuantum] = leftChannel[j];
                    tmpRightChannelData[j % audioGraph.SamplesPerQuantum] = rightChannel[j];
                }

                // then pad the rest with 0s till we get to 512
                for (int j = audioGraph.SamplesPerQuantum; j < 512; j++)
                {
                    tmpLeftChannelData[j] = 0;
                    tmpRightChannelData[j] = 0;
                }
                newChannelData.Add(tmpLeftChannelData);
                newChannelData.Add(tmpRightChannelData);
            }
            return newChannelData;
        }

        public static List<float[]> GetFftData(List<float[]> channelData, AudioGraph audioGraph)
        {
            List<float[]> fftData = new List<float[]>();
            for (int i = 0; i < channelData.Count / 2; i++)
            {
                float[] leftChannel = GetFftChannelData(channelData[i], audioGraph);
                float[] rightChannel = GetFftChannelData(channelData[i + 1], audioGraph);
                fftData.Add(leftChannel);
                fftData.Add(rightChannel);
            }
            return fftData;
        }

        public static float[] GetFftChannelData(float[] channelData, AudioGraph audioGraph)
        {
            Complex[] fftData = new Complex[512];
            for (int j = 0; j < fftData.Length; j++)
            {
                Complex c = new Complex();
                c.Re = channelData[j] * (float)Fft.HammingWindow(j, fftData.Length);
                fftData[j] = c;
            }
            Fft.FFT(fftData, Fft.Direction.Forward);
            float[] fftResult = new float[audioGraph.SamplesPerQuantum / 2];
            for (int j = 0; j < fftResult.Length; j++)
            {
                fftResult[j] = Math.Abs((float)Math.Sqrt(Math.Pow(fftData[j].Re, 2) + Math.Pow(fftData[j].Im, 2)));
            }
            return fftResult;
        }

        public static float Average(float[] array, int start, int stop)
        {
            float average = 0;
            for (int i = start; i < stop; i++)
            {
                average += array[i];
            }
            return average / (stop - start);
        }
        public static float Max(float[] array, int start, int stop)
        {
            float max = 0;
            for (int i = start; i < stop; i++)
            {
                if (max < array[i])
                    max = array[i];
            }
            return max;
        }
        public static double AverageFreq(float[] array, int start, int stop)
        {
            float total = 0;
            double fullcount = 0;
            for (int i = start; i < stop; i++)
            {
                float val = array[i];
                fullcount += val;
                total += val * i;
            }
            return total/fullcount;
        }
        /// <summary>
        /// Partitions the given list around a pivot element such that all elements on left of pivot are <= pivot
        /// and the ones at thr right are > pivot. This method can be used for sorting, N-order statistics such as
        /// as median finding algorithms.
        /// Pivot is selected ranodmly if random number generator is supplied else its selected as last element in the list.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
        /// </summary>
        private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
        {
            if (rnd != null)
                list.Swap(end, rnd.Next(start, end + 1));

            var pivot = list[end];
            var lastLow = start - 1;
            for (var i = start; i < end; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                    list.Swap(i, ++lastLow);
            }
            list.Swap(end, ++lastLow);
            return lastLow;
        }

        /// <summary>
        /// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </summary>
        public static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
        {
            return NthOrderStatistic(list, n, 0, list.Count - 1, rnd);
        }
        private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
        {
            while (true)
            {
                var pivotIndex = list.Partition(start, end, rnd);
                if (pivotIndex == n)
                    return list[pivotIndex];

                if (n < pivotIndex)
                    end = pivotIndex - 1;
                else
                    start = pivotIndex + 1;
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            if (i == j)   //This check is not required but Partition function may make many calls so its for perf reason
                return;
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Note: specified list would be mutated in the process.
        /// </summary>
        public static T Median<T>(this IList<T> list) where T : IComparable<T>
        {
            return list.NthOrderStatistic((list.Count - 1) / 2);
        }

        public static double Median<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var list = sequence.Select(getValue).ToList();
            var mid = (list.Count - 1) / 2;
            return list.NthOrderStatistic(mid);
        }
        public static float Average(float[] array, int start)
        {
            return Average(array, start, array.Length);
        }

        public static float Average(ICollection<float> collection)
        {
            float average = 0;
            foreach (float value in collection)
            {
                average += value;
            }
            return average / collection.Count;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Calculates power of 2.
        /// </summary>
        /// 
        /// <param name="power">Power to raise in.</param>
        /// 
        /// <returns>Returns specified power of 2 in the case if power is in the range of
        /// [0, 30]. Otherwise returns 0.</returns>
        /// 
        public static int Pow2(int power)
        {
            return ((power >= 0) && (power <= 30)) ? (1 << power) : 0;
        }

        /// <summary>
        /// Checks if the specified integer is power of 2.
        /// </summary>
        /// 
        /// <param name="x">Integer number to check.</param>
        /// 
        /// <returns>Returns <b>true</b> if the specified number is power of 2.
        /// Otherwise returns <b>false</b>.</returns>
        /// 
        public static bool IsPowerOf2(int x)
        {
            return (x > 0) ? ((x & (x - 1)) == 0) : false;
        }

        /// <summary>
        /// Get base of binary logarithm.
        /// </summary>
        /// 
        /// <param name="x">Source integer number.</param>
        /// 
        /// <returns>Power of the number (base of binary logarithm).</returns>
        /// 
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
                                    return 0;
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }
                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }

            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            return 31;
        }
    }
}
