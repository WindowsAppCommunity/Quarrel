// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// MyAccount Settings page binding data.
    /// </summary>
    public class MyAccountSettingsViewModel : ViewModelBase
    {
        private string _email;
        private string _username;
        private bool _editingAccountInfo;
        private string _newPassword;
        private bool editingPassword;
        private string base64Avatar = string.Empty;
        private string avatarUrl;
        private string password = null;
        private RelayCommand enterAccountEditCommand;
        private RelayCommand finalizeAccountEditCommand;
        private RelayCommand cancelAccountEditCommand;
        private RelayCommand enterPasswordEditCommand;
        private RelayCommand deleteAvatar;
        private RelayCommand logoutCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountSettingsViewModel"/> class.
        /// </summary>
        public MyAccountSettingsViewModel()
        {
            SetDefaults();
        }

        /// <summary>
        /// Gets or sets the pending change in Email.
        /// </summary>
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        /// <summary>
        /// Gets or sets the pending change in username.
        /// </summary>
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether account info is being modified.
        /// </summary>
        public bool EditingAccountInfo
        {
            get => _editingAccountInfo;
            set => Set(ref _editingAccountInfo, value);
        }

        /// <summary>
        /// Gets or sets the pending new password.
        /// </summary>
        public string NewPassword
        {
            get => _newPassword;
            set => Set(ref _newPassword, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the password is being modified.
        /// </summary>
        public bool EditingPassword
        {
            get => editingPassword;
            set => Set(ref editingPassword, value);
        }

        /// <summary>
        /// Gets or sets the Base64 string Avatar used to upload.
        /// </summary>
        /// <remarks>Empty string if unchanged.</remarks>
        public string Base64Avatar
        {
            get => base64Avatar;
            set => Set(ref base64Avatar, value);
        }

        /// <summary>
        /// Gets or sets the URL of the User's Avatar.
        /// </summary>
        public string AvatarUrl
        {
            get => avatarUrl;
            set => Set(ref avatarUrl, value);
        }

        /// <summary>
        /// Gets a value indicating whether the Avatar has a pending change.
        /// </summary>
        public bool EditingAvatar => Base64Avatar != string.Empty;

        /// <summary>
        /// Gets or sets the password used to authenticate user edits.
        /// </summary>
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        /// <summary>
        /// Gets a command to enter editing mode.
        /// </summary>
        public RelayCommand EnterAccountEditCommand => enterAccountEditCommand = new RelayCommand(() =>
        {
            EditingAccountInfo = true;
        });

        /// <summary>
        /// Gets a command to save changes and leaves editing mode.
        /// </summary>
        public RelayCommand FinalizeAccountEditCommand => finalizeAccountEditCommand = new RelayCommand(() =>
        {
            ApplyChanges();
            EditingAccountInfo = false;
            EditingPassword = false;
        });

        /// <summary>
        /// Gets a command to cancel editing mode.
        /// </summary>
        public RelayCommand CancelAccountEditCommand => cancelAccountEditCommand = new RelayCommand(() =>
        {
            SetDefaults();
            EditingAccountInfo = false;
            EditingPassword = false;
        });

        /// <summary>
        /// Gets a command to enter password editing mode.
        /// </summary>
        public RelayCommand EnterPasswordEditCommand => enterPasswordEditCommand = new RelayCommand(() =>
        {
            EditingPassword = true;
        });

        /// <summary>
        /// Gets a command to adds Deleted Avatar to pending changes.
        /// </summary>
        public RelayCommand DeleteAvatar => deleteAvatar = new RelayCommand(() =>
        {
            Base64Avatar = null;
        });

        /// <summary>
        /// Gets a command that logs the user out of the app.
        /// </summary>
        public RelayCommand LogoutCommand => logoutCommand = new RelayCommand(() =>
        {
            DiscordService.Logout();
        });

        private ICurrentUserService CurrentUsersService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        /// <summary>
        /// Restores unedited values.
        /// </summary>
        public void SetDefaults()
        {
            Email = CurrentUsersService.CurrentUser.Model.Email;
            Username = CurrentUsersService.CurrentUser.Model.Username;
            AvatarUrl = CurrentUsersService.CurrentUser.Model.AvatarUrl;
        }

        /// <summary>
        /// Saves pending changes to the user.
        /// </summary>
        public async void ApplyChanges()
        {
            ModifyUser modify = new ModifyUser();
            if (EditingAvatar)
            {
                modify = new ModifyUserAndAvatar()
                {
                    Username = Username,
                    Avatar = Base64Avatar,
                    NewPassword = _newPassword,
                    Password = password,
                };
            }
            else
            {
                modify = new ModifyUser()
                {
                    Username = Username,
                    NewPassword = _newPassword,
                    Password = password,
                };
            }

            CurrentUsersService.CurrentUser.Model = await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.ModifyCurrentUser(modify);
        }
    }
}
