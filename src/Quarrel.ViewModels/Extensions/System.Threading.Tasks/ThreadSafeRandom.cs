// Copyright (c) Quarrel. All rights reserved.

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
