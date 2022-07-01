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
        /// <param name="streamKey">The id of the stream.</param>
        public StreamCreatedMessage(string streamKey)
        {
            StreamKey = streamKey;
        }
        
        /// <summary>
        /// Gets the id of the user hosting the stream
        /// </summary>
        public string StreamKey { get; }
    }
}
