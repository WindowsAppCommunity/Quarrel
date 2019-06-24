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
using Quarrel.Models.Bindables;
using Quarrel.Messages;
using Quarrel.Messages.Gateway;
using UICompositionAnimations.Helpers;
using Quarrel.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Members
{
    public sealed partial class MemberFlyoutTemplate : UserControl
    {
        public MemberFlyoutTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };

            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (ViewModel != null && m.UserId == ViewModel.Model.User.Id)
                        this.Bindings.Update();
                });
            });

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
            ServicesManager.Discord.UserService.AddNote(ViewModel.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }
    }
}
