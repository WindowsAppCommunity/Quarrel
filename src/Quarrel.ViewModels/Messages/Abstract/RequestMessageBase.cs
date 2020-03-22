// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.ViewModels.Messages.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for request messages.
    /// </summary>
    /// <typeparam name="TResponse">The response type to the request.</typeparam>
    public abstract class RequestMessageBase<TResponse>
    {
        /// <summary>
        /// Gets the message response.
        /// </summary>
        public TResponse Result { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not a result has already been assigned to this instance.
        /// </summary>
        public bool ResponseReceived { get; private set; }

        /// <summary>
        /// Reports a result for the current request message.
        /// </summary>
        /// <param name="result">The response to the message.</param>
        public void ReportResult(TResponse result)
        {
            if (ResponseReceived)
            {
                throw new InvalidOperationException("This message has already been used");
            }

            ResponseReceived = true;
            Result = result;
        }
    }
}
