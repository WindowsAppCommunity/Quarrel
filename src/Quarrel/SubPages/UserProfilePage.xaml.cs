// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Analytics;
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
        private IAnalyticsService _analyticsService = null;
        private IDiscordService _discordService = null;
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfilePage"/> class.
        /// </summary>
        public UserProfilePage()
        {
            this.InitializeComponent();
            if (SubFrameNavigationService.Parameter != null)
            {
                var member = (BindableGuildMember)SubFrameNavigationService.Parameter;
                ConnectedAnimationService.GetForCurrentView()?.GetAnimation(ViewModels.Helpers.Constants.ConnectedAnimationKeys.MemberFlyoutAnimation)?.TryStart(FullAvatar);
                this.DataContext = new UserProfilePageViewModel(member);

                AnalyticsService.Log(
                    Constants.Analytics.Events.OpenUserProfile,
                    ("user-id", member.RawModel.Id));
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

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        /// <summary>
        /// Change user note automatically when focus NoteBox is lost.
        /// </summary>
        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DiscordService.UserService.AddNote(ViewModel.User.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }
    }
}
