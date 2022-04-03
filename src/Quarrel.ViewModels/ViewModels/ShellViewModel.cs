// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.DispatcherService;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Accounts.Models;

namespace Quarrel.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private IMessenger _messenger;
        private IStorageService _storageService;
        private IDispatcherService _dispatcherService;

        private bool _isConnecting;
        private bool _isLoggedIn;

        public ShellViewModel(IMessenger messenger, IStorageService storageService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _storageService = storageService;
            _dispatcherService = dispatcherService;
            _isLoggedIn = false;

            _messenger.Register<ConnectingMessage>(this, (_, _) => IsConnecting = true);
            _messenger.Register<UserLoggedInMessage>(this, (_, m) => OnLogin(m.AccountInfo));
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set => SetProperty(ref _isConnecting, value);
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        private void OnLogin(AccountInfo info)
        {
            _storageService.AccountInfoStorage.RegisterAccount(info);
            _storageService.AccountInfoStorage.SelectAccount(info.Id);

            _dispatcherService.RunOnUIThread(() =>
            {
                IsConnecting = false;
                IsLoggedIn = true;
            });
        }
    }
}
