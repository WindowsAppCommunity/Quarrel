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
using Discord_UWP.Classes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscordStatus : Page
    {
        public class SimpleComponent
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public SolidColorBrush Color { get; set; }
        }
        public DiscordStatus()
        {
            this.InitializeComponent();
        }

        public SolidColorBrush ColorFromStatus(string status)
        {
            switch (status)
            {
                 case "operational": return (SolidColorBrush) Application.Current.Resources["online"];
                 case "partial_outage": return (SolidColorBrush)Application.Current.Resources["idle"];
            }
            return (SolidColorBrush)Application.Current.Resources["dnd"];
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadingRing.Visibility = Visibility.Visible;
            var status = await StatusPage.GetStatus();
            LoadingRing.Visibility = Visibility.Collapsed;
            if (status == null)
            {
                FailedToLoad.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                FailedToLoad.Visibility = Visibility.Collapsed;
            }

            foreach (var component in status.Components)
            {
                SimpleComponent sc = new SimpleComponent()
                {
                    Name = component.Name,
                    Status = component.Status.Replace("_", " "),
                    Color = ColorFromStatus(component.Status),
                    Description = component.Description
                };
                ComponentsPanel.Items.Add(sc);
                
            }
        }
    }
}
