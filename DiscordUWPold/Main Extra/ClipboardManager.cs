using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Discord_UWP
{
    /*<summary>
     An empty page that can be used on its own or navigated to within a Frame.
     </summary>*/

    public sealed partial class Main : Page
    {
        private void SetupClipbardWatcher()
        {
            Clipboard.ContentChanged += async (s, e) =>
            {
                await CheckClipboard();
            };
        }

        protected override async void OnGotFocus(RoutedEventArgs e)
        {
            await CheckClipboard();
            base.OnGotFocus(e);
        }

        string LastText = "";
        private async Task CheckClipboard()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string text = await dataPackageView.GetTextAsync();
                if (text == LastText) return;
                Regex type1 = new Regex(@"https?:\/\/(discord.gg|discordapp.com)\/(invite\/)?[\w_-]+");
                var match = type1.Match(text);
                if (match.Success)
                {
                    try
                    {
                        var invite = await Session.GetInvite(match.Value.Remove(0,match.Value.LastIndexOf('/')+1));

                        if (Storage.Cache.Guilds.ContainsKey(invite.Guild.Id)) return;

                        var toast = CreateToast(invite);
                        ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toast.GetXml()));
                    }
                    catch (Exception exception)
                    {
                        App.NavigateToBugReport(exception);
                    }
                }
                LastText = text;
            }
        }
        private ToastContent CreateToast(SharedModels.Invite invite)
        {
            string name = invite.Channel.Name;
            if (invite.Channel.Type == "text") name = "#" + name;
            string TopString = App.GetString("/Dialogs/JoinBTN/Content") + " " + name + "?";
            string BottomString = invite.Guild.Name;
            return new ToastContent()
            {
                Launch = "invite/" + invite.String,
                Scenario = ToastScenario.Default,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =

                        {
                            new AdaptiveText()
                            { Text = TopString },
                            new AdaptiveText()
                            { Text = BottomString }
                        }
                    }
                }
            };
       
        }
    }
}


