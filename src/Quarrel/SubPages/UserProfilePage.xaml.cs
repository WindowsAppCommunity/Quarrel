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
using Quarrel.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Windows.UI.Xaml.Media.Animation;
using Quarrel.Services.Users;
using Quarrel.ViewModels.ViewModels.SubPages;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class UserProfilePage : UserControl, IConstrainedSubPage
    {
        #region Constrcutors

        public UserProfilePage()
        {
            this.InitializeComponent();
            if (subFrameNavigationService.Parameter != null)
            {
                ConnectedAnimationService.GetForCurrentView()?.GetAnimation(ViewModels.Helpers.Constants.ConnectedAnimationKeys.MemberFlyoutAnimation)?.TryStart(FullAvatar);
                this.DataContext = new UserProfilePageViewModel((BindableGuildMember)subFrameNavigationService.Parameter);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change user note automatically when focus NoteBox is lost
        /// </summary>
        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            discordService.UserService.AddNote(ViewModel.User.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }


        #endregion

        #region Properties

        #region Services

        private IDiscordService discordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISubFrameNavigationService subFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        #endregion

        public UserProfilePageViewModel ViewModel => DataContext as UserProfilePageViewModel;

        #endregion

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 768;
        public double MaxExpandedWidth { get; } = 768;

        #endregion
    }
}
