using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace InvisibleBackgroundTask
{
    public sealed class Main : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            if (taskInstance.TriggerDetails is ToastNotificationActionTriggerDetail details)
                    {
                        string[] segments = details.Argument.Split('/');
                        var count = segments.Count();
                if (count > 0)
                {
                    var creds = (new Windows.Security.Credentials.PasswordVault()).FindAllByResource("Token").FirstOrDefault();
                    if (creds == null) return;
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
                    else if (segments[0] == "send")
                    {
                        using (var http = new System.Net.Http.HttpClient())
                        {
                            string response = CleanForJson(details.UserInput["Reply"].ToString());
                            if (response == null) return;
                            System.Net.Http.HttpContent content = new System.Net.Http.StringContent("{\"content\":\""+ response +"\"}");
                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);
                            await http.PostAsync("https://discordapp.com/api/v6/channels/" + segments[1] + "/messages", content);
                            
                            ToastNotificationManager.History.Remove(segments[2], "Mention");
                        }
                    }
                }         
            }
            deferral.Complete();
        }

        public static string CleanForJson(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                }
            }
            return sb.ToString();
        }
    }
}
