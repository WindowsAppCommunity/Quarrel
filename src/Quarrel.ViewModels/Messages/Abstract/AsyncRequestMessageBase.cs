using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Quarrel.Messages.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for an asynchronous request message
    /// </summary>
    public abstract class AsyncRequestMessageBase<TRequest, TResponse> : ValueChangedMessageBase<TRequest>
    {
        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="value">The changed value</param>
        protected AsyncRequestMessageBase(TRequest value) : base(value) { }

        // Private completion source to signal the autosave completion
        private readonly TaskCompletionSource<TResponse> CompletionSource = new TaskCompletionSource<TResponse>();

        /// <summary>
        /// Gets a <see cref="Task{T}"/> that indicates the result of the current request message
        /// </summary>
        [NotNull]
        public Task<TResponse> Task => CompletionSource.Task;

        /// <summary>
        /// Reports a result for the current request message
        /// </summary>
        public void ReportResult(TResponse result) => CompletionSource.TrySetResult(result);
    }
}
