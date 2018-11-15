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

using Quarrel.RichPresence;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuarrelPresence : Page
    {
        public QuarrelPresence()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            QuarrelAppService service = new QuarrelAppService("511732870231097374");
            await service.TryConnectAsync();
            if (service.Status == Windows.ApplicationModel.AppService.AppServiceConnectionStatus.Success)
            {
                await service.SetActivity(new Game() { ApplicationId = "511732870231097374", Name = "myTube!", Type = ActivityType.Watching, Assets = new assets() { LargeImage = "mytube", LargeText = "myTube!"}, Details = "Not Actually", State = "Just testing out Rich Presence" });
            } else
            {

            }
        }
    }
}
