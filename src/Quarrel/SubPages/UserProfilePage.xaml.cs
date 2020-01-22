using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Rest;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

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
