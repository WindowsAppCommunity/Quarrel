using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.UI.Notifications;
using System.Diagnostics;
using Discord_UWP;

namespace DiscordBackgroundTask1
{
    public sealed class MainClass : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //throw new NotImplementedException();
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
        }

        
        public static void SendToast(string message)
        {
            //build toast
            var template = ToastTemplateType.ToastText01;
            var xml = ToastNotificationManager.GetTemplateContent(template);
            var elements = xml.GetElementsByTagName("text");
            var text = xml.CreateTextNode(message);
            elements[0].AppendChild(text);
            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
