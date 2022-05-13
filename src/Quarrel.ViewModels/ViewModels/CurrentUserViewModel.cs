// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Users;
using Quarrel.Messages;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.ViewModels.SubPages.Settings;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The view model for the current user button.
    /// </summary>
    public partial class CurrentUserViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private BindableSelfUser? _me;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserViewModel"/> class.
        /// </summary>
        public CurrentUserViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            NavigateToSettingsCommand = new RelayCommand(NavigateToSettings);
            SetStatusCommand = new RelayCommand<UserStatus>(SetStatus);

            _messenger.Register<UserLoggedInMessage>(this, (_, _) =>
            {
                _dispatcherService.RunOnUIThread(() =>
                {
                    Me = _discordService.GetMe();
                });
            });
        }

        public BindableSelfUser? Me
        {
            get => _me;
            set => SetProperty(ref _me, value);
        }

        public RelayCommand NavigateToSettingsCommand { get; }

        public RelayCommand<UserStatus> SetStatusCommand { get; }

        /// <summary>
        /// Sends a request to open the settings subpage.
        /// </summary>
        public void NavigateToSettings()
        {
            _messenger.Send(new NavigateToSubPageMessage(typeof(UserSettingsPageViewModel)));
        }

        public void SetStatus(UserStatus status)
        {
            _discordService.SetStatus(status);
        }
    }
}
