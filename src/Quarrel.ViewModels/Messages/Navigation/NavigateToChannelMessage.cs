// Quarrel © 2022

using Quarrel.Bindables.Channels.Interfaces;

namespace Quarrel.Messages.Navigation
{
    public class NavigateToChannelMessage
    {
        public NavigateToChannelMessage(IBindableSelectableChannel channel)
        {
            Channel = channel;
        }

        public IBindableSelectableChannel Channel { get; }
    }
}
