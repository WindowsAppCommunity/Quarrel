using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace DiscordBackgroundTask1
{
    public sealed class SendToast
    {
        public static void Default(string message)
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

        public static void UnreadDM(string channelstr, int count, string lastmessage)
        {
            PrivateChannel channel = Newtonsoft.Json.JsonConvert.DeserializeObject<PrivateChannel>(channelstr);
            string imageurl = "";
            string text = "";
            if(channel.type == 1)
            {
                text = channel.recipients.FirstOrDefault().username;
                imageurl = "https://cdn.discordapp.com/avatars/" + channel.recipients.FirstOrDefault().id + "/" + channel.recipients.FirstOrDefault().avatar + ".png";
            }     
            else if(channel.type == 3)
            {
                if (string.IsNullOrEmpty(channel.name))
                    text = String.Join(",", channel.recipients.Select(x => x.username));
                else text = channel.name;
                if (channel.icon == null)
                    imageurl = "https://discordapp.com/assets/f046e2247d730629309457e902d5c5b3.svg";
                else
                    imageurl = "https://cdn.discordapp.com/channel-icons/" + channel.id + "/" + channel.icon + ".png";
            }

            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                        new AdaptiveText()
                        {
                            Text = text
                        },
                        new AdaptiveText()
                        {
                            Text = count.ToString() + " missed messages"
                        }
                    },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = imageurl,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                DisplayTimestamp = SnowflakeToTime(lastmessage),
                ActivationType = ToastActivationType.Protocol,
                Launch = "quarrel://channels/@me/" + channel.id
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void NewMention(string icon, string guildid, string guildname, string channelname, string channelid, int count, string lastmessage)
        {
            string imageurl = "https://cdn.discordapp.com/icons/" + guildid + "/" + icon +".png";
            string text = "";
            if (count > 1)
                text = count + " new mentions in #" + channelname;
            else
                text = "You have been mentioned in #" + channelname;

            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                        new AdaptiveText()
                        {
                            Text = guildname
                        },
                        new AdaptiveText()
                        {
                            Text = text
                        }
                    },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = imageurl,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                DisplayTimestamp = SnowflakeToTime(lastmessage),
                ActivationType = ToastActivationType.Protocol,
                Launch = "quarrel://channels/"+guildid+"/"+channelid
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }


        public static void FriendRequest(string username, string avatar, string userid, string relationshipid)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children = {
                        new AdaptiveText() {
                            Text = username
                        },
                        new AdaptiveText()
                        {
                            Text = "Sent you a friend request"
                        }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "https://cdn.discordapp.com/avatars/" + userid + "/" + avatar + ".png",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons = {
                         new ToastButton("Accept", "relationship/accept/"+relationshipid) {
                            ActivationType = ToastActivationType.Background,
                            ActivationOptions = new ToastActivationOptions(){ AfterActivationBehavior=ToastAfterActivationBehavior.PendingUpdate }
                         },
                         new ToastButton("Decline", "relationship/decline/="+relationshipid) {
                            ActivationType = ToastActivationType.Background,
                            ActivationOptions = new ToastActivationOptions(){ AfterActivationBehavior=ToastAfterActivationBehavior.PendingUpdate }
                         }
                    }
                },
                ActivationType = ToastActivationType.Protocol,
                Launch = "quarrel://friendrequests"
            };
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.Tag = relationshipid;
            toastNotif.RemoteId = "relationship" + relationshipid;
            toastNotif.Group = "relationship";
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
        public static DateTimeOffset SnowflakeToTime(string id)
        {
            //returns unix time in ms
            if (String.IsNullOrEmpty(id)) return new DateTimeOffset();
            return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64((double)((Convert.ToInt64(id) / (4194304)) + 1420070400000)));
        }

    }
}