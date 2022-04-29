// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
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
        internal BindableGuildMember(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            GuildMember guildMember) :
            base(messenger, discordService, dispatcherService)
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
