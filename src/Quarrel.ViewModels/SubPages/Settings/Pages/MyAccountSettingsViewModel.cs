using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Rest;
using Quarrel.ViewModels.Services.Users;

namespace Quarrel.ViewModels.SubPages.Settings.Pages
{
    public class MyAccountSettingsViewModel : ViewModelBase
    {
        public MyAccountSettingsViewModel()
        {
            SetDefaults();
        }

        public ICurrentUsersService CurrentUsersService
        {
            get => SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        }

        private string email;
        public string Email
        {
            get => email;
            set => Set(ref email, value);
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

        private string avatarUrl;
        public string AvatarUrl
        {
            get => avatarUrl;
            set => Set(ref avatarUrl, value);
        }

        public bool EditingAvatar => Base64Avatar != "";

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
            ApplyChanges();
            EditingAccountInfo = false;
            EditingPassword = false;
        });

        private RelayCommand cancelAccountEditCommand;
        public RelayCommand CancelAccountEditCommand => cancelAccountEditCommand = new RelayCommand(() =>
        {
            SetDefaults();
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

        public void SetDefaults()
        {
            Email = CurrentUsersService.CurrentUser.Model.Email;
            Username = CurrentUsersService.CurrentUser.Model.Username;
            AvatarUrl = CurrentUsersService.CurrentUser.Model.AvatarUrl;
        }

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
            else
                modify = new ModifyUser()
                {
                    Username = Username,
                    NewPassword = newPassword,
                    Password = password
                };
            CurrentUsersService.CurrentUser.Model = await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.ModifyCurrentUser(modify);
        }
    }
}
