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

namespace Discord_UWP.Managers
{
    public class NotifcationManager
    {
        public static void CreateMessageCreatedNotifcation(Message message)
        {
            TimeSpan VibrationDuration = TimeSpan.FromMilliseconds(100);
            bool muted = false;
            foreach (var guild in LocalState.Guilds.Values) //LocalState.GuildSettings wouldn't contain every channel
            {
                if (guild.channels.ContainsKey(message.ChannelId) && LocalState.GuildSettings[guild.Raw.Id].channelOverrides.ContainsKey(message.ChannelId))
                {
                    muted = LocalState.GuildSettings[guild.Raw.Id].raw.Muted || LocalState.GuildSettings[guild.Raw.Id].channelOverrides[message.ChannelId].Muted;
                }
            }

            if (message.User.Id != LocalState.CurrentUser.Id && !muted)
            {
                #region CreateContent
                string toastTitle = message.User.Username + " " + App.GetString("/Main/Notifications_sentMessageOn") + " #" +
                    LocalState.Guilds.FirstOrDefault(x => x.Value.channels.ContainsKey(message.ChannelId)).Value.channels[message.ChannelId].raw.Name;
                string content = message.Content;
                string userPhoto = "https://cdn.discordapp.com/avatars/" + message.User.Id + "/" + message.User.Avatar + ".jpg";
                string conversationId = message.ChannelId;
                #endregion

                if (Storage.Settings.Toasts)
                {
                    #region Toast
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
                    #endregion
                }

                #region Tile
                string tileTitle = message.User.Username + " " + App.GetString("/Main/Notifications_sentMessageOn") + " #" +
                        LocalState.Guilds.FirstOrDefault(x => x.Value.channels.ContainsKey(message.ChannelId)).Value.channels[message.ChannelId].raw.Name;

                TileContent tileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText() {
                                    Text = tileTitle,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },

                                /*new AdaptiveText()
                                {
                                    Text = message.,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },*/

                                new AdaptiveText()
                                {
                                    Text = message.Content,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                    new AdaptiveText() {
                                        Text = tileTitle,
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },

                                    /*new AdaptiveText()
                                    {
                                        Text = message.,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },*/

                                    new AdaptiveText()
                                    {
                                        Text = message.Content,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                        HintWrap = true
                                    }
                                }
                            }
                        },

                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                    new AdaptiveText() {
                                        Text = tileTitle,
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },

                                    /*new AdaptiveText()
                                    {
                                        Text = message.,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },*/

                                    new AdaptiveText()
                                    {
                                        Text = message.Content,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                        HintWrap = true
                                    }
                                }
                            }
                        }
                    }
                };

                // Create the tile notification
                var tileNotification = new TileNotification(tileContent.GetXml());

                // Send the notification to the primary tile
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
                #endregion
            }
        }
    }
}
