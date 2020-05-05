// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// A control to display an invite.
    /// </summary>
    public sealed partial class InviteControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InviteControl"/> class.
        /// </summary>
        public InviteControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the invite displayed.
        /// </summary>
        public BindableInvite ViewModel => DataContext as BindableInvite;

        private void CopyInvite(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(string.Format("https://discord.gg/invite/{0}", ViewModel.Model.Code));
        }

        private void ShareInvite(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO: Allow sharing invites.
        }

        private void RemoveInvite(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO: Invite management list
        }
    }
}
