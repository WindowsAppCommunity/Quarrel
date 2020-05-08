// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;

namespace Quarrel.ViewModels.SubPages.AddServer.Pages
{
    /// <summary>
    /// Join Server page data.
    /// </summary>
    public class JoinServerPageViewModel : ViewModelBase
    {
        private string _joinCode;
        private bool _isValid;
        private BindableInvite _invite = new BindableInvite(null);
        private RelayCommand _joinServerCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinServerPageViewModel"/> class.
        /// </summary>
        public JoinServerPageViewModel()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current invite code is valid.
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }

        /// <summary>
        /// Gets or sets the join code of the new server.
        /// </summary>
        public string JoinCode
        {
            get => _joinCode;
            set
            {
                Set(ref _joinCode, value);
                CheckInvite(value);
            }
        }

        /// <summary>
        /// Gets or sets the invite found for the pending inviteCode.
        /// </summary>
        public BindableInvite FoundInvite
        {
            get => _invite;
            set => Set(ref _invite, value);
        }

        /// <summary>
        /// Gets a command that uses the <see cref="JoinCode"/> to join a server.
        /// </summary>
        public RelayCommand JoinServerCommand => _joinServerCommand = new RelayCommand(async () =>
        {
            try
            {
                await DiscordService.InviteService.AcceptInvite(JoinCode);
            }
            catch
            {
                // TODO: Display error
                return;
            }

            SubFrameNavigationService.GoBack();
        });

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        private async void CheckInvite(string code)
        {
            try
            {
                FoundInvite.Model = await DiscordService.InviteService.GetInvite(code);
                FoundInvite.UpdateBindings();
                IsValid = true;
            }
            catch
            {
                IsValid = false;
            }
        }
    }
}
