using GalaSoft.MvvmLight.Messaging;
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
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables;
using Quarrel.Messages;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Navigation;
using Quarrel.Services;
using Quarrel.Services.Rest;
using Quarrel.SubPages;
using UICompositionAnimations.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Members
{
    public sealed partial class MemberFlyoutTemplate : UserControl
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISubFrameNavigationService subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
        public MemberFlyoutTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
            Messenger.Default.Register<GatewayNoteUpdatedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (ViewModel != null && m.UserId == ViewModel.Model.User.Id)
                        this.Bindings.Update();
                });
            });
        }

        public BindableGuildMember ViewModel => DataContext as BindableGuildMember;

        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            discordService.UserService.AddNote(ViewModel.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            // Close popup
            if ((Parent is FlyoutPresenter presenter))
            {
                (presenter.Parent as Popup).IsOpen = false;
            }

            subFrameNavigationService.NavigateTo("UserProfilePage", ViewModel);
        }
    }
}
