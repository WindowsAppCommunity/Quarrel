// Adam Dernis © 2022

using Quarrel.Bindables.Guilds;

namespace Quarrel.Messages.Navigation
{
    public class NavigateToGuildMessage
    {
        public NavigateToGuildMessage(BindableGuild guild)
        {
            Guild = guild;
        }

        public BindableGuild Guild { get; }
    }
}
