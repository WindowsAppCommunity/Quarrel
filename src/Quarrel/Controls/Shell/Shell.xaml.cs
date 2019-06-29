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
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Services;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.SubPages;
using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();

            // Setup SideDrawer
            ContentContainer.SetupInteraction();

            Login();
        }

        public async void Login()
        {
            var token = (string)(await ServicesManager.Cache.Persistent.Roaming.TryGetValueAsync<object>(Quarrel.Helpers.Constants.Cache.Keys.AccessToken));
            if (string.IsNullOrEmpty(token))
            {
                await Task.Delay(100);
                Messenger.Default.Send(SubFrameNavigationRequestMessage.To(new LoginPage()));
            }
            else
            {
                ServicesManager.Discord.Login(token);
            }
        }
    }
}
