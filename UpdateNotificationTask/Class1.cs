using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace UpdateNotificationTask
{
    public sealed class UpdateTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            ToastContent content = new ToastContent();
            ToastVisual visual = new ToastVisual()
            {
BindingGeneric=    new ToastBindingGeneric()
            {
                Children =
    {
        new AdaptiveText()
        {
            Text = "Discord UWP has been rebranded, it's now called Quarrel!"
        },

        new AdaptiveText()
        {
            Text = "This update also has a few new features (including background notifications!). Wanna check it out?"
        }
                }
                }
            };
            content.Visual = visual;
            var toast = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
