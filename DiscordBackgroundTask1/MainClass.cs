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
using Windows.UI.Xaml;

namespace DiscordBackgroundTask1
{
    public sealed class MainClass : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
            _deferral = taskInstance.GetDeferral();

            // TODO: Insert code to start one or more asynchronous methods using the
            //       await keyword, for example:

            var timer = new System.Threading.Timer(new System.Threading.TimerCallback(timer_Tick), null, 0, 1000);

            Debug.WriteLine("Looping...");

            //await Task.Delay(-1);

            _deferral.Complete();
        }

        private void timer_Tick(object state)
        {
            Console.WriteLine(">10 seconds have passed.");
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
