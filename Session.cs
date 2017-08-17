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
using Discord_UWP.SharedModels;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace Discord_UWP
{
    static class Session
    {
        #region ILogin
        public static DiscordApiConfiguration config;
        public static async Task AutoLogin()
        {
            config = new DiscordApiConfiguration
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
        #endregion

        #region IUser

        #region CurrentUser

        #region Get
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

        public static async Task<User> GetCurrentUser()
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

        public static async Task<IEnumerable<SharedModels.Message>> GetRecentMentions(int limit, bool ShowRoles, bool ShowEveryone)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetRecentMentions(limit, ShowRoles, ShowEveryone);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static async Task<IEnumerable<DirectMessageChannel>> GetDMs()
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
        #endregion

        #region Set
        public static async void ChangeUserSettings(string settings)
        {
            try
            {
                await Task.Run(() =>
                {
                    Gateway.UpdateStatus(settings, 0, null);
                    IUserService userservice = AuthenticatedRestFactory.GetUserService();
                    userservice.UpdateSettings("{\"status\":\"" + settings + "\"}").Wait();
                });

            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async void ChangeCurrentGame(string game)
        {
            try
            {
                await Task.Run(() =>
                {
                    IUserService userservice = AuthenticatedRestFactory.GetUserService();
                    userservice.UpdateGame("{\"name\":\"" + game + "\"}").Wait();
                });
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
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

        public static void LeaveServer(string guildId)
        {
            IUserService userservice = AuthenticatedRestFactory.GetUserService();
            userservice.LeaveGuild(guildId).Wait();
        }
        #endregion

        #endregion

        #region Get
        public static async Task<User> GetUser(string userid)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.GetUser(userid);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new User();
        }

        public static async Task<UserProfile> GetUserProfile(string id)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetUserProfile(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.UserProfile();
        }

        public static async Task<IEnumerable<SharedFriend>> GetUserRelationShips(string id)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetUserReleations(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
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

        #endregion

        #region Set
        public static void SendFriendRequest(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                userservice.SendFriendRequest(userId).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void RemoveFriend(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                userservice.RemoveFriend(userId).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void BlockUser(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                userservice.BlockUser(userId).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void AddNote(string userid, string note)
        {
            try
            {
                IUserService channelservice = AuthenticatedRestFactory.GetUserService();
                channelservice.AddNote(userid, new Note() { note = note }).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void CreateDM(CreateDM createDM)
        {
            IUserService userservice = AuthenticatedRestFactory.GetUserService();
            userservice.CreateDirectMessageChannelForCurrentUser(createDM).Wait();
        }
        #endregion

        #endregion

        #region IGuild

        #region Get
        public static async Task<SharedModels.Guild> GetGuild(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuild(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.Guild();
        }

        public static async Task<IEnumerable<SharedModels.GuildChannel>> GetGuildData(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildChannels(id);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.GuildMember>> GetGuildMembers(string id)
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

        public static GuildMember GetGuildMember(string guildid, string userid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                Task<GuildMember> memberTask = guildservice.GetGuildMemeber(guildid, userid);
                memberTask.Wait();
                return memberTask.Result;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.GuildMember();
        }

        public static async Task<IEnumerable<Ban>> GetGuildBans(string guildId)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            return await guildservice.GetGuildBans(guildId);
        }
        #endregion

        #region Set
        public static void AckGuild(string guildId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.AckGuild(guildId).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void ModifyGuildRole(string guildId, string roleId, ModifyGuildRole newRole)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.ModifyGuildRole(guildId, roleId, newRole).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void CreateGuild(string name)
        {
            API.Guild.Models.CreateGuild guild = new API.Guild.Models.CreateGuild();
            guild.Name = name;
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.CreateGuild(guild);
        }

        public static void ModifyGuild(string guildid, ModifyGuild modifyguild)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.ModifyGuild(guildid, modifyguild).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static void DeleteGuild(string guildid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                guildservice.DeleteGuild(guildid).Wait();
            }
            catch { }
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

        public static void ModifyCurrentUserNickname(string guildId, string nickname)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.ModifyCurrentUserNickname(guildId, new ModifyGuildMember() { Nick = nickname });
        }

        public static void ModifyGuildMember(string guildId, string userId, ModifyGuildMember modify)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.ModifyGuildMember(guildId, userId, modify);
        }

        public static void ModifyGuildMemberNickname(string guildId, string userId, string nickname)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.ModifyGuildMemberNickname(guildId, userId, new ModifyGuildMember() { Nick = nickname });
        }

        public static void RemoveGuildMember(string guildId, string userId)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.RemoveGuildMember(guildId, userId);
        }

        public static void CreateBan(string guildId, string userId, CreateGuildBan guildBan)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.CreateGuildBan(guildId, userId, guildBan);
        }

        public static void RemoveBan(string guildId, string userId)
        {
            IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
            guildservice.RemoveGuildBan(guildId, userId);
        }
        #endregion

        #endregion

        #region IChannel

        #region Get
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
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelMessagesBefore(string id, string msgpos)
        {
            IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
            return await channelservice.GetChannelMessagesBefore(id, msgpos);
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

        public static async Task<Message> GetMessage(string chnid, string msgid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessage(chnid, msgid);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return new SharedModels.Message();
        }

        public static async Task<IEnumerable<Invite>> GetChannelInvites(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                var ch = await channelservice.GetChannelInvites(channelId);
                return ch;
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
            return null;
        }
        #endregion

        #region Set

        public static void ModifyGuildChannel(string chnId, ModifyChannel newChn)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                channelservice.ModifyChannel(chnId, newChn).Wait();
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async Task CreateMessage(string id, string text)
        {
            try
            {
                MessageUpsert message = new MessageUpsert();
                message.Content = text;
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.CreateMessage(id, message);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }


        public static HttpClient messageclient = new HttpClient();
        public static event Windows.Foundation.AsyncOperationProgressHandler<HttpResponseMessage, HttpProgress> MessageUploadProgress;
        
        public static async Task CreateMessage(string id, string text, Windows.Storage.StorageFile file)
        {
            try
            {
                MessageUpsert message = new MessageUpsert();
                message.Content = text;

                HttpMultipartFormDataContent content = new HttpMultipartFormDataContent("---------------------------7e11a60110a78");

                //content.Add(new HttpStringContent(message.Content), "content");
                content.Add(new HttpStringContent(Uri.EscapeUriString(JsonConvert.SerializeObject(message))), "payload_json");
                //content.Add(new HttpStringContent(message.TTS.ToString()), "tts");

                if (file != null)
                    content.Add(new HttpStreamContent(await file.OpenAsync(Windows.Storage.FileAccessMode.Read)), "file", file.Name);

                

                content.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("multipart/form-data; boundary=---------------------------7e11a60110a78");

                if (messageclient.DefaultRequestHeaders.Authorization == null)
                messageclient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue(Token);

                var send = messageclient.PostAsync(new Uri(config.BaseUrl + "/v6/channels/" + id + "/messages"), content);
                send.Progress = MessageUploadProgress;
                var resp = await send;
                if (resp.IsSuccessStatusCode)
                    id = "";
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async Task AckMessage(string chnId, string msgId)
        {
            try
            {
                await Task.Run(async () =>
                {
                    IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                    await channelservice.AckMessage(chnId, msgId);
                });
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async Task CreateReactionAsync(string channelid, string messageid, string emoji)
        {
            try
            {
                await Task.Run(() =>
                {
                    IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                    channelservice.CreateReaction(channelid, messageid, emoji).Wait();
                });
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
        }

        public static async Task DeleteReactionAsync(string channelid, string messageid, string emoji)
        {
            try
            {
                await Task.Run(() =>
                {
                    IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                    channelservice.DeleteReaction(channelid, messageid, emoji).Wait();
                });
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

        public static async void EditMessageAsync(string chnid, string msgid, string content)
        {
            try
            {
                await Task.Run(() =>
                {
                    EditMessage editmessage = new EditMessage();
                    editmessage.Content = content;
                    IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                    channelservice.EditMessage(chnid, msgid, editmessage);
                });
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

        public static async Task DeleteInvite(string channelId)
        {
            try
            {
                IInviteService channelservice = AuthenticatedRestFactory.GetInviteService();
                var ch = await channelservice.DeleteInvite(channelId);
            }
            catch (Exception e)
            {
                Showmsg(e);
            }
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

        public static async Task<Invite> CreateInvite(string chnId, CreateInvite invite)
        {
            IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
            return await channelservice.CreateChannelInvite(chnId, invite);
        }
        #endregion

        #endregion

        #region IInvite
        public static async Task<Invite> AcceptInvite(string code)
        {
            IInviteService inviteservice = AuthenticatedRestFactory.GetInviteService();
            return await inviteservice.AcceptInvite(code);
        }
        public static async Task<Invite> GetInvite(string code)
        {
            IInviteService inviteservice = AuthenticatedRestFactory.GetInviteService();
            return await inviteservice.GetInvite(code);
        }

        #endregion

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
        public static Dictionary<string, ReadState> RPC = new Dictionary<string, ReadState>();
        public static Dictionary<string, GuildSetting> GuildSettings = new Dictionary<string, GuildSetting>();

        public static AuthenticatedRestFactory AuthenticatedRestFactory;
        public static Gateway.Gateway Gateway;
        public static LoginResult Loginresult;
        public static LoginRequest LoginRequest = new LoginRequest();
        public static Dictionary<string, Presence> PrecenseDict = new Dictionary<string, Presence>();
        public static bool Unlocked;
        public static bool Online;
        public static bool SlowSpeeds;
        public static string Editcache;


    }
}
