// Credit to Sergio Pedri for extensions

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace System.Threading.Tasks
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="Task"/> type
    /// </summary>
    internal static partial class TaskExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task{T}"/> that either returns a <see cref="bool"/> value depending on if the given <see cref="CancellationToken"/> has expired
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to watch</param>
        /// <param name="token">The token to use to monitor the <see cref="Task"/> to await</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static Task<bool> WithToken([NotNull] this Task task, CancellationToken token)
        {
            return task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion, token);
        }

        /// <summary>
        /// Returns a <see cref="Task{T}"/> that either returns the computed value, or <see langword="default"/> if the given <see cref="CancellationToken"/> expires
        /// </summary>
        /// <param name="task">The <see cref="Task{T}"/> to watch</param>
        /// <param name="token">The token to use to monitor the <see cref="Task{T}"/> to await</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static Task<T> WithToken<T>([NotNull] this Task<T> task, CancellationToken token)
        {
            return task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion ? t.Result : default, token);
        }

        /// <summary>
        /// Waits for two <see cref="Task{TResult}"/> instances in parallel and returns their results as a tuple
        /// </summary>
        /// <typeparam name="T1">The return type of the first <see cref="Task{TResult}"/></typeparam>
        /// <typeparam name="T2">The return type of the second <see cref="Task{TResult}"/></typeparam>
        /// <param name="pair">The tuple with the two tasks to await</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<(T1, T2)> AsTask<T1, T2>(this (Task<T1> T1, Task<T2> T2) pair)
        {
            return Task.WhenAll(pair.T1, pair.T2).ContinueWith(_ => (pair.T1.Result, pair.T2.Result));
        }

        /// <summary>
        /// Waits for three <see cref="Task{TResult}"/> instances in parallel and returns their results as a tuple
        /// </summary>
        /// <typeparam name="T1">The return type of the first <see cref="Task{TResult}"/></typeparam>
        /// <typeparam name="T2">The return type of the second <see cref="Task{TResult}"/></typeparam>
        /// <typeparam name="T3">The return type of the third <see cref="Task{TResult}"/></typeparam>
        /// <param name="pair">The tuple with the three tasks to await</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<(T1, T2, T3)> AsTask<T1, T2, T3>(this (Task<T1> T1, Task<T2> T2, Task<T3> T3) pair)
        {
            return Task.WhenAll(pair.T1, pair.T2, pair.T3).ContinueWith(_ => (pair.T1.Result, pair.T2.Result, pair.T3.Result));
        }

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it and handles its exception, if it occurs
        /// </summary>
        /// <param name="task">The <see cref="Task"/> returned by the async call</param>
        /// <param name="handler">The <see cref="AggregateExceptionHandler"/> instance to use to handle the task exception</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WithHandler([NotNull] this Task task, [NotNull] AggregateExceptionHandler handler)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                    handler(t.Exception);
            });
        }

        /// <summary>
        /// Schedules a continuation on the given <see cref="Task{TResult}"/> only in case it completes successfully
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Task{TResult}"/> to wait for</typeparam>
        /// <param name="task">The input <see cref="Task{TResult}"/> instance</param>
        /// <param name="continuation">The continuation to schedule</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Then<T>([NotNull] this Task<T> task, [NotNull] Action<T> continuation)
        {
            task.ContinueWith(
                t => continuation(t.Result),
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
