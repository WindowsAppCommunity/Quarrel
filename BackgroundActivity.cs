using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Discord_UWP.CacheModels;
using Discord_UWP.SharedModels;
#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion

namespace Discord_UWP
{
    class BackgroundActivity
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            EstablishGateway();
            loadCache();
            foreach (KeyValuePair<string, Guild> guild in Cache.Guilds)
            {
                foreach (KeyValuePair<string, GuildChannel> chn in Cache.Guilds[guild.Key].Channels)
                {
                    if (chn.Value.Raw.LastMessageId != newCache.Guilds[guild.Key].Channels[chn.Key].Raw.LastMessageId)
                    {
                        //In a real app, these would be initialized with actual data
                        string toastTitle = "New messages";
                        string content = "In" + chn.Value.Raw.Name + "on" + guild.Value.RawGuild.Name;
                        //string imageurl = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
                        //string userPhoto = "https://cdn.discordapp.com/avatars/" + e.EventData.User.Id + "/" + e.EventData.User.Avatar + ".jpg";
                        string conversationId = chn.Key;
                        // Construct the visuals of the toast
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
                                /*AppLogoOverride = new ToastGenericAppLogo()
                                {
                                    Source = userPhoto,
                                    HintCrop = ToastGenericAppLogoCrop.Circle
                                }*/
                            }
                        };
                        // Construct the actions for the toast (inputs and buttons)

                        /*ToastTextBox replyContent = new ToastTextBox("Reply")
                        {
                            PlaceholderContent = "Type a response"
                        };

                        ToastActionsCustom actions = new ToastActionsCustom()
                        {
                            Inputs =
                            {
                                replyContent
                            },
                            Buttons =
                            {
                                new ToastButton("Reply", "reply")
                                {
                                    ActivationType = ToastActivationType.Background,
                                    TextBoxId = replyContent.Id
                                }
                            }
                        };
                        */
                        // Now we can construct the final toast content
                        ToastContent toastContent = new ToastContent()
                        {
                            Visual = visual,
                            //Actions = actions,
                            // Arguments when the user taps body of toast
                            /*Launch = new QueryString()
                            {
                                { "action", "reply" },
                                { "conversationId", conversationId },
                                {"message", replyContent.Id }
                            }.ToString()*/
                        };
                        // And create the toast notification
                        ToastNotification notification = new ToastNotification(toastContent.GetXml());
                        // And then send the toast
                        ToastNotificationManager.CreateToastNotifier().Show(notification);

                    }
                }
            }
        }

        Cache Cache;
        Cache newCache;

        async void EstablishGateway()
        {
            Session.Gateway.Ready += OnReady;
            try
            {
                await Session.Gateway.ConnectAsync();
            }
            catch
            {

            }
        }


        private void OnReady(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.Ready> e)
        {
            foreach (DirectMessageChannel dm in e.EventData.PrivateChannels)
            {
                newCache.DMs.Add(dm.Id, new DmCache(dm));
            }
            
            foreach (SharedModels.Guild guild in e.EventData.Guilds)
            {
                if (!newCache.Guilds.ContainsKey(guild.Id))
                {
                    newCache.Guilds.Add(guild.Id, new Guild(guild));
                }
                
                foreach (GuildMember member in guild.Members)
                {
                    newCache.Guilds[guild.Id].Members.Add(member.User.Id, new Member(member));
                }
                
                foreach (SharedModels.GuildChannel chn in guild.Channels)
                {
                    SharedModels.GuildChannel channel = chn;
                    channel.GuildId = guild.Id;
                    newCache.Guilds[guild.Id].Channels.Add(chn.Id, new GuildChannel(channel));
                }
            }
        }

        async void loadCache()
        {
            try
            {
                StorageFile file = await Storage.SavedData.GetFileAsync("cache");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TempCache));
                    StringReader messageReader = new StringReader(await FileIO.ReadTextAsync(file));
                    Cache = new Cache((TempCache)serializer.Deserialize(messageReader));
                }
                catch
                {
                    await file.DeleteAsync();
                    //MessageDialog msg = new MessageDialog("You had a currupted cache, loading was slowed and cache as been reset");
                    //await msg.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg = new MessageDialog("You had no cache, the app will now start caching data to improve loading times");
                await msg.ShowAsync();
            }
        }
    }
}
