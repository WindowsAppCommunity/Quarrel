using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.UI.Notifications;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace Discord_UWP.Managers
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
                #region CreateContent
                string toastTitle = message.User.Username + " " + App.GetString("/Main/Notifications_sentMessageOn") + " #" + ChnName;
                string content = message.Content;
                string userPhoto = "https://cdn.discordapp.com/avatars/" + message.User.Id + "/" + message.User.Avatar + ".jpg";
                string conversationId = message.ChannelId;
                #endregion

                //if (Storage.Settings.Toasts)
                //{
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
                                Text = content
                            },
                            /*new AdaptiveImage()
                            {
                                Source = imageurl
                            }*/
                        },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = userPhoto,
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
                        new ToastButton("Send",  new QueryString()
                    {
                        { "action", "SendMessage" },
                        { "channelid", conversationId },
                        { "content", replyContent.Id }
                    }.ToString())
                        {
                            ActivationType = ToastActivationType.Foreground,
                            TextBoxId = replyContent.Id,
                            ImageUri = "Assets/sendicon.png"
                        }
                    }
                    };

                    ToastContent toastContent = new ToastContent()
                    {
                        Visual = visual,
                        //Actions = actions, //TODO: Actions
                        // Arguments when the user taps body of toast
                        Launch = new QueryString()
                    {
                        { "action", "Navigate" },
                        { "page", "Channel" },
                        { "channelid", replyContent.Id }
                    }.ToString()
                    };

                    ToastNotification notification = new ToastNotification(toastContent.GetXml());

                    ToastNotificationManager.CreateToastNotifier().Show(notification);
                //}

                if (Storage.Settings.LiveTile)
                {
                    //TODO find a better way of doing this
                    NotificationManager.UpdateDetailedStatus(message, ChnGldName);
                }
            }
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

                // Create a badge notification from the XML content.
                string payload = App.GetString("/TileTemplates/Iconic");
                var tileXml = new Windows.Data.Xml.Dom.XmlDocument();
                tileXml.LoadXml(payload);
                var badgeNotification = new TileNotification(tileXml);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(badgeNotification);
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
