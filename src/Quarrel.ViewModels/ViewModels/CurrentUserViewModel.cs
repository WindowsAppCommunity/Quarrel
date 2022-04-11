// Adam Dernis © 2022

using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

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

        [ObservableProperty]
        private SelfUser? _me;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserViewModel"/> class.
        /// </summary>
        public CurrentUserViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            _messenger.Register<UserLoggedInMessage>(this, (_, _) =>
            {
                _dispatcherService.RunOnUIThread(() =>
                {
                    Me = _discordService.GetMe();
                });
            });
        }
    }
}
