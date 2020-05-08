// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for displaying a user's profile.
    /// </summary>
    public sealed partial class UserProfilePage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfilePage"/> class.
        /// </summary>
        public UserProfilePage()
        {
            this.InitializeComponent();
            if (SubFrameNavigationService.Parameter != null)
            {
                ConnectedAnimationService.GetForCurrentView()?.GetAnimation(ViewModels.Helpers.Constants.ConnectedAnimationKeys.MemberFlyoutAnimation)?.TryStart(FullAvatar);
                this.DataContext = new UserProfilePageViewModel((BindableGuildMember)SubFrameNavigationService.Parameter);
            }
        }

        /// <summary>
        /// Gets the user's profile page data.
        /// </summary>
        public UserProfilePageViewModel ViewModel => DataContext as UserProfilePageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 768;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 768;

        private IDiscordService DiscordService => SimpleIoc.Default.GetInstance<IDiscordService>();

        private ISubFrameNavigationService SubFrameNavigationService => SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        /// <summary>
        /// Change user note automatically when focus NoteBox is lost.
        /// </summary>
        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DiscordService.UserService.AddNote(ViewModel.User.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }
    }
}
