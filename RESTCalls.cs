using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Web.Http;

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

using Discord_UWP.SharedModels;
using Discord_UWP.LocalModels;

using Discord_UWP.Managers;

namespace Discord_UWP
{
    public class RESTCalls
    {
        #region ILogin
        public static async Task<Exception> Login(string email, string password)
        {
            try
            {
                LoginResult LoginResult;
                LoginRequest loginRequest = new LoginRequest();
                config = new DiscordApiConfiguration
                {
                    BaseUrl = "https://discordapp.com/api"
                };
                BasicRestFactory basicRestFactory = new BasicRestFactory(config);


                ILoginService loginService = basicRestFactory.GetLoginService();

                loginRequest.Email = email;
                loginRequest.Password = password;

                LoginResult = await loginService.Login(loginRequest);
                Token = LoginResult.Token;
                IAuthenticator authenticator = new DiscordAuthenticator(Token);
                AuthenticatedRestFactory = new AuthenticatedRestFactory(config, authenticator);

                //TODO: Maybe restructure gateway setup
                IGatewayConfigService gatewayService = basicRestFactory.GetGatewayConfigService();

                SharedModels.GatewayConfig gateconfig = await gatewayService.GetGatewayConfig();
                GatewayManager.Gateway = new Gateway.Gateway(gateconfig, authenticator);

                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        //public static async Task<bool> LoginOauth2()
        //{
        //    string url = "https://discordapp.com/api/oauth2/authorize?response_type=code&client_id=" + App.ClientId;
        //    try
        //    {
        //        throw new Exception();
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return LocalState.CurrentUser;
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetRecentMentions(int limit, bool ShowRoles, bool ShowEveryone)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetRecentMentions(limit, ShowRoles, ShowEveryone);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        #endregion

        #region Set
        public static async Task<SharedModels.GuildSetting> ModifyGuildSettings(string guildId, SharedModels.GuildSetting guildSettings)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.ModifyGuildSettings(guildId, guildSettings);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return guildSettings;
        }

        public static async Task ChangeUserSettings(string settings)
        {
            try
            {
                await Task.Run( async () =>
                {
                    try
                    {
                        GatewayManager.Gateway.UpdateStatus(settings, 0, null);
                        IUserService userservice = AuthenticatedRestFactory.GetUserService();
                        await userservice.UpdateSettings("{\"status\":\"" + settings + "\"}");
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                });

            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task ChangeCurrentGame(string game)
        {
            try
            {
                await Task.Run( async () =>
                {
                    try
                    {
                        IUserService userservice = AuthenticatedRestFactory.GetUserService();
                        await userservice.UpdateGame("{\"name\":\"" + game + "\"}");
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<User> ModifyCurrentUser(string newUsername)
        {
            try
            {
                ModifyUser modifyuser = new ModifyUser();
                modifyuser.Username = newUsername;
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.ModifyCurrentUser(modifyuser);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new User();
        }

        public static async Task LeaveServer(string guildId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                await userservice.LeaveGuild(guildId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        #endregion

        #region Set
        public static async Task SendFriendRequest(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                await userservice.SendFriendRequest(userId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task RemoveFriend(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                await userservice.RemoveFriend(userId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task BlockUser(string userId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                await userservice.BlockUser(userId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task AddNote(string userid, string note)
        {
            try
            {
                IUserService channelservice = AuthenticatedRestFactory.GetUserService();
                await channelservice.AddNote(userid, new Note() { note = note });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<DirectMessageChannel> CreateDM(CreateDM createDM)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.CreateDirectMessageChannelForCurrentUser(createDM);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new DirectMessageChannel();
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<GuildMember> GetGuildMember(string guildid, string userid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildMemeber(guildid, userid);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.GuildMember();
        }

        public static async Task<IEnumerable<Ban>> GetGuildBans(string guildId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildBans(guildId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        #endregion

        #region Set
        public static async Task<SharedModels.GuildChannel> AckGuild(string guildId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.AckGuild(guildId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.GuildChannel();
        }

        public static async Task<Role> ModifyGuildRole(string guildId, string roleId, ModifyGuildRole newRole)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyGuildRole(guildId, roleId, newRole);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Role();
        }

        public static async Task<SharedModels.Guild> CreateGuild(string name)
        {
            try
            {
                CreateGuild guild = new CreateGuild();
                guild.Name = name;
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.CreateGuild(guild);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.Guild();
        }

        public static async Task<SharedModels.Guild> ModifyGuild(string guildid, ModifyGuild modifyguild)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyGuild(guildid, modifyguild);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.Guild();
        }

        public static async Task<SharedModels.Guild> DeleteGuild(string guildid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.DeleteGuild(guildid);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.Guild();
        }

        public static async Task<IEnumerable<SharedModels.GuildChannel>> ModifyGuildChannelPositions(string channelid, int Position)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyGuildChannelPositions(channelid, Position);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<GuildMember> ModifyCurrentUserNickname(string guildId, string nickname)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyCurrentUserNickname(guildId, new ModifyGuildMember() { Nick = nickname });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new GuildMember();
        }

        public static async Task ModifyGuildMember(string guildId, string userId, ModifyGuildMember modify)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.ModifyGuildMember(guildId, userId, modify);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task ModifyGuildMemberNickname(string guildId, string userId, string nickname)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.ModifyGuildMemberNickname(guildId, userId, new ModifyGuildMember() { Nick = nickname });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task RemoveGuildMember(string guildId, string userId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.RemoveGuildMember(guildId, userId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task CreateBan(string guildId, string userId, CreateGuildBan guildBan)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.CreateGuildBan(guildId, userId, guildBan);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task RemoveBan(string guildId, string userId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.RemoveGuildBan(guildId, userId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }
        #endregion

        #endregion

        #region IChannel

        #region Get
        public static async Task<SharedModels.GuildChannel> GetGuildChannel(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetGuildChannel(id);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelMessagesBefore(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesBefore(id, msgpos);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelMessagesAfter(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesAfter(id, msgpos);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<SharedModels.Message>> GetChannelPinnedMessages(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetPinnedMessages(id);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.Message();
        }

        public static async Task<IEnumerable<Invite>> GetChannelInvites(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelInvites(channelId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        #endregion

        #region Set

        public static async Task ModifyGuildChannel(string chnId, ModifyChannel newChn)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.ModifyChannel(chnId, newChn);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<Message> CreateMessage(string channelId, MessageUpsert message)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.CreateMessage(channelId, message);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Message();
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
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task AckMessage(string chnId, string msgId)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                        await channelservice.AckMessage(chnId, msgId);
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task CreateReactionAsync(string channelid, string messageid, string emoji)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                        await channelservice.CreateReaction(channelid, messageid, emoji);
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task DeleteReactionAsync(string channelid, string messageid, string emoji)
        {
            try
            {
                await Task.Run( async () =>
                {
                    try
                    {
                        IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                        await channelservice.DeleteReaction(channelid, messageid, emoji);
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<SharedModels.GuildChannel> CreateChannel(string guildid, string name)
        {
            try
            {
                CreateGuildChannel cgc = new CreateGuildChannel();
                cgc.Bitrate = 64000;
                cgc.Name = name;
                cgc.Type = "text";
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.CreateGuildChannel(guildid, cgc);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new SharedModels.GuildChannel();
        }

        public static async Task DeleteChannel(string chnid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeleteChannel(chnid);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<Message> EditMessageAsync(string chnid, string msgid, string content)
        {
            try
            {
                return await Task.Run( async () =>
                {
                    try
                    {
                        EditMessage editmessage = new EditMessage();
                        editmessage.Content = content;
                        IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                        return await channelservice.EditMessage(chnid, msgid, editmessage);
                    }
                    catch /*(Exception exception)*/
                    {
                        //App.NavigateToBugReport(exception);
                    }
                    return new Message();
                });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Message();
        }

        public static async Task PinMesage(string chnId, string msgId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.AddPinnedChannelMessage(chnId, msgId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task UnpinMessage(string chnId, string msgId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeletePinnedChannelMessage(chnId, msgId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task DeleteMessage(string chnid, string msgid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeleteMessage(chnid, msgid);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task TriggerTypingIndicator(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.TriggerTypingIndicator(channelId);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }
        #endregion

        #endregion

        #region IInvite

        #region Get
        public static async Task<Invite> GetInvite(string code)
        {
            try
            {
                IInviteService inviteservice = AuthenticatedRestFactory.GetInviteService();
                return await inviteservice.GetInvite(code);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Invite();
        }
        #endregion

        #region Set
        public static async Task<Invite> AcceptInvite(string code)
        {
            try
            {
                IInviteService inviteservice = AuthenticatedRestFactory.GetInviteService();
                return await inviteservice.AcceptInvite(code);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Invite();
        }

        public static async Task<Invite> CreateInvite(string chnId, CreateInvite invite)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.CreateChannelInvite(chnId, invite);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Invite();
        }
        
        public static async Task<Invite> DeleteInvite(string inviteCode)
        {
            try
            {
                IInviteService channelservice = AuthenticatedRestFactory.GetInviteService();
                return await channelservice.DeleteInvite(inviteCode);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return new Invite();
        }
        #endregion

        #endregion

        static AuthenticatedRestFactory AuthenticatedRestFactory;
        static DiscordApiConfiguration config;
        static string Token;
    }
}
