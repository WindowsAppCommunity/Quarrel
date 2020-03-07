// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Messages.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for an asynchronous request message.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    public abstract class AsyncRequestMessageBase<TRequest, TResponse> : ValueChangedMessageBase<TRequest>
    {
        // Private completion source to signal the autosave completion
        private readonly TaskCompletionSource<TResponse> _completionSource = new TaskCompletionSource<TResponse>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRequestMessageBase{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="value">The changed value.</param>
        protected AsyncRequestMessageBase(TRequest value) : base(value)
        {
        }

        /// <summary>
        /// Gets a <see cref="Task{T}"/> that indicates the result of the current request message.
        /// </summary>
        [NotNull]
        public Task<TResponse> Task => _completionSource.Task;

        /// <summary>
        /// Reports a result for the current request message.
        /// </summary>
        /// <param name="result">Result of request.</param>
        public void ReportResult(TResponse result) => _completionSource.TrySetResult(result);
    }
}
