using Quarrel.RichPresence;
using Quarrel.RichPresence.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RichPresenceTester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        QuarrelRichPresenceService quarrelRichPresenceService = new QuarrelRichPresenceService();

        public MainPage()
        {
            this.InitializeComponent();
            Run();
            
        }

        public async void Run()
        {
            await ConnectService();
            await SetPresence();
        }

        public async Task ConnectService()
        {
            await quarrelRichPresenceService.TryConnectAsync();
        }

        public async Task SetPresence()
        {
            RichGame game = new RichGame("Test", Quarrel.RichPresence.Models.Enums.ActivityType.Playing);
            game.Details = "details";
            game.State = "state";
            await quarrelRichPresenceService.SetRawActivity(game);
        }
    }
}
