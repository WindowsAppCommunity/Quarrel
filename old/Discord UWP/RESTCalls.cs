using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Web.Http;
using System.Threading;
using DiscordAPI.API.Guild.Models;
using DiscordAPI.API;
using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Channel.Models;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Login;
using DiscordAPI.API.Login.Models;
using DiscordAPI.API.Misc;
using DiscordAPI.API.Misc.Models;
using DiscordAPI.API.User;
using DiscordAPI.API.User.Models;
using DiscordAPI.Authentication;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using ModifyUser = DiscordAPI.API.User.Models.ModifyUser;
using ModifyUserSettings = DiscordAPI.API.User.Models.ModifyUserSettings;
using SendFriendRequestResponse = DiscordAPI.API.User.Models.SendFriendRequestResponse;
using GuildSetting = DiscordAPI.SharedModels.GuildSetting;
using Connection = DiscordAPI.SharedModels.Connection;
using Guild = DiscordAPI.SharedModels.Guild;
using GuildChannel = DiscordAPI.SharedModels.GuildChannel;
using GuildMember = DiscordAPI.SharedModels.GuildMember;

namespace Quarrel
{
    public static class RESTCalls
    {
        #region ILogin
        static string EmailInUse;
        public static async Task<LoginResult> Login(string email, string password)
        {
            try
            {
                EmailInUse = email;
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

                if(LoginResult.MFA == false)
                {
                    
                    PasswordCredential credentials = new PasswordCredential("Token", email, LoginResult.Token);
                    Storage.PasswordVault.Add(credentials);
                }

                return LoginResult;
            }
            catch (Exception e)
            {
                return new LoginResult() { exception=e };
            }
        }
        public static async Task SetupToken(bool ignoregateway = false)
        {
            var credentials = Storage.PasswordVault.FindAllByResource("Token");

            var creds = credentials.FirstOrDefault(); //TODO: support multi-account storage
            foreach (var cred in credentials)
                if (cred.UserName == Storage.Settings.DefaultAccount)
                    creds = cred;
            creds.RetrievePassword();
            Token = creds.Password;

            config = new DiscordApiConfiguration
            {
                BaseUrl = "https://discordapp.com/api"
            };
            BasicRestFactory basicRestFactory = new BasicRestFactory(config);

            IAuthenticator authenticator = new DiscordAuthenticator(Token);
            AuthenticatedRestFactory = new AuthenticatedRestFactory(config, authenticator);

            //TODO: Maybe restructure gateway setup
            if (!ignoregateway)
            {
                IGatewayConfigService gatewayService = basicRestFactory.GetGatewayConfigService();

                GatewayConfig gateconfig = await gatewayService.GetGatewayConfig();

                Gateway.UseCompression = Storage.Settings.UseCompression;
                GatewayManager.Gateway = new Gateway(gateconfig, authenticator);
            }

        }
        public static async Task<SendSmsResult> SendLoginSms(string ticket)
        {
            try
            {
                config = new DiscordApiConfiguration
                {
                    BaseUrl = "https://discordapp.com/api"
                };
                BasicRestFactory basicRestFactory = new BasicRestFactory(config);
                ILoginService loginService = basicRestFactory.GetLoginService();
                return await loginService.SendSMS(new SendSmsRequest() { Ticket = ticket });
            }
            catch
            {
                return new SendSmsResult() { PhoneNumber = null };
            }
        }
        public static async Task<LoginResult> LoginSMS(string code, string ticket)
        {
            try
            {
                LoginResult LoginResult;
                LoginMFARequest loginRequest = new LoginMFARequest();
                config = new DiscordApiConfiguration
                {
                    BaseUrl = "https://discordapp.com/api"
                };
                BasicRestFactory basicRestFactory = new BasicRestFactory(config);


                ILoginService loginService = basicRestFactory.GetLoginService();

                loginRequest.Code = code;
                loginRequest.Ticket = ticket;

                LoginResult = await loginService.LoginSMS(loginRequest);

                if (LoginResult.Token != null)
                {
                    Token = LoginResult.Token;
                    PasswordCredential credentials = new PasswordCredential("Token", EmailInUse, LoginResult.Token);
                    Storage.PasswordVault.Add(credentials);
                }
                else
                {
                    return new LoginResult() { exception = new Exception("There was a problem authenticating you!") };
                }
                return LoginResult;
            }
            catch (Exception e)
            {
                return new LoginResult() { exception = e };
            }
        }
        public static async Task<LoginResult> LoginMFA(string code, string ticket)
        {
            try
            {
                LoginResult LoginResult;
                LoginMFARequest loginRequest = new LoginMFARequest();
                config = new DiscordApiConfiguration
                {
                    BaseUrl = "https://discordapp.com/api"
                };
                BasicRestFactory basicRestFactory = new BasicRestFactory(config);


                ILoginService loginService = basicRestFactory.GetLoginService();

                loginRequest.Code = code;
                loginRequest.Ticket = ticket;

                LoginResult = await loginService.LoginMFA(loginRequest);

                if (LoginResult.Token != null)
                {
                    Token = LoginResult.Token;
                    PasswordCredential credentials = new PasswordCredential("Token", EmailInUse, LoginResult.Token);
                    Storage.PasswordVault.Add(credentials);
                }
                else
                {
                    return new LoginResult() { exception = new Exception("There was a problem authenticating you!") };
                }
                return LoginResult;
            }
            catch (Exception e)
            {
                return new LoginResult() { exception = e };
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
        public static async Task<IEnumerable<UserGuild>> GetGuilds()
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetCurrentUserGuilds();
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return LocalState.CurrentUser;
        }

        public static async Task<IEnumerable<Message>> GetRecentMentions(int limit, bool ShowRoles, bool ShowEveryone)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.GetRecentMentions(limit, ShowRoles, ShowEveryone);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        #endregion

        #region Set
        public static async Task<UserSettings> ModifyUserSettings(ModifyUserSettings userSettings)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.UpdateSettings(userSettings);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return LocalState.Settings;
        }

        public static async Task<GuildSetting> ModifyGuildSettings(string guildId, GuildSettingModify guildSettings)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.ModifyGuildSettings(guildId, guildSettings);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return LocalState.GuildSettings[guildId].raw;
        }

        public static async Task ChangeUserStatus(string status)
        {
            try
            {
                await Task.Run( async () =>
                {
                    try
                    {
                        GatewayManager.Gateway.UpdateStatus(status, 0, null);
                        IUserService userservice = AuthenticatedRestFactory.GetUserService();
                        await userservice.UpdateSettings("{\"status\":\"" + status + "\"}");
                    }
                    catch /*(Exception exception)*/
                    {
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                });

            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<User> ModifyCurrentUser(ModifyUser modifyuser)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.ModifyCurrentUser(modifyuser);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                return null;
                //App.NavigateToBugReport(exception);
            }
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task LeaveServer(object guildId)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                await userservice.LeaveGuild((string)guildId);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
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
            catch (Exception exception)
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new UserProfile();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<Connection>> GetUserConnections(string id)
        {
            try
            {
                IUserService userService = AuthenticatedRestFactory.GetUserService();
                return await userService.GetCurrentUserConnections();
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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

        public static async Task<SendFriendRequestResponse> SendFriendRequest(string username, int discriminator)
        {
            try
            {
                IUserService userservice = AuthenticatedRestFactory.GetUserService();
                return await userservice.SendFriendRequest(new SendFriendRequest() { Username = username, Discriminator = discriminator});
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
                return null;
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
                App.CheckOnline();
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
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new DirectMessageChannel();
        }
        #endregion

        #endregion

        #region IGuild

        #region Get
        public static async Task<AuditLog> GetAuditLog(string id, string userId = null, int type = -1, int limit = 50, string before = null)
        {
            string args = "";

            if (userId != null)
            {
                args += args == "" ? "" : "&";
                args += "user_id={" + userId + "}";
            }

            if (type != -1)
            {
                args += args == "" ? "" : "&";
                args += "type={" + type.ToString() + "}";
            }

            args += args == "" ? "" : "&";
            args += "limit={" + limit.ToString() + "}";

            if (before != null)
            {
                args += args == "" ? "" : "&";
                args += "before={" + before.ToString() + "}";
            }

            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetAuditLog(id, args);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }

            return null;
        }
        public static async Task<Guild> GetGuild(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuild(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }
        public static async Task<IEnumerable<GuildChannel>> GetGuildChannels(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildChannels(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<GuildChannel>> GetGuildData(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.GetGuildChannels(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<GuildMember>> GetGuildMembers(string id)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ListGuildMemebers(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
            catch (Exception exception)
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new GuildMember();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<SearchResults> SearchGuild(string guildId, SearchArgs args, int offset)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.SearchGuildMessages(guildId, args.ConvertToArgs(), offset);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new SearchResults();
        }
        #endregion

        #region Set
        public static async Task AckGuild(string guildId)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.AckGuild(guildId);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Role();
        }

        public static async Task<Guild> CreateGuild(string name)
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }
        public static async Task<Guild> CreateGuild(object args)
        {
            try
            {
                CreateGuild guild = new CreateGuild();
                guild.Name = (string)(args as List<object>)[0];
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.CreateGuild(guild);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }

        public static async Task<Guild> ModifyGuild(string guildid, ModifyGuild modifyguild)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyGuild(guildid, modifyguild);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }

        public static async Task<Guild> DeleteGuild(string guildid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.DeleteGuild(guildid);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }
        public static async Task<Guild> DeleteGuild(object guildid)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.DeleteGuild((string)guildid);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Guild();
        }

        public static async Task<IEnumerable<GuildChannel>> ModifyGuildChannelPositions(string channelid, int Position)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyGuildChannelPositions(channelid, Position);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<GuildMember> ModifyCurrentUserNickname(string guildId, string nickname)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyCurrentUserNickname(guildId, new ModifyGuildMember(nickname));
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new GuildMember();
        }
        public static async Task<GuildMember> ModifyCurrentUserNickname(object args)
        {
            try
            {
                List<object> listArgs = (args as List<object>);
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.ModifyCurrentUserNickname((string)listArgs[0], new ModifyGuildMember((string)listArgs[1]));
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task ModifyGuildMemberNickname(string guildId, string userId, string nickname)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.ModifyGuildMemberNickname(guildId, userId, new IModifyGuildMember() { Nick = nickname });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }
        public static async Task ModifyGuildMemberNickname(object args)
        {
            try
            {
                List<object> listArgs = (args as List<object>);
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.ModifyGuildMemberNickname((string)listArgs[0], (string)listArgs[1], new IModifyGuildMember() { Nick = (string)listArgs[2] });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }
        public static async Task CreateBan(object args)
        {
            try
            {
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                await guildservice.CreateGuildBan((args as Tuple<string, string, CreateGuildBan>).Item1, (args as Tuple<string, string, CreateGuildBan>).Item2, (args as Tuple<string, string, CreateGuildBan>).Item3);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }
        #endregion

        #endregion

        #region IChannel

        #region Get
        public static async Task<GuildChannel> GetGuildChannel(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetGuildChannel(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new GuildChannel();
        }

        public static async Task<IEnumerable<Message>> GetChannelMessages(string id, int limit = 50)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessages(id, limit);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<Message>> GetChannelMessagesBefore(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesBefore(id, msgpos);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
       
        public static async Task<IEnumerable<Message>> GetChannelMessagesAround(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesAround(id, msgpos);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<Message>> GetChannelMessagesAfter(string id, string msgpos)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetChannelMessagesAfter(id, msgpos);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        public static async Task<IEnumerable<Message>> GetChannelPinnedMessages(string id)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                return await channelservice.GetPinnedMessages(id);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Message();
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
                App.CheckOnline();
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
                App.CheckOnline();
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
                App.CheckOnline();
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

                
                //content.Add(new HttpStringContent(Uri.EscapeUriString(JsonConvert.SerializeObject(message))), "payload_json");
                
                //content.Add(new HttpStringContent(message.TTS.ToString()), "tts");

                if (file != null)
                    content.Add(new HttpStreamContent(await file.OpenAsync(Windows.Storage.FileAccessMode.Read)), "file", file.Name);

                content.Add(new HttpStringContent(message.Content), "content");

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
                App.CheckOnline();
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
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task<GuildChannel> CreateChannel(string guildid, string name, string type = "text")
        {
            try
            {
                CreateGuildChannel cgc = new CreateGuildChannel();
                cgc.Bitrate = 64000;
                cgc.Name = name;
                cgc.Type = type;
                IGuildService guildservice = AuthenticatedRestFactory.GetGuildService();
                return await guildservice.CreateGuildChannel(guildid, cgc);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new GuildChannel();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }
        public static async Task DeleteChannel(object chnid)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeleteChannel((string)chnid);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                        App.CheckOnline();
                        //App.NavigateToBugReport(exception);
                    }
                    return new Message();
                });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new Message();
        }

        public static async Task PinMessage(string chnId, string msgId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.AddPinnedChannelMessage(chnId, msgId);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task DeleteMessage(object args)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeleteMessage((args as Tuple<string, string>).Item1, (args as Tuple<string, string>).Item2);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task StartCall(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.StartCall(channelId, new CallDetails() { Recipients = null });
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task DeclineCall(string channelId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.DeclineCall(channelId);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }

        public static async Task RemoveGroupUser(string channelId, string userId)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.RemoveGroupUser(channelId, userId);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
        }
        public static async Task RemoveGroupUser(object args)
        {
            try
            {
                IChannelService channelservice = AuthenticatedRestFactory.GetChannelService();
                await channelservice.RemoveGroupUser((args as Tuple<string, string>).Item1, (args as Tuple<string, string>).Item2);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
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

        #region IActivities

        #region Get
        public static async Task<IEnumerable<ActivityData>> GetActivites()
        {
            try
            {
                IActivitesService activiteservice = AuthenticatedRestFactory.GetActivitesService();
                return await activiteservice.GetActivites();
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        public static async Task<FeedSettings> GetFeedSettings()
        {
            try
            {
                IActivitesService activiteservice = AuthenticatedRestFactory.GetActivitesService();
                return await activiteservice.GetFeedSettings();
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        public static async Task<FeedSettings> AddFeedSubscriptions(IEnumerable<string> users, IEnumerable<string> games)
        {
            try
            {
                IActivitesService activiteservice = AuthenticatedRestFactory.GetActivitesService();
                return await activiteservice.PatchFeedSettings(new FeedPatch() { UserSubscriptions = users, GameSubscriptions = games });
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        public static async Task<List<GameNews>> GetGameNews(string[] ids)
        {
            try
            {
                IActivitesService activiteservice = AuthenticatedRestFactory.GetActivitesService();
                return await activiteservice.GetGameNews(String.Join(",", ids));
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
            return null;
        }
        #endregion

        #endregion

        #region Connections
        public static async Task<string> GetConnectionUrl(string service)
        {
            try
            {
                IConnectionsService connectionservice = AuthenticatedRestFactory.GetConnectionService();
                return (await connectionservice.GetOauthUrl(service)).Url;
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return "";
        }
        #endregion

        #region Games
        public static async Task<List<GameListItem>> GetGamelist()
        {
            try
            {
                IGameService gameservice = AuthenticatedRestFactory.GetGameService();
                return await gameservice.GetGames();
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return new List<GameListItem>();
        }
        #endregion

        #region Other

        public static async Task<IEnumerable<GifSearchResult>> SearchGiphy(string query)
        {
            try
            {
                IMiscService miscservice = AuthenticatedRestFactory.GetMiscService();
                return await miscservice.SearchGiphy(query);
            }
            catch /*(Exception exception)*/
            {
                App.CheckOnline();
                //App.NavigateToBugReport(exception);
            }
            return null;
        }

        /*public async Task<SearchResult> SearchGiphy(string query, int limit = 20, int offset = 0)
        {
            try
            {
                IGiphyService giphyService = GiphyAPI.GiphyAPI.GetGiphyService();
                return await giphyService.Search(query, limit, offset);
            }
            catch
            {
                App.CheckOnline();
            }
            return new SearchResult();
        }

        public async Task<SearchResult> GetTrendingGiphy(int limit = 20, int offset = 0)
        {
            try
            {
                IGiphyService giphyService = GiphyAPI.GiphyAPI.GetGiphyService();
                return await giphyService.Trending(limit, offset);
            }
            catch
            {
                App.CheckOnline();
            }
            return new SearchResult();
        }*/
        #endregion

        #region Random
        public static async Task<string> GetStringFromURI(Uri uri)
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(uri);
        }
        #endregion

        static AuthenticatedRestFactory AuthenticatedRestFactory;
        static DiscordApiConfiguration config;
        static string Token;
    }
}
