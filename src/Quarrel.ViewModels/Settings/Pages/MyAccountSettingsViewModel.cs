using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Quarrel.Services.Users;
using Quarrel.Services.Rest;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using DiscordAPI.API.User.Models;

namespace Quarrel.ViewModels.Settings.Pages
{
    public class MyAccountSettingsViewModel : ViewModelBase
    {
        public MyAccountSettingsViewModel()
        {
            Username = CurrentUsersService.CurrentUser.Model.Username;
        }

        public ICurrentUsersService CurrentUsersService
        {
            get => SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        }

        private string username;
        public string Username
        {
            get => username;
            set => Set(ref username, value);
        }


        private bool editingAccountInfo;
        public bool EditingAccountInfo
        {
            get => editingAccountInfo;
            set => Set(ref editingAccountInfo, value);
        }

        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set => Set(ref newPassword, value);
        }


        private bool editingPassword;
        public bool EditingPassword
        {
            get => editingPassword;
            set => Set(ref editingPassword, value);
        }


        private string base64Avatar = "";
        public string Base64Avatar
        {
            get => base64Avatar;
            set => Set(ref base64Avatar, value);
        }

        public bool EditingAvatar => Base64Avatar != "";

        public bool RemovingAvater => Base64Avatar == null;

        private string password = null;
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }


        private RelayCommand enterAccountEditCommand;
        public RelayCommand EnterAccountEditCommand => enterAccountEditCommand = new RelayCommand(() =>
        {
            EditingAccountInfo = true;
        });

        private RelayCommand finalizeAccountEditCommand;
        public RelayCommand FinalizeAccountEditCommand => finalizeAccountEditCommand = new RelayCommand(() =>
        {
            // TODO:
        });

        private RelayCommand cancelAccountEditCommand;
        public RelayCommand CancelAccountEditCommand => cancelAccountEditCommand = new RelayCommand(() =>
        {
            EditingAccountInfo = false;
            EditingPassword = false;
        });

        private RelayCommand enterPasswordEditCommand;
        public RelayCommand EnterPasswordEditCommand => enterPasswordEditCommand = new RelayCommand(() =>
        {
            EditingPassword = true;
        });

        private RelayCommand deleteAvatar;
        public RelayCommand DeleteAvatar => deleteAvatar = new RelayCommand(() =>
        {
            Base64Avatar = null;
        });

        public async void ApplyChanges()
        {
            ModifyUser modify = new ModifyUser();
            if (EditingAvatar)
                modify = new ModifyUserAndAvatar()
                {
                    Username = Username,
                    Avatar = Base64Avatar,
                    NewPassword = newPassword,
                    Password = password
                };

            CurrentUsersService.CurrentUser.Model = await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.ModifyCurrentUser(modify);
        }
    }
}
