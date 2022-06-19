// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Users;
using Quarrel.Messages;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.ViewModels.SubPages.Settings.UserSettings;
using System;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The view model for the current user button.
    /// </summary>
    public partial class CurrentUserViewModel : ObservableRecipient
    {
        private readonly ILoggingService _loggingService;
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private BindableSelfUser? _me;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserViewModel"/> class.
        /// </summary>
        public CurrentUserViewModel(ILoggingService loggingService, IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _loggingService = loggingService;
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

        /// <summary>
        /// Gets the bindable current self user.
        /// </summary>
        public BindableSelfUser? Me
        {
            get => _me;
            set => SetProperty(ref _me, value);
        }

        /// <summary>
        /// Gets a command that requests navigation to settings.
        /// </summary>
        public RelayCommand NavigateToSettingsCommand { get; }

        /// <summary>
        /// Gets a command that sets the current user's status.
        /// </summary>
        public RelayCommand<UserStatus> SetStatusCommand { get; }

        private void NavigateToSettings()
        {
            _messenger.Send(new NavigateToSubPageMessage(typeof(UserSettingsPageViewModel)));
        }

        private void SetStatus(UserStatus status)
        {
            _loggingService.Log(LoggedEvent.StatusSet,
                ("Status", status.GetStringValue()));

            _discordService.SetStatus(status);
        }
    }
}
