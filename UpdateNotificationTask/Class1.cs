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
            Text = "Discord UWP has been updated!"
        },

        new AdaptiveText()
        {
            Text = "There are lots of new features and tons of bug fixes, do you want to check it out?"
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
