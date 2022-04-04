// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.DispatcherService;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Accounts.Models;

namespace Quarrel.ViewModels
{
    public partial class WindowViewModel : ObservableObject
    {
        public enum WindowHostState
        {
            LoggedOut,
            Connecting,
            LoggedIn,
        }

        private IMessenger _messenger;
        private IStorageService _storageService;
        private IDispatcherService _dispatcherService;

        [AlsoNotifyChangeFor(nameof(IsConnecting))]
        [AlsoNotifyChangeFor(nameof(IsLoggedIn))]
        [ObservableProperty]
        private WindowHostState _windowState;

        public WindowViewModel(IMessenger messenger, IStorageService storageService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _storageService = storageService;
            _dispatcherService = dispatcherService;
            WindowState = WindowHostState.LoggedOut;

            _messenger.Register<ConnectingMessage>(this, (_, _) => WindowState = WindowHostState.Connecting);
            _messenger.Register<UserLoggedInMessage>(this, (_, m) => OnLoggedIn(m.AccountInfo));
        }

        public bool IsConnecting => _windowState == WindowHostState.Connecting;

        public bool IsLoggedIn => _windowState == WindowHostState.LoggedIn;

        private void OnLoggedIn(AccountInfo info)
        {
            _storageService.AccountInfoStorage.RegisterAccount(info);
            _storageService.AccountInfoStorage.SelectAccount(info.Id);

            _dispatcherService.RunOnUIThread(() =>
            {
                WindowState = WindowHostState.LoggedIn;
            });
        }
    }
}
