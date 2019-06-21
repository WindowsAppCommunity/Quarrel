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
using Quarrel.LocalModels;
using DiscordAPI.API.Gateway;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class ListenOnSpotify : UserControl
    {
        /// <summary>
        /// ID of user sharing session
        /// </summary>
        string userid;

        /// <summary>
        /// ID of session
        /// </summary>
        string partyid;

        /// <summary>
        /// Setup UI
        /// </summary>
        /// <param name="user_id">ID of user sharing</param>
        /// <param name="party_id">Session ID</param>
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
            {
                UpdateUI(false);
            }
        }

        private void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
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

        /// <summary>
        /// Get AlbumArt from Asset Id
        /// </summary>
        /// <param name="id">Asset Id from Discord</param>
        /// <returns>AlbumArt url</returns>
        public string GetSpotifyImageLink(string id)
        {
            return "https://i.scdn.co/image/" + id.Remove(0, 8);
        }

        /// <summary>
        /// True if the Control is displaying a preview
        /// </summary>
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

        /// <summary>
        /// If this is the active session, update preview
        /// </summary>
        private async void SpotifyManager_SpotifyStateUpdated(object sender, EventArgs e)
        {
            if (preview)
            {
                try
                {
                    if (Managers.SpotifyManager.SpotifyState != null && Managers.SpotifyManager.SpotifyState.IsPlaying)
                    {
                        // Update details
                        UpdateUI(true, string.Join(", ", Managers.SpotifyManager.SpotifyState.Item.Artists.Select(x => x.Name)),
                            Managers.SpotifyManager.SpotifyState.Item.Name,
                            Managers.SpotifyManager.SpotifyState.Item.Album.Images.FirstOrDefault().Url);
                    }
                    else
                    {
                        // Session's over
                        UpdateUI(false);
                    }
                }
                catch (Exception)
                {
                    UpdateUI(false);
                }
            }
        }

        /// <summary>
        /// Update session details
        /// </summary>
        /// <param name="ongoing">False if the session is over</param>
        /// <param name="artists">The artist playing</param>
        /// <param name="title">The name of the track playing</param>
        /// <param name="albumart">Url of AlbumArt image</param>
        private async void UpdateUI(bool ongoing, string artists = "", string title = "", string albumart = "")
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                   () =>
                   {
                       if (ongoing)
                       {
                           // Update display
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
                           // Hide details, sessions over
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

        /// <summary>
        /// When control is unloaded
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        private void Dispose()
        {
            Managers.GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
            Managers.SpotifyManager.SpotifyStateUpdated -= SpotifyManager_SpotifyStateUpdated;
        }
    }
}
