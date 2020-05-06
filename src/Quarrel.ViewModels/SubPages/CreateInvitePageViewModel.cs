// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// Create Invite page data.
    /// </summary>
    public class CreateInvitePageViewModel : ViewModelBase
    {
        private string _guildId;
        private BindableInvite _invite;
        private RelayCommand _generateInvite;
        private int _maxUses;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInvitePageViewModel"/> class.
        /// </summary>
        /// <param name="guildId">The guild to create an invite for.</param>
        public CreateInvitePageViewModel(string guildId)
        {
            _guildId = guildId;
            _invite = new BindableInvite(null);
        }

        /// <summary>
        /// Gets or sets the invite shown.
        /// </summary>
        public BindableInvite Invite
        {
            get => _invite;
            set => Set(ref _invite, value);
        }

        /// <summary>
        /// Gets or sets the max uses for the next created invite.
        /// </summary>
        public int MaxUses
        {
            get => _maxUses;
            set => Set(ref _maxUses, value);
        }

        /// <summary>
        /// Gets a command to create an invite for a guild.
        /// </summary>
        public RelayCommand GenerateInvite => _generateInvite = new RelayCommand(async () =>
        {
            CreateInvite invite = new CreateInvite()
            {
                MaxAge = 0,
                MaxUses = MaxUses,
                Temporary = false,
            };

            Invite.Model = await DiscordService.ChannelService.CreateChannelInvite(_guildId, invite);
            Invite.UpdateBindings();
        });

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();
    }
}
