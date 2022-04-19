// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when navigation to a channel is requested.
    /// </summary>
    public class NavigateToChannelMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToChannelMessage{T}"/> class.
        /// </summary>
        public NavigateToChannelMessage(T channel)
        {
            Channel = channel;
        }
        
        /// <summary>
        /// Gets the channel being navigated to.
        /// </summary>
        public T Channel { get; }
    }
}
