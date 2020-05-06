// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.Models.Bindables.Messages.Embeds
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Invite"/> model.
    /// </summary>
    public class BindableInvite : BindableModelBase<Invite>, IEmbed
    {
        private bool _joined;
        private RelayCommand _acceptInviteCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableInvite"/> class.
        /// </summary>
        /// <param name="invite">The base <paramref name="invite"/> object.</param>
        public BindableInvite(Invite invite) : base(invite)
        {
            if (Model != null)
            {
                _joined = SimpleIoc.Default.GetInstance<IGuildsService>().AllGuilds.ContainsKey(Model.Guild.Id);
            }
        }

        /// <summary>
        /// Gets the url of the guild icon.
        /// </summary>
        public string IconUrl => $"https://cdn.discordapp.com/icons/{Model.Guild.Id}/{Model.Guild.Icon}.png?size=128";

        /// <summary>
        /// Gets or sets a value indicating whether or not the user is in the guild.
        /// </summary>
        public bool Joined
        {
            get => _joined;
            set => Set(ref _joined, value);
        }

        /// <summary>
        /// Gets a command that accepts the bound invite.
        /// </summary>
        public RelayCommand AcceptInviteCommand => _acceptInviteCommand = new RelayCommand(async () =>
        {
            await DiscordService.InviteService.AcceptInvite(Model.Code);
            Joined = true;
        });

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        /// <summary>
        /// Updates bindings for unbound properties.
        /// </summary>
        public void UpdateBindings()
        {
            RaisePropertyChanged(nameof(IconUrl));
        }
    }
}
