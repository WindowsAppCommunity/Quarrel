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
        public void Setup(string party_id)
        {

        }
        public void SetupPreview()
        {
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                               () =>
                               {
                                   
                                   if (Managers.SpotifyManager.SpotifyState != null && Managers.SpotifyManager.SpotifyState.IsPlaying)
                                   {
                                       sessionOver.Visibility = Visibility.Collapsed;
                                       bgArtwork.Visibility = Visibility.Visible;
                                       contentGrid.Visibility = Visibility.Visible;
                                       trackTitle.Text = Managers.SpotifyManager.SpotifyState.Item.Name;
                                       artistTitle.Text = string.Join(", ", Managers.SpotifyManager.SpotifyState.Item.Artists.Select(x => x.Name));
                                       if (Managers.SpotifyManager.SpotifyState.Item.Album?.Images?.FirstOrDefault() != null)
                                       {
                                           var bmp = new BitmapImage(new Uri(Managers.SpotifyManager.SpotifyState.Item.Album.Images.FirstOrDefault().Url));
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
    }
}
