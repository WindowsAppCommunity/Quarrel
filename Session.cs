using Discord_UWP.API;
using Discord_UWP.API.Channel;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.API.Gateway;
using Discord_UWP.API.Guild;
using Discord_UWP.API.Guild.Models;
using Discord_UWP.API.Invite;
using Discord_UWP.API.Login;
using Discord_UWP.API.Login.Models;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace Discord_UWP
{
    static class Session
    {
        public static async Task AutoLogin()
        {
            DiscordApiConfiguration config = new DiscordApiConfiguration
            {
                BaseUrl = "https://discordapp.com/api"
            };

            Token = Storage.Token;
            BasicRestFactory basicRestFactory = new BasicRestFactory(config);
            IAuthenticator authenticator = new DiscordAuthenticator(Token);
            AuthenticatedRestFactory = new AuthenticatedRestFactory(config, authenticator);
            IGatewayConfigService gatewayService = basicRestFactory.GetGatewayConfigService();

            SharedModels.GatewayConfig gateconfig = await gatewayService.GetGatewayConfig();
            Gateway = new Gateway.Gateway(gateconfig, authenticator);
        }

        public static async Task Login(string email, string password)
        {
            DiscordApiConfiguration config = new DiscordApiConfiguration
            {
                BaseUrl = "https://discordapp.com/api"
            };
            BasicRestFactory basicRestFactory = new BasicRestFactory(config);

            ILoginService loginService = basicRestFactory.GetLoginService();

            LoginRequest.Email = email;
            LoginRequest.Password = password;

            Loginresult = await loginService.Login(LoginRequest);
            Token = Loginresult.Token;
            IAuthenticator authenticator = new DiscordAuthenticator(Token);
            AuthenticatedRestFactory = new AuthenticatedRestFactory(config, authenticator);
            IGatewayConfigService gatewayService = basicRestFactory.GetGatewayConfigService();

            SharedModels.GatewayConfig gateconfig = await gatewayService.GetGatewayConfig();
            Gateway = new Gateway.Gateway(gateconfig, authenticator);
        }

       /*public static async Task CreateGuildGateway(string guildid)
        {
            DiscordApiConfiguration config = new DiscordApiConfiguration
            {
                BaseUrl = "https://discordapp.com/api/guilds/" + guildid 
            };

            BasicRestFactory basicRestFactory = new BasicRestFactory(config);
            IGatewayConfigService gatewayService = basicRestFactory.GetGatewayConfigService();
            IAuthenticator authenticator = new DiscordAuthenticator(token);

            SharedModels.GatewayConfig gateconfig = await gatewayService.GetGatewayConfig();
            guildgateway = new Gateway.Gateway(gateconfig, authenticator);
        }*/

        public static async Task<SharedModels.User> GetCurrentUser()
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.GetCurrentUser();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return Storage.Cache.CurrentUser.Raw;
        }

        public static async Task<IEnumerable<SharedModels.Connection>> GetUserConnections(string id)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.GetCurrentUserConnections();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.UserGuild>> GetGuilds()
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetCurrentUserGuilds();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static async Task<SharedModels.Guild> GetGuild(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuild(id);
            } catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.Guild();
            /*try
            {
                IGuildService guildservice = authenticatedRestFactory.GetGuildService();
                Task<SharedModels.Guild> guild_task = guildservice.GetGuild(id);
                guild_task.Wait();
                guild = guild_task.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }*/
        }

        public static async Task<IEnumerable<SharedModels.GuildChannel>> GetGuildData(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildChannels(id);
            } catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.GuildMember>> GetGuildMembers (string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ListGuildMemebers(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static SharedModels.GuildChannel GetGuildChannel(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                Task<SharedModels.GuildChannel> channelTask = channelservice.GetGuildChannel(id);
                channelTask.Wait();
                return channelTask.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.GuildChannel();
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelMessages(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessages(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
            /*try
            {
                IChannelService channelservice = authenticatedRestFactory.GetChannelService();
                Task<IEnumerable<SharedModels.Message>> message_task = channelservice.GetChannelMessages(id);
                message_task.Wait();
                messages = message_task.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }*/
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelMessagesBefore(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesBefore(id, msgpos);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }

            /*try
            {
                IChannelService channelservice = authenticatedRestFactory.GetChannelService();
                Task<IEnumerable<SharedModels.Message>> message_task = channelservice.GetChannelMessages(id);
                message_task.Wait();
                messages = message_task.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }*/
            return null;
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelPinnedMessages(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetPinnedMessages(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static void CreateMessage(string id, string text)
        {
            try
            {
                MessageUpsert message = new MessageUpsert();
                message.Content = text;
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.CreateMessage(id, message).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async void CreateMessage(string id, string text, Windows.Storage.StorageFile file)
        {
            try
            {
                MessageUpsert message = new MessageUpsert();
                message.Content = text;
                message.file = await Windows.Storage.FileIO.ReadTextAsync(file);
                 IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.CreateMessage(id, message).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void CreateReaction(string channelid, string messageid, SharedModels.Emoji emoji)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.CreateReaction(channelid, messageid, emoji.Name).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void DeleteReaction(string channelid, string messageid, SharedModels.Emoji emoji)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.DeleteReaction(channelid, messageid, emoji.Name).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void CreateChannel(string guildid, string name)
        {
            try
            {
                CreateGuildChannel cgc = new CreateGuildChannel();
                cgc.Bitrate = 64000;
                cgc.Name = name;
                cgc.Type = "text";
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.CreateGuildChannel(guildid, cgc).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void DeleteChannel(string chnid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.DeleteChannel(chnid).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void EditMessage(string chnid, string msgid, string content)
        {
            try
            {
                EditMessage editmessage = new EditMessage();
                editmessage.Content = content;
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.EditMessage(chnid, msgid, editmessage).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void PinMesage(string chnId, string msgId)
        {
            IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
            channelservice.AddPinnedChannelMessage(chnId, msgId);
        }

        public static void UnpinMessage(string chnId, string msgId)
        {

            IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
            channelservice.DeletePinnedChannelMessage(chnId, msgId);
        }

        public static async Task<SharedModels.Message> GetMessage(string chnid, string msgid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessage(chnid, msgid);            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.Message();
        }

        public static void DeleteMessage(string chnid, string msgid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.DeleteMessage(chnid, msgid).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async Task<IEnumerable<SharedModels.DirectMessageChannel>> GetDMs()
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                 return await userService.GetCurrentUserDirectMessageChannels();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static void ModifyCurrentUser(string newUsername)
        {
            try
            {
                ModifyUser modifyuser = new ModifyUser();
                modifyuser.Username = newUsername;
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                Task<SharedModels.User> userTask = userService.ModifyCurrentUser(modifyuser);
                userTask.Wait();
                Storage.Cache.CurrentUser = new CacheModels.User(userTask.Result);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void ModifyGuildChannel(string channelid, string newChannelName, string newChannelTopic)
        {
            try
            {
                ModifyChannel modifychannel = new ModifyChannel();
                modifychannel.Bitrate = 64000;
                modifychannel.Name = newChannelName;
                modifychannel.Topic = newChannelTopic;
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.ModifyChannel(channelid, modifychannel).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        /*public static void ModifyGuildChannelPositions(string channelid, int Position)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
               // guildservice.ModifyGuildChannelPositions(channelid, Position).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }*/

        public static void ModifyGuild(string guildid, string newName)
        {
            try
            {
                ModifyGuild modifyguild = new ModifyGuild();
                modifyguild.Name = newName;
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.ModifyGuild(guildid, modifyguild).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static SharedModels.GuildMember GetGuildMember(string guildid, string userid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                Task<SharedModels.GuildMember> memberTask = guildservice.GetGuildMemeber(guildid, userid);
                memberTask.Wait();
                return memberTask.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.GuildMember();
        }

        public static async Task TriggerTypingIndicator(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.TriggerTypingIndicator(channelId);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async void Showmsg(Exception e)
        {
            //MessageDialog msg = new MessageDialog("An error occured: " + e.Message + " You've either triggered an unexpected event, a feature has been implimented incorrectly or you're offline. I'm likely looking to fix this next update. You will also likely experience other unexpected behavior if necessary data was not recieved.");
            //MessageDialog msg = new MessageDialog("An error occured: " + e.InnerException.Message + " Please enter this message in the Feedback Hub (under \"Apps and Games\">\"Discord UWP\") along with how to recreate in the descrpition. Here's the rest of the data. Exception Message:" + e.Message + " HResult:" + e.HResult.ToString() + " Data:" + e.Data);

            if (Unlocked)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog("An error occured. You likely just don't have permission to perform that action. The developer is working on trying to prevent this from happening. The app may crash now, enjoy your day ;).");
                    await msg.ShowAsync();
                });
            }
        }

        public static void Logout()
        {
            AuthenticatedRestFactory = null;
            Unlocked = false;
        }

        public static string Token;
        public static List<SharedModels.TypingStart> Typers = new List<SharedModels.TypingStart>();
        public static AuthenticatedRestFactory AuthenticatedRestFactory;
        public static Gateway.Gateway Gateway;
        public static LoginResult Loginresult;
        public static LoginRequest LoginRequest = new LoginRequest();
        public static Dictionary<string, SharedModels.Presence> PrecenseDict = new Dictionary<string, SharedModels.Presence>();
        public static bool Unlocked;
        public static bool Online;
        public static bool SlowSpeeds;
        public static string Editcache;
    }
}
