namespace Quarrel.Messages.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for request messages with an input parameter
    /// </summary>
    public abstract class RequestMessageBase<TRequest, TResponse> : RequestMessageBase<TResponse>
    {
        /// <summary>
        /// Gets the request parameter for the current message
        /// </summary>
        public TRequest Parameter { get; }

        /// <summary>
        /// Creates a new request message with the specified parameters
        /// </summary>
        /// <param name="request">The request to send</param>
        protected RequestMessageBase(TRequest request) => Parameter = request;
    }
}
