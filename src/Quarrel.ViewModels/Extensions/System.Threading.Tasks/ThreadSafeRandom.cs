// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

namespace System.Threading.Tasks
{
    /// <summary>
    /// A <see langword="static"/> <see langword="class"/> that provides thread safe access to shared <see cref="Random"/> instances.
    /// </summary>
    public static class ThreadSafeRandom
    {
        /// <summary>
        /// Thread local provider of <see cref="Random"/> instances.
        /// </summary>
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _ticks)));

        /// <summary>
        /// Incremental seed for the <see cref="Random"/> instances.
        /// </summary>
        private static int _ticks = Environment.TickCount;

        /// <summary>
        /// Gets a singleton, thread local <see cref="Random"/> instance.
        /// </summary>
        public static Random Instance => _random.Value;
    }
}
