// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="Task"/> type.
    /// </summary>
    internal static partial class TaskExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task{T}"/> that either returns a <see cref="bool"/> value depending on if the given <see cref="CancellationToken"/> has expired.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to watch.</param>
        /// <param name="token">The token to use to monitor the <see cref="Task"/> to await.</param>
        /// <returns>A <see cref="Task"/> that either returns a <see cref="bool"/> value depending on if the given <see cref="CancellationToken"/> has expired.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static Task<bool> WithToken([NotNull] this Task task, CancellationToken token)
        {
            return task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion, token);
        }

        /// <summary>
        /// Returns a <see cref="Task{T}"/> that either returns the computed value, or <see langword="default"/> if the given <see cref="CancellationToken"/> expires.
        /// </summary>
        /// <param name="task">The <see cref="Task{T}"/> to watch.</param>
        /// <param name="token">The token to use to monitor the <see cref="Task{T}"/> to await.</param>
        /// <typeparam name="T">The type of value for the <see cref="Task{T}"/> to return.</typeparam>
        /// <returns>A <see cref="Task{T}"/> that either returns the computed value, or <see langword="default"/> if the given <see cref="CancellationToken"/> expires.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static Task<T> WithToken<T>([NotNull] this Task<T> task, CancellationToken token)
        {
            return task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion ? t.Result : default, token);
        }

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it and handles its exception, if it occurs.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> returned by the async call.</param>
        /// <param name="handler">The <see cref="AggregateExceptionHandler"/> instance to use to handle the task exception.</param>
        /// <returns>The computed value for <paramref name="task"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WithHandler([NotNull] this Task task, [NotNull] AggregateExceptionHandler handler)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    handler(t.Exception);
                }
            });
        }

        /// <summary>
        /// Schedules a continuation on the given <see cref="Task{TResult}"/> only in case it completes successfully.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Task{TResult}"/> to wait for.</typeparam>
        /// <param name="task">The input <see cref="Task{TResult}"/> instance.</param>
        /// <param name="continuation">The continuation to schedule.</param>
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
