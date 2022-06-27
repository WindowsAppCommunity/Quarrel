// Quarrel © 2022

namespace Quarrel.Messages.Discord.Stream
{
    /// <summary>
    /// A message sent when a stream is opened.
    /// </summary>
    public class StreamCreatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamCreatedMessage"/> class.
        /// </summary>
        /// <param name="userId">The id of the user hosting the stream.</param>
        public StreamCreatedMessage(ulong userId)
        {
            UserId = userId;
        }
        
        /// <summary>
        /// Gets the id of the user hosting the stream
        /// </summary>
        public ulong UserId { get; }
    }
}
