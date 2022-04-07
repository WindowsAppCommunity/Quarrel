// Adam Dernis © 2022

namespace Quarrel.Messages.Navigation
{
    public class NavigateToGuildMessage<T>
    {
        public NavigateToGuildMessage(T guild)
        {
            Guild = guild;
        }

        public T Guild { get; }
    }
}
