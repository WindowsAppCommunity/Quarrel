// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when a channel is selected.
    /// </summary>
    public class ChannelSelectedMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSelectedMessage{T}"/> class.
        /// </summary>
        public ChannelSelectedMessage(T channel)
        {
            Channel = channel;
        }
        
        /// <summary>
        /// Gets the channel selected.
        /// </summary>
        public T Channel { get; }
    }
}
