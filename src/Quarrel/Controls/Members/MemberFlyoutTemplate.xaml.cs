// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace Quarrel.Controls.Members
{
    /// <summary>
    /// Flyout to represent a GuildMember.
    /// </summary>
    public sealed partial class MemberFlyoutTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFlyoutTemplate"/> class.
        /// </summary>
        public MemberFlyoutTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if (e.NewValue is BindableGuildMember bMem)
                {
                    bMem.UpdateAccentColor();
                }

                this.Bindings.Update();
            };

            // Updates Note when it changes
            Messenger.Default.Register<GatewayNoteUpdatedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (ViewModel != null && m.UserId == ViewModel.Model.User.Id)
                    {
                        ViewModel.RaisePropertyChanged(nameof(ViewModel.Note));
                    }
                });
            });
        }

        /// <summary>
        /// Gets the GuildMember shown.
        /// </summary>
        public BindableGuildMember ViewModel => DataContext as BindableGuildMember;

        /// <summary>
        /// Updates Note on server when Notebox is edited.
        /// </summary>
        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().UserService.AddNote(ViewModel.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }

        /// <summary>
        /// Sets up connected animation and navigates to UserProfilePage.
        /// </summary>
        private void OpenProfilePage(object sender, RoutedEventArgs e)
        {
            // Connected Animation
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(ViewModels.Helpers.Constants.ConnectedAnimationKeys.MemberFlyoutAnimation, FullAvatar);

            // Navigate
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("UserProfilePage", ViewModel);

            // Close popup
            var presenter = Parent.FindParent<FlyoutPresenter>();
            if (presenter != null)
            {
                (presenter.Parent as Popup).IsOpen = false;
            }
        }
    }
}
