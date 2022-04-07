// Adam Dernis © 2022

using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.ViewModels
{
    public partial class CurrentUserViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private SelfUser? _me;

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

        public SelfUser? Me
        {
            get => _me;
            set => SetProperty(ref _me, value);
        }
    }
}
