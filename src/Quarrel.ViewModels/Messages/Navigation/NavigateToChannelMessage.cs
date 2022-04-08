// Adam Dernis © 2022

namespace Quarrel.Messages.Navigation
{
    public class NavigateToChannelMessage<T>
    {
        public NavigateToChannelMessage(T channel)
        {
            Channel = channel;
        }

        public T Channel { get; }
    }
}
