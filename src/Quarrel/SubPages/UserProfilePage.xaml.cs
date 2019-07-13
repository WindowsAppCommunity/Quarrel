using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using Quarrel.SubPages.Interfaces;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class UserProfilePage : UserControl, IConstrainedSubPage
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        public UserProfilePage()
        {
            this.InitializeComponent();
        }

        public UserProfilePage([NotNull] BindableGuildMember member) : this()
        {
            this.DataContext = member;
            LoadProfile();
        }

        public async void LoadProfile()
        {
            if (!ViewModel.Model.User.Bot)
                _Profile = await discordService.UserService.GetUserProfile(ViewModel.Model.User.Id);
            else
                _Profile = new UserProfile() { user = ViewModel.Model.User };

            _Profile.Friend = cacheService.Runtime.TryGetValue<Friend>(Quarrel.Helpers.Constants.Cache.Keys.Friend, ViewModel.Model.User.Id);

            if (_Profile.Friend == null)
                _Profile.Friend = new Friend() { Type = 0, Id = ViewModel.Model.User.Id, user = ViewModel.Model.User };

            if (!ViewModel.Model.User.Bot)
                _Profile.SharedFriends = await discordService.UserService.GetUserReleations(ViewModel.Model.User.Id);

            this.Bindings.Update();
        }

        BindableGuildMember ViewModel => DataContext as BindableGuildMember;

        private UserProfile _Profile;


        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            discordService.UserService.AddNote(ViewModel.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }

        public double MaxExpandedHeight { get; } = 768;

        public double MaxExpandedWidth { get; } = 768;
    }
}
