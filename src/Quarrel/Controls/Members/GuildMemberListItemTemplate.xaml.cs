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
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using UICompositionAnimations.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Members
{
    public sealed partial class GuildMemberListItemTemplate : UserControl
    {
        public GuildMemberListItemTemplate()
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
        }

        public BindableUser ViewModel => DataContext as BindableUser;
    }
}
