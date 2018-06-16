using Discord_UWP.LocalModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class ListenOnSpotify : UserControl
    {
        string userid;
        string partyid;
        public void Setup(string user_id, string party_id)
        {
            userid = user_id;
            if (LocalState.PresenceDict.ContainsKey(user_id) && LocalState.PresenceDict[user_id].Game != null)
            {
                var presence = LocalState.PresenceDict[user_id].Game;
                if (presence.Party != null && presence.Party.Id == party_id)
                {
                    UpdateUI(true, presence.State, presence.Details, GetSpotifyImageLink(presence.Assets.LargeImage));
                }
                else
                    UpdateUI(false);
            }
            else
                UpdateUI(false);
            //TODO fix this memory leak
            Managers.GatewayManager.Gateway.PresenceUpdated += Gateway_PresenceUpdated;
        }

        private void Gateway_PresenceUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Presence> e)
        {
            if (!preview)
            {
                if (e.EventData.User.Id == userid)
                {
                    if (e.EventData.Game != null && e.EventData.Game.Type == 3 && e.EventData.Game.Party != null && e.EventData.Game.Party.Id == partyid)
                        UpdateUI(true, e.EventData.Game.State, e.EventData.Game.Details, GetSpotifyImageLink(e.EventData.Game.Assets.LargeImage));
                    else
                        UpdateUI(false);
                }
            }
        }
        public string GetSpotifyImageLink(string id)
        {
            return "https://i.scdn.co/image/" + id.Remove(0, 8);
        }
        bool preview = false;
        public void SetupPreview()
        {
            preview = true;
            if(Managers.SpotifyManager.SpotifyState != null)
            {
                Managers.SpotifyManager.SpotifyStateUpdated += SpotifyManager_SpotifyStateUpdated;
                SpotifyManager_SpotifyStateUpdated(null, null);
            }
            else
            {
                sessionOver.Visibility = Visibility.Collapsed;
                bgArtwork.Visibility = Visibility.Visible;
                contentGrid.Visibility = Visibility.Visible;
            }
        }

        private async void SpotifyManager_SpotifyStateUpdated(object sender, EventArgs e)
        {
            if (preview)
            {
                try
                {
                    if (Managers.SpotifyManager.SpotifyState != null && Managers.SpotifyManager.SpotifyState.IsPlaying)
                        UpdateUI(true, string.Join(", ", Managers.SpotifyManager.SpotifyState.Item.Artists.Select(x => x.Name)),
                            Managers.SpotifyManager.SpotifyState.Item.Name,
                            Managers.SpotifyManager.SpotifyState.Item.Album.Images.FirstOrDefault().Url);
                    else
                        UpdateUI(false);
                }
                catch (Exception)
                {
                    UpdateUI(false);
                }
            }
        }

        private async void UpdateUI(bool visible, string artists = "", string title = "", string albumart = "")
        {
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                   () =>
                   {
                       if (visible)
                       {
                           sessionOver.Visibility = Visibility.Collapsed;
                           bgArtwork.Visibility = Visibility.Visible;
                           contentGrid.Visibility = Visibility.Visible;
                           trackTitle.Text = title;
                           artistTitle.Text = artists;
                           if (albumart != null)
                           {
                               var bmp = new BitmapImage(new Uri(albumart));
                               artwork.Source = bmp;
                               bgArtwork.Source = bmp;
                               bgArtwork.Blur(12, 0).Start();
                           }
                       }
                       else
                       {
                           sessionOver.Visibility = Visibility.Visible;
                           bgArtwork.Visibility = Visibility.Collapsed;
                           contentGrid.Visibility = Visibility.Collapsed;
                       }
                   });
        }

        public ListenOnSpotify()
        {
            this.InitializeComponent();
        }

        public void Dipose()
        {
            Managers.GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
            Managers.SpotifyManager.SpotifyStateUpdated -= SpotifyManager_SpotifyStateUpdated;
        }
    }
}
