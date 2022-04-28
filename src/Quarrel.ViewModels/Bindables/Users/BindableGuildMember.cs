// Quarrel © 2022

using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Users
{
    public class BindableGuildMember : BindableItem
    {
        private GuildMember _guildMember;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableGuildMember"/> class.
        /// </summary>
        internal BindableGuildMember(IDiscordService discordService, IDispatcherService dispatcherService, GuildMember guildMember) :
            base(discordService, dispatcherService)
        {
            _guildMember = guildMember;
        }

        public GuildMember GuildMember
        {
            get => _guildMember;
            set => SetProperty(ref _guildMember, value);
        }
    }
}
