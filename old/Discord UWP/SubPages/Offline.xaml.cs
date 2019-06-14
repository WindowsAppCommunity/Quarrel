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
using Quarrel.Classes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel
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

        private StatusPageClasses.Index index = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                statusButton.Visibility = Visibility.Collapsed;
                OfflineReason.Text = "You aren't connected to the internet";
            }
            else
            {
                statusButton.Visibility = Visibility.Visible;
                index = (StatusPageClasses.Index) e.Parameter;
                if (index.Status.Indicator == "major_outage")
                {
                    OfflineReason.Text =
                        "You're connected to the internet, but Discord currently has a major outage :(";
                    OfflineReason.Foreground = (SolidColorBrush) Application.Current.Resources["dnd"];
                    statusButton.Background = (SolidColorBrush)Application.Current.Resources["dnd"];
                }
                else if(index.Status.Indicator == "partial_outage")
                {
                    OfflineReason.Text = "You're connected to the internet, but Discord currently has a partial outage :/";
                    OfflineReason.Foreground = (SolidColorBrush)Application.Current.Resources["idle"];
                    statusButton.Background = (SolidColorBrush)Application.Current.Resources["idle"];
                }
                else
                {
                    OfflineReason.Text = "You're connected to the internet, but we're having issues connecting to Discord :/";
                    OfflineReason.Foreground = (SolidColorBrush)Application.Current.Resources["Blurple"];
                    statusButton.Background = (SolidColorBrush)Application.Current.Resources["Blurple"];
                }
            }
        }

        private void TryLogin(object sender, RoutedEventArgs e)
        {
            App.LogIn();
        }
        public void AdjustSize()
        {
            try
            {
                var location = App.Splash.ImageLocation;
                viewbox.Width = location.Width;
                viewbox.Height = location.Height;

                //this.Focus(FocusState.Pointer);
                stack.Margin = new Thickness(0, location.Bottom, 0, 0);
            }
            catch (Exception)
            {

            }
        }
        //public async void loadMessages()
        //{
        //    foreach (var message in await MessageManager.ConvertMessage(Storage.Settings.savedMessages.Values.ToList()))
        //    {
        //        SavedMessages.Items.Add(message);
        //    }
        //}
        private void Offline_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustSize();
        }

        private void StatusButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (index != null)
            {
                Frame.Navigate(typeof(SubPages.DiscordStatus), index);
            }

            else
            {
                Frame.Navigate(typeof(SubPages.DiscordStatus), 1); //Just a dummy value so that the parameter != null
            }
        }
    }
}
