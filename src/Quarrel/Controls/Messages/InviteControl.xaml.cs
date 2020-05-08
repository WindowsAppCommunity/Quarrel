// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Services.Clipboard;
using System;
using Windows.UI.Xaml;
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
        /// Invoked when the remove button is clicked.
        /// </summary>
        public event EventHandler<BindableInvite> RemoveClicked;

        /// <summary>
        /// Gets the invite displayed.
        /// </summary>
        public BindableInvite ViewModel => DataContext as BindableInvite;

        /// <summary>
        /// Gets or sets a value indicating whether or not the control is being used in the invite list.
        /// </summary>
        public bool InviteList { get; set; }

        private void CopyInvite(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(string.Format("https://discord.gg/invite/{0}", ViewModel.Model.Code));
        }

        private void ShareInvite(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO: Allow sharing invites.
        }

        private void Remove_Clicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this, ViewModel);
        }
    }
}
