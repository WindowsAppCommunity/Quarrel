// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Controls.Shell.Enums;
using Quarrel.Messages.Discord;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Accounts.Models;
using Quarrel.ViewModels.Enums;

namespace Quarrel.ViewModels
{
    public partial class WindowViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IStorageService _storageService;
        private readonly IDispatcherService _dispatcherService;

        [AlsoNotifyChangeFor(nameof(IsLoading))]
        [AlsoNotifyChangeFor(nameof(IsLoggedOut))]
        [AlsoNotifyChangeFor(nameof(SplashStatus))]
        [ObservableProperty]
        private WindowHostState _windowState;

        public WindowViewModel(IMessenger messenger, IDiscordService discordService, IStorageService storageService, IDispatcherService dispatcherService)
        {
            WindowState = WindowHostState.Loading;
            _messenger = messenger;
            _discordService = discordService;
            _storageService = storageService;
            _dispatcherService = dispatcherService;

            _messenger.Register<ConnectingMessage>(this, (_, _) => WindowState = WindowHostState.Connecting);
            _messenger.Register<UserLoggedInMessage>(this, (_, m) => OnLoggedIn(m.AccountInfo));

            InitializeLogin();
        }

        public SplashStatus SplashStatus => (SplashStatus)_windowState;

        public bool IsLoading => _windowState == WindowHostState.Connecting || _windowState == WindowHostState.Loading;

        public bool IsLoggedOut => _windowState == WindowHostState.LoggedOut;

        private async void InitializeLogin()
        {
            // Login if possible
            await _storageService.AccountInfoStorage.LoadAsync();
            AccountInfo? activeAccount = _storageService.AccountInfoStorage.ActiveAccount;
            if (activeAccount is not null)
            {
                await _discordService.LoginAsync(activeAccount.Token);
            }
            else
            {
                WindowState = WindowHostState.LoggedOut;
            }
        }

        private async void OnLoggedIn(AccountInfo info)
        {
            _storageService.AccountInfoStorage.RegisterAccount(info);
            _storageService.AccountInfoStorage.SelectAccount(info.Id);
            await _storageService.AccountInfoStorage.SaveAsync();

            _dispatcherService.RunOnUIThread(() =>
            {
                WindowState = WindowHostState.LoggedIn;
            });
        }
    }
}
