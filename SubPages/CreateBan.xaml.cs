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

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateBan : Page
    {
        public CreateBan()
        {
            this.InitializeComponent();
        }

        string userId = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            userId = e.Parameter.ToString();
            Message.Text = App.GetString("/Dialogs/VerifyBan") + LocalState.Guilds[App.CurrentGuildId].members[userId].User.Username + "?";
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            API.Guild.Models.CreateGuildBan GuildBan = new API.Guild.Models.CreateGuildBan() { DeleteMessageDays = 0};
            await RESTCalls.CreateBan(App.CurrentGuildId, userId, GuildBan); //TODO: Rig to App.Events
            CloseButton_Click(null, null);
        }
    }
}
