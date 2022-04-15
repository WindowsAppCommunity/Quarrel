// Quarrel © 2022

using Quarrel.Bindables.Channels.Interfaces;

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when navigation to a channel is requested.
    /// </summary>
    public class NavigateToChannelMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToChannelMessage"/> class.
        /// </summary>
        public NavigateToChannelMessage(IBindableSelectableChannel channel)
        {
            Channel = channel;
        }
        
        /// <summary>
        /// Gets the channel being navigated to.
        /// </summary>
        public IBindableSelectableChannel Channel { get; }
    }
}
