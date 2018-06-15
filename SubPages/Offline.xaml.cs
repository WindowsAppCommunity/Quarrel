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

using Discord_UWP.Managers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Offline : Page
    {
        public Offline()
        {
            this.InitializeComponent();
            //loadMessages();
        }

        private void TryLogin(object sender, RoutedEventArgs e)
        {
            App.LogIn();
        }

        //public async void loadMessages()
        //{
        //    foreach (var message in await MessageManager.ConvertMessage(Storage.Settings.savedMessages.Values.ToList()))
        //    {
        //        SavedMessages.Items.Add(message);
        //    }
        //}
    }
}
