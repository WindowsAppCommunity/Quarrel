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
using Quarrel.Managers;

using DiscordAPI.SharedModels;
using DiscordAPI.API.Gateway;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Search : Page
    {
        public Search()
        {
            this.InitializeComponent();
        }

        private async void SearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResults.Items.Clear();
            List<string> guilds = new List<string>();
            foreach (var guild in LocalModels.LocalState.Guilds)
                guilds.Add(guild.Key);
            await GatewayManager.Gateway.Search(SearchQuery.Text, guilds, 20);
            GatewayManager.Gateway.GuildMemberChunk += Gateway_GuildMemberChunk;
        }

        private void Gateway_GuildMemberChunk(object sender, GatewayEventArgs<GuildMemberChunk> e)
        {
            foreach (var member in e.EventData.Members)
                SearchResults.Items.Add(member.User.Username);
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
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
    }
}
