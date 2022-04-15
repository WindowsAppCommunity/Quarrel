// Quarrel © 2022

using Quarrel.Bindables.Guilds;

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when navigation to a guild is requested.
    /// </summary>
    public class NavigateToGuildMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToGuildMessage"/> class.
        /// </summary>
        public NavigateToGuildMessage(BindableGuild guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets the guild being navigated to.
        /// </summary>
        public BindableGuild Guild { get; }
    }
}
