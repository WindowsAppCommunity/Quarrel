using GalaSoft.MvvmLight.Messaging;
using DiscordAPI.Models;
using Quarrel.Services;
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
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using UICompositionAnimations.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class VoiceConnection : UserControl
    {
        public VoiceConnection()
        {
            this.InitializeComponent();

            Messenger.Default.Register<GatewayVoiceStateUpdateMessage>(this, async m =>
            {
                if (m.VoiceState.UserId == ServicesManager.Discord.CurrentUser.Id)
                {
                    await DispatcherHelper.RunAsync(() => DataContext = m.VoiceState);
                }
            });

            Messenger.Default.Register<CurrentUserVoiceStateRequestMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() => m.ReportResult(ViewModel));
            });

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        VoiceState ViewModel => DataContext as VoiceState ?? new VoiceState() { };

        private async void Disconnect(object sender, RoutedEventArgs e)
        {
            await ServicesManager.Discord.Gateway.Gateway.VoiceStatusUpdate(null, null, false, false);
        }
    }
}
