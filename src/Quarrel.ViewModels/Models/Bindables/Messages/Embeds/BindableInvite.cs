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
        private RelayCommand _removeCommand;
        private IDiscordService _discordService = null;
        private IGuildsService _guildsService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableInvite"/> class.
        /// </summary>
        /// <param name="invite">The base <paramref name="invite"/> object.</param>
        public BindableInvite(Invite invite) : base(invite)
        {
            if (Model != null)
            {
                _joined = GuildsService.GetGuild(Model.Guild.Id) != null;
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
        /// Gets a value indicating whether or not the invite can be removed by the user.
        /// </summary>
        public bool CanRemove => GuildsService.GetGuild(Model.Guild.Id).Permissions.ManangeGuild;

        /// <summary>
        /// Gets a command that accepts the bound invite.
        /// </summary>
        public RelayCommand AcceptInviteCommand => _acceptInviteCommand = new RelayCommand(async () =>
        {
            await DiscordService.InviteService.AcceptInvite(Model.Code);
            Joined = true;
        });

        /// <summary>
        /// Gets the command run when clicking the remove button.
        /// </summary>
        public RelayCommand RemoveCommand => _removeCommand = new RelayCommand(async () =>
        {
            await DiscordService.InviteService.DeleteInvite(Model.Code);
        });

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        /// <summary>
        /// Updates bindings for unbound properties.
        /// </summary>
        public void UpdateBindings()
        {
            RaisePropertyChanged(nameof(IconUrl));
        }
    }
}
