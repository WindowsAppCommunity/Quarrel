// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent to request selecting a channel.
    /// </summary>
    public class SelectChannelMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectChannelMessage{T}"/> class.
        /// </summary>
        public SelectChannelMessage(T channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets the channel to request selecting.
        /// </summary>
        public T Channel { get; }
    }
}
