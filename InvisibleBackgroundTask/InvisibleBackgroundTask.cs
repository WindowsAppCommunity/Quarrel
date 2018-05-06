using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace InvisibleBackgroundTask
{
    public sealed class InvisibleBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance task)
        {
            var deferral = task.GetDeferral();
                    var details = task.TriggerDetails as Windows.UI.Notifications.ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        string[] segments = details.Argument.Split('/');
                        var count = segments.Count();
                if (count > 0)
                {
                    var creds = (new Windows.Security.Credentials.PasswordVault()).FindAllByResource("Token").FirstOrDefault();
                    creds.RetrievePassword();
                    var token = creds.Password;

                    if (segments[0] == "relationship")
                    {
                        if (count > 2)
                        {
                            if (segments[1] == "accept")
                            {
                                //accept friend request
                                using (var http = new System.Net.Http.HttpClient())
                                {
                                    System.Net.Http.HttpContent content = new System.Net.Http.StringContent("{}");
                                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);
                                    await http.PutAsync("https://discordapp.com/api/v6/users/@me/relationships/" + segments[2], content);
                                    ToastNotificationManager.History.Remove(segments[2], "relationship");
                                }
                            }
                            else if (segments[1] == "decline")
                            {
                                //decline friend request
                                using (var http = new System.Net.Http.HttpClient())
                                {
                                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);
                                    await http.DeleteAsync("https://discordapp.com/api/v6/users/@me/relationships/" + segments[2]);
                                    ToastNotificationManager.History.Remove(segments[2], "relationship");
                                }
                            }
                        }
                    }
                }         
            }
            ;
            deferral.Complete();
        }
    }
}
