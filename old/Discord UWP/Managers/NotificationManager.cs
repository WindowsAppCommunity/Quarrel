using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.UI.Notifications;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;

namespace Quarrel.Managers
{
    public class NotificationManager
    {
        public static void CreateMessageCreatedNotifcation(Message message)
        {

            TimeSpan VibrationDuration = TimeSpan.FromMilliseconds(100);
            bool muted = true;
            string ChnGldName = String.Empty;
            string ChnName = String.Empty;

            foreach (var guild in LocalState.Guilds.Values) //LocalState.GuildSettings wouldn't contain every channel
            {
                if (guild.channels.ContainsKey(message.ChannelId))
                {
                    ChnName = guild.channels[message.ChannelId].raw.Name;
                    ChnGldName = guild.Raw.Name + " - #" + ChnName;
                    if (LocalState.GuildSettings.ContainsKey(guild.Raw.Id) && LocalState.GuildSettings[guild.Raw.Id].channelOverrides.ContainsKey(message.ChannelId))
                    {
                        muted = LocalState.GuildSettings[guild.Raw.Id].raw.Muted && LocalState.GuildSettings[guild.Raw.Id].channelOverrides[message.ChannelId].Muted;
                    }
                    break;
                }
            }

            foreach (var dm in LocalState.DMs.Values)
            {
                if (dm.Id == message.ChannelId && ChnGldName == String.Empty)
                {
                    ChnGldName = dm.Name;
                    break;
                }
            }

            if (message.User.Id != LocalState.CurrentUser.Id && !muted)
            {

                if (Storage.Settings.LiveTile)
                {
                    //TODO find a better way of doing this
                    NotificationManager.UpdateDetailedStatus(message, ChnGldName);
                }
            }
        }

        public static void CreateCallNotification(Message message)
        {
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
                                Text = message.User.Username
                            },
                            new AdaptiveText()
                            {
                                Text = "Incoming Call"
                            },
                            new AdaptiveImage()
                            {
                                HintCrop = AdaptiveImageCrop.Circle,
                                Source = Common.AvatarString(message.User.Avatar, message.User.Id)
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Direct Message", "quarrel://channels/@me/"+message.User.Id)
                        {
                            ActivationType = ToastActivationType.Protocol,
                            ImageUri = "Assets/message.png"
                        },
                        new ToastButton("Ignore", "quarrel://call/decline/"+message.ChannelId)
                        {
                            ActivationType = ToastActivationType.Protocol,
                            ImageUri = "Assets/hangup.png"
                        },
                        new ToastButton("Answer", "quarrel://call/answer/"+message.ChannelId)
                        {
                            ActivationType = ToastActivationType.Protocol,
                            ImageUri = "Assets/call.png"
                        }
                    }
                },
                ActivationType = ToastActivationType.Protocol,
                Scenario = ToastScenario.IncomingCall,
                HintToastId = "IncomingCall"
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void CreateMentionNotification(string username, string avatar, string guildname, string channelname, string body, string channelid, string guildid, string messageid)
        {
            string toastTitle = username + " " + App.GetString("/Main/Notifications_sentMessageOn") + " #" + channelname;
            if (LocalState.Guilds[guildid].members.ContainsKey(LocalState.CurrentUser.Id))
                body = body.Replace("<@!" + LocalState.CurrentUser.Id + ">", "@" + LocalState.Guilds[guildid].members[LocalState.CurrentUser.Id].Nick);
            body = body.Replace("<@" + LocalState.CurrentUser.Id + ">", "@" + LocalState.CurrentUser.Username);
            
            ToastVisual visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                        {
                            new AdaptiveText()
                            {
                                Text = toastTitle
                            },
                            new AdaptiveText()
                            {
                                Text = body
                            },
                            /*new AdaptiveImage()
                            {
                                Source = imageurl
                            }*/
                        },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = avatar,
                                HintCrop = ToastGenericAppLogoCrop.Circle
                            }
                        }
                    };

        ToastTextBox replyContent = new ToastTextBox("Reply")
        {
            PlaceholderContent = App.GetString("/Main/Notifications_Reply"),
        };

        ToastActionsCustom actions = new ToastActionsCustom()
        {
            Inputs =
                    {
                        replyContent
                    },
            Buttons =
                    {
                        new ToastButton("Send", "send/" + channelid + "/"+messageid)
                        {
                            ActivationType = ToastActivationType.Background,
                            TextBoxId = replyContent.Id,
                            ImageUri = "Assets/sendicon.png",
                        }
                    }
        };

        ToastContent toastContent = new ToastContent()
        {
            Visual = visual,
            Actions = actions,
            // Arguments when the user taps body of toast
            Launch = "quarrel://channels/"+guildid+"/"+channelid,
            ActivationType= ToastActivationType.Protocol,
            HintToastId="Mention"
        };

        ToastNotification notification = new ToastNotification(toastContent.GetXml());
            notification.RemoteId = "Mention"+messageid;
            notification.Group = "Mention";
            notification.Tag = messageid;
        ToastNotificationManager.CreateToastNotifier().Show(notification);
    }
        static int previousvalue = -1;
        public static void SendBadgeNotification(int value)
        {
            if(value != previousvalue)
            {
                string xml1 = @"<badge value='";
                string xml2 = @"' />";
                var badgeXml = new Windows.Data.Xml.Dom.XmlDocument();
                badgeXml.LoadXml(xml1 + value.ToString() + xml2);

                var badge = new BadgeNotification(badgeXml);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                previousvalue = value;
            }
        }

        public static void SendBadgeNotification(BadgeGlyph value)
        {
            string xml1 = @"<badge value='";
            string xml2 = @"' />";
            var badgeXml = new Windows.Data.Xml.Dom.XmlDocument();
            badgeXml.LoadXml(xml1 + value.ToString() + xml2);

            var badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

            // Create a badge notification from the XML content.
            string payload = App.GetString("/TileTemplates/Iconic");
            var tileXml = new Windows.Data.Xml.Dom.XmlDocument();
            tileXml.LoadXml(payload);
            var badgeNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(badgeNotification);
        }

        public static void UpdateDetailedStatus(Message message, string name)
        {
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    LockDetailedStatus1 = message.User.Username,
                    LockDetailedStatus2 = name,
                    LockDetailedStatus3 = message.Content,

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = message.User.Username,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },

                                new AdaptiveText()
                                {
                                    Text = name,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = message.Content,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                }
            };

            var tileNotification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        public enum BadgeGlyph
        {
            none,
            activity,
            alarm,
            alert,
            attention,
            available,
            away,
            busy,
            error,
            newMessage,
            paused,
            playing,
            unavailable
        }
    }
}
