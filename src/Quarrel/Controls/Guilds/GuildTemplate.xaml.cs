using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UICompositionAnimations.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Guilds
{
    public sealed partial class GuildTemplate : UserControl
    {
        public GuildTemplate()
        {
            this.InitializeComponent();

            Messenger.Default.Register<GatewayMessageAckMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() => { this.Bindings.Update(); });
            });

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        public BindableGuild ViewModel => DataContext as BindableGuild;
    }
}
