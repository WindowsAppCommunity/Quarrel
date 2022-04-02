// Adam Dernis © 2022

namespace Discord.API.Models.Managed.Base
{
    public abstract class DiscordItem
    {
        private DiscordClient _context;

        public DiscordItem(DiscordClient context)
        {
            _context = context;
        }

        protected DiscordClient Context { get; }
    }
}
