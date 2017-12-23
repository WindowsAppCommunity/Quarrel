using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Discord_UWP.Gateway.DownstreamEvents;
using Microsoft.Toolkit.Uwp;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;

using Discord_UWP.Managers;
using Windows.Foundation.Metadata;

namespace Discord_UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            LoadSettings();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        /// <summary>
        /// This is a task that is executed after the gateway is loaded, and so the app fully loaded and ready to go
        /// </summary>
        public static EventArgs PostLoadTaskArgs;
        public static string PostLoadTask;

        #region Publics
        #region Events

        #region Startup
        public static event EventHandler LoggingInHandler;
        public static void LoggingIn()
        {
            LoggingInHandler?.Invoke(typeof(App), new EventArgs());
            App.GatewayCreated = true;
        }

        public static event EventHandler ReadyRecievedHandler;
        public static void ReadyRecieved()
        {
            ReadyRecievedHandler?.Invoke(typeof(App), new EventArgs());
        }
        #endregion

        #region Logout
        public static event EventHandler LogOutHandler;
        public static void LogOut()
        {
            LogOutHandler?.Invoke(typeof(App), new EventArgs());
        }
        #endregion

        #region General
        public class ProfileNavigationArgs : EventArgs
        {
            public SharedModels.User User { get; set; }
        }
        #endregion

        #region Flyouts
        public class MenuArgs : EventArgs
        {
            //public Type Type { get; set; }
            //public string Id { get; set; }
            //public string ParentId { get; set; }
            public MenuFlyout Flyout { get; set; }
            public Point Point { get; set; }
        }
        public static event EventHandler<MenuArgs> MenuHandler;
        public static void ShowMenuFlyout(object sender, FlyoutManager.Type type, string Id, string parentId, Point point)
        {
            MenuHandler?.Invoke(sender, new MenuArgs() {Flyout = FlyoutManager.ShowMenu(type, Id, parentId), Point = point});
        }

        public static event EventHandler<ProfileNavigationArgs> ShowMemberFlyoutHandler;
        public static void ShowMemberFlyout(object sender, SharedModels.User user)
        {
            ShowMemberFlyoutHandler?.Invoke(sender, new ProfileNavigationArgs() { User = user });
        }
        #endregion

        #region Link
        /// <summary>
        /// Fired when a link element in the markdown was tapped.
        /// </summary>
        public static event EventHandler<MarkdownTextBlock.LinkClickedEventArgs> LinkClicked;
        public static void FireLinkClicked(MarkdownTextBlock.LinkClickedEventArgs LinkeventArgs)
        {
            LinkClicked?.Invoke(typeof(App), LinkeventArgs);
        }
        #endregion

        #region Navigation

        #region Guild
        public class GuildNavigationArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<GuildNavigationArgs> NavigateToGuildHandler;
        public static void NavigateToGuild(string guildId)
        {
            NavigateToGuildHandler?.Invoke(typeof(App), new GuildNavigationArgs() { GuildId = guildId });
        }

        #endregion

        #region GuildChannel
        public class GuildChannelNavigationArgs : EventArgs
        {
            public string GuildId { get; set; }
            public string ChannelId { get; set; }
            public string Message { get; set; }
            public bool Send { get; set; }
            public bool OnBack { get; set; }
        }

        public static event EventHandler<GuildChannelNavigationArgs> NavigateToGuildChannelHandler;
        public static Task NavigateToGuildChannel(string guildId, string channelId, string message = null, bool send = false, bool onBack = false)
        {
            NavigateToGuildChannelHandler?.Invoke(typeof(App), new GuildChannelNavigationArgs() { GuildId = guildId, ChannelId = channelId, Message = message, Send = send, OnBack = onBack });
            return null;
        }
        #endregion

        #region DMChannel
        public class DMChannelNavigationArgs : EventArgs
        {
            public string ChannelId { get; set; }
            public string UserId { get; set; }
            public string Message { get; set; }
            public bool Send { get; set; }
            public bool OnBack { get; set; }
        }
        public static event EventHandler<DMChannelNavigationArgs> NavigateToDMChannelHandler;
        public static void NavigateToDMChannel(string channelId, string message = null, bool send = false, bool onBack = false)
        {
            NavigateToDMChannelHandler?.Invoke(typeof(App), new DMChannelNavigationArgs() { ChannelId = channelId, Message = message, Send = send, OnBack = onBack });
        }
        public static void NavigateToDMChannel(string channelId, string userId, string message = null, bool send = false, bool onBack = false)
        {
            NavigateToDMChannelHandler?.Invoke(typeof(App), new DMChannelNavigationArgs() { ChannelId = channelId, UserId = userId, Message = message, Send = send, OnBack = onBack });
        }
        #endregion

        public static event EventHandler NavigateToLoginHandler;
        public static void NavigateToLogin()
        {
            NavigateToLoginHandler?.Invoke(null, null);
        }

        #endregion

        #region Subpages

        #region Profile
        public static event EventHandler<ProfileNavigationArgs> NavigateToProfileHandler;
        public static void NavigateToProfile(SharedModels.User user)
        {
            NavigateToProfileHandler?.Invoke(typeof(App), new ProfileNavigationArgs() { User = user });
        }
        #endregion

        #region BugReport
        public class BugReportNavigationArgs : EventArgs
        {
            public Exception Exception { get; set; }
        }
        public static event EventHandler<BugReportNavigationArgs> NavigateToBugReportHandler;
        public static void NavigateToBugReport(Exception exception)
        {
            NavigateToBugReportHandler?.Invoke(typeof(App), new BugReportNavigationArgs() { Exception = exception });
        }
        #endregion

        #region CreatBan
        public class CreateBanNavigationArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<CreateBanNavigationArgs> NavigateToCreateBanHandler;
        public static void NavigateToCreateBan(string userId)
        {
            NavigateToCreateBanHandler?.Invoke(typeof(App), new CreateBanNavigationArgs() { UserId = userId });
        }
        #endregion

        #region AddServer
        public static event EventHandler NavigateToAddServerHandler;
        public static void NavigateToAddServer()
        {
            NavigateToAddServerHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #region CreateServer
        public static event EventHandler NavigateToCreateServerHandler;
        public static void NavigateToCreateServer()
        {
            NavigateToCreateServerHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #region JoinServer
        public static event EventHandler NavigateToJoinServerHandler;
        public static void NavigateToJoinServer()
        {
            NavigateToJoinServerHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #region NicknameEdit
        public class NicknameEditNavigationArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<NicknameEditNavigationArgs> NavigateToNicknameEditHandler;
        public static void NavigateToNicknameEdit(string userId)
        {
            NavigateToNicknameEditHandler?.Invoke(typeof(App), new NicknameEditNavigationArgs() { UserId = userId });
        }
        #endregion

        #region LeaveServer
        public class LeaverServerNavigationArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<LeaverServerNavigationArgs> NavigateToLeaveServerHandler;
        public static void NavigateToLeaveServer(string guildId)
        {
            NavigateToLeaveServerHandler?.Invoke(typeof(App), new LeaverServerNavigationArgs() { GuildId = guildId });
        }
        #endregion

        #region DeleteServer
        public class DeleteServerNavigationArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<DeleteServerNavigationArgs> NavigateToDeleteServerHandler;
        public static void NavigateToDeleteServer(string guildId)
        {
            NavigateToDeleteServerHandler?.Invoke(typeof(App), new DeleteServerNavigationArgs() { GuildId = guildId });
        }
        #endregion

        #region CreateChannel
        public static event EventHandler NavigateToCreateChannelHandler;
        public static void NavigateToCreateChannel()
        {
            NavigateToCreateChannelHandler?.Invoke(typeof(App), new EventArgs());
        }
        #endregion

        #region ChannelEdit
        public class ChannelEditNavigationArgs : EventArgs
        {
            public string ChannelId { get; set; }
        }
        public static event EventHandler<ChannelEditNavigationArgs> NavigateToChannelEditHandler;
        public static void NavigateToChannelEdit(string channelId)
        {
            NavigateToChannelEditHandler?.Invoke(typeof(App), new ChannelEditNavigationArgs() { ChannelId = channelId });
        }
        #endregion

        #region DeleteChannel
        public class DeleteChannelNavigationArgs : EventArgs
        {
            public string ChannelId { get; set; }
        }

        internal static string GetString(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public static event EventHandler<DeleteChannelNavigationArgs> NavigateToDeleteChannelHandler;
        public static void NavigateToDeleteChannel(string channelId)
        {
            NavigateToDeleteChannelHandler?.Invoke(typeof(App), new DeleteChannelNavigationArgs() { ChannelId = channelId });
        }
        #endregion

        #region GuildEdit
        public class GuildEditNavigationArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<GuildEditNavigationArgs> NavigateToGuildEditHandler;
        public static void NavigateToGuildEdit(string guildId)
        {
            NavigateToGuildEditHandler?.Invoke(typeof(App), new GuildEditNavigationArgs() { GuildId = guildId });
        }
        #endregion

        #region ChannelTopic
        public class ChannelTopicNavigationArgs : EventArgs
        {
            public SharedModels.GuildChannel Channel { get; set; }
        }
        public static event EventHandler<ChannelTopicNavigationArgs> NavigateToChannelTopicHandler;
        public static void NavigateToChannelTopic(SharedModels.GuildChannel channel)
        {
            NavigateToChannelTopicHandler?.Invoke(typeof(App), new ChannelTopicNavigationArgs() { Channel = channel });
        }
        #endregion

        #region MessageEditor
        public class MessageEditorNavigationArgs : EventArgs
        {
            public string Content { get; set; }
        }
        public static event EventHandler<MessageEditorNavigationArgs> NavigateToMessageEditorHandler;
        public static void NavigateToMessageEditor(string content)
        {
            NavigateToMessageEditorHandler?.Invoke(typeof(App), new MessageEditorNavigationArgs() { Content = content });
        }
        #endregion

        #region IAPs
        public static event EventHandler NavigateToIAPSHandler;
        public static void NavigateToIAP()
        {
            NavigateToIAPSHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #region Settings
        public static event EventHandler NavigateToSettingsHandler;
        public static void NavigateToSettings()
        {
            NavigateToSettingsHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #region About
        public static event EventHandler<bool> NavigateToAboutHandler;
        public static void NavigateToAbout(bool changelist = false)
        {
            NavigateToAboutHandler?.Invoke(typeof(App), changelist);
        }
        #endregion

        #region Closed
        public static event EventHandler SubpageCloseHandler;
        public static void SubpageClose()
        {
            SubpageCloseHandler?.Invoke(typeof(App), EventArgs.Empty);
        }

        public static event EventHandler SubpageClosedHandler;
        public static void SubpageClosed()
        {
            SubpageClosedHandler?.Invoke(typeof(App), EventArgs.Empty);
        }
        #endregion

        #endregion

        #region UIUpdates
        public static event EventHandler UpdateUnreadIndicatorsHandler;
        public static void UpdateUnreadIndicators()
        {
            UpdateUnreadIndicatorsHandler?.Invoke(null, null);
        }

        public class TypingArgs
        {
            public string UserId;
            public bool Typing;
            public string ChnId;
        }
        public static event EventHandler<TypingArgs> TypingHandler;
        public static void UpdateTyping(string userId, bool typing, string chnId)
        {
            TypingHandler?.Invoke(typeof(App), new TypingArgs() { UserId = userId, Typing = typing, ChnId = chnId });
        }

        #region Messages
        public class MessageCreatedArgs
        {
            public SharedModels.Message Message;
        }
        public static event EventHandler<MessageCreatedArgs> MessageCreatedHandler;
        public static void MessageCreated(SharedModels.Message message)
        {
            MessageCreatedHandler?.Invoke(typeof(App), new MessageCreatedArgs() { Message = message});
        }

        public class MessageDeletedArgs
        {
            public string MessageId;
        }
        public static event EventHandler<MessageDeletedArgs> MessageDeletedHandler;
        public static void MessageDeleted(string messageId)
        {
            MessageDeletedHandler?.Invoke(typeof(App), new MessageDeletedArgs() { MessageId = messageId });
        }

        public class MessageEditedArgs
        {
            public SharedModels.Message Message;
        }
        public static event EventHandler<MessageEditedArgs> MessageEditedHandler;
        public static void MessageEdited(SharedModels.Message message)
        {
            MessageEditedHandler?.Invoke(typeof(App), new MessageEditedArgs() { Message = message });
        }
        #endregion

        #region Channels
        public class GuildChannelCreatedArgs
        {
            public SharedModels.GuildChannel Channel;
        }
        public static event EventHandler<GuildChannelCreatedArgs> GuildChannelCreatedHandler;
        public static void GuildChannelCreated(SharedModels.GuildChannel channel)
        {
            GuildChannelCreatedHandler?.Invoke(typeof(App), new GuildChannelCreatedArgs() { Channel = channel });
        }

        public class GuildChannelDeletedArgs
        {
            public string GuildId;
            public string ChannelId;
        }
        public static event EventHandler<GuildChannelDeletedArgs> GuildChannelDeletedHandler;
        public static void GuildChannelDeleted(string guildId, string channelId)
        {
            GuildChannelDeletedHandler?.Invoke(typeof(App), new GuildChannelDeletedArgs() { GuildId = guildId, ChannelId = channelId });
        }
        #endregion

        #region Guilds
        public class GuildCreatedArgs
        {
            public SharedModels.Guild Guild;
        }
        public static event EventHandler<GuildCreatedArgs> GuildCreatedHandler;
        public static void GuildCreated(SharedModels.Guild guild)
        {
            GuildCreatedHandler?.Invoke(typeof(App), new GuildCreatedArgs() { Guild = guild });
        }

        public class GuildDeletedArgs
        {
            public string GuildId;
        }
        public static event EventHandler<GuildDeletedArgs> GuildDeletedHandler;
        public static void GuildDeleted(string guildId)
        {
            GuildDeletedHandler?.Invoke(typeof(App), new GuildDeletedArgs() { GuildId = guildId });
        }
        #endregion

        #region Members
        public class PresenceUpdatedArgs
        {
            public string UserId;
            public SharedModels.Presence Presence;
        }
        public static event EventHandler<PresenceUpdatedArgs> PresenceUpdatedHandler;
        public static void PresenceUpdated(string userId, SharedModels.Presence presence)
        {
            PresenceUpdatedHandler?.Invoke(typeof(App), new PresenceUpdatedArgs() { UserId = userId, Presence = presence });
        }

        public static event EventHandler MembersUpdatedHandler;
        public static void MembersUpdated()
        {
            MembersUpdatedHandler?.Invoke(typeof(App), null);
        }
        #endregion

        #endregion

        #region API

        #region Messages
        public class CreateMessageArgs
        {
            public string ChannelId;
            public API.Channel.Models.MessageUpsert Message;
        }
        public static event EventHandler<CreateMessageArgs> CreateMessageHandler;
        public static void CreateMessage(string channelId, string message)
        {
            CreateMessageHandler?.Invoke(typeof(App), new CreateMessageArgs() { ChannelId = channelId, Message = new API.Channel.Models.MessageUpsert() { Content = message } });
        }


        #endregion

        public class StartTypingArgs
        {
            public string ChannelId;
        }
        public static event EventHandler<StartTypingArgs> StartTypingHandler;
        public static void StartTyping(string channelId)
        {
            StartTypingHandler?.Invoke(typeof(App), new StartTypingArgs() { ChannelId = channelId});
        }

        public class UpdatePresenceArgs
        {
            public string Status;
        }
        public static event EventHandler<UpdatePresenceArgs> UpdatePresenceHandler;
        public static void UpdatePresence(string status)
        {
            UpdatePresenceHandler?.Invoke(typeof(App), new UpdatePresenceArgs() { Status = status });
        }

        public class VoiceConnectArgs
        {
            public string ChannelId;
            public string GuildId;
        }
        public static event EventHandler<VoiceConnectArgs> VoiceConnectHandler;
        public static void ConnectToVoice(string channelId, string guildId)
        {
            VoiceConnectHandler?.Invoke(typeof(App), new VoiceConnectArgs() { ChannelId = channelId, GuildId = guildId });
        }
        #region Relations
        public class AddFriendArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<AddFriendArgs> AddFriendHandler;
        public static void AddFriend(string userId)
        {
            AddFriendHandler?.Invoke(typeof(App), new AddFriendArgs() { UserId = userId });
        }

        public class RemoveFriendArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<RemoveFriendArgs> RemoveFriendHandler;
        public static void RemoveFriend(string userId)
        {
            RemoveFriendHandler?.Invoke(typeof(App), new RemoveFriendArgs() { UserId = userId });
        }

        public class BlockUserArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<BlockUserArgs> BlockUserHandler;
        public static void BlockUser(string userId)
        {
            BlockUserHandler?.Invoke(typeof(App), new BlockUserArgs() { UserId = userId });
        }
        #endregion

        #region RPC
        public class MuteChannelArgs : EventArgs
        {
            public string ChannelId { get; set; }
        }
        public static event EventHandler<MuteChannelArgs> MuteChannelHandler;
        public static void MuteChannel(string channelId)
        {
            MuteChannelHandler?.Invoke(typeof(App), new MuteChannelArgs() { ChannelId = channelId });
        }

        public class MuteGuildArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<MuteGuildArgs> MuteGuildHandler;
        public static void MuteGuild(string guildId)
        {
            MuteGuildHandler?.Invoke(typeof(App), new MuteGuildArgs() { GuildId = guildId });
        }

        public class MarkMessageAsReadArgs : EventArgs
        {
            public string ChannelId { get; set; }
            public string MessageId { get; set; }
        }
        public static event EventHandler<MarkMessageAsReadArgs> MarkMessageAsReadHandler;
        public static void MarkMessageAsRead(string messageId, string channelId)
        {
            MarkMessageAsReadHandler?.Invoke(typeof(App), new MarkMessageAsReadArgs() { MessageId = messageId, ChannelId = channelId });
        }

        public class MarkChannelAsReadArgs : EventArgs
        {
            public string ChannelId { get; set; }
        }
        public static event EventHandler<MarkChannelAsReadArgs> MarkChannelAsReadHandler;
        public static void MarkChannelAsRead(string channelId)
        {
            MarkChannelAsReadHandler?.Invoke(typeof(App), new MarkChannelAsReadArgs() { ChannelId = channelId });
        }

        public class MarkGuildAsReadArgs : EventArgs
        {
            public string GuildId { get; set; }
        }
        public static event EventHandler<MarkGuildAsReadArgs> MarkGuildAsReadHandler;
        public static void MarkGuildAsRead(string guildId)
        {
            MarkGuildAsReadHandler?.Invoke(typeof(App), new MarkGuildAsReadArgs() { GuildId = guildId });
        }

        public static event EventHandler AckLastMessage;
        #endregion

        #endregion

        #region AutoSelects
        public class GuildChannelSelectArgs : EventArgs
        {
            public string GuildId { get; set; }
            public string ChannelId { get; set; }
        }
        public static event EventHandler<GuildChannelSelectArgs> SelectGuildChannelHandler;
        public static void SelectGuildChannel(string guildId, string channelId)
        {
            if (!App.FullyLoaded)
            {
                PostLoadTask = "SelectGuildChannelTask";
                PostLoadTaskArgs = new GuildChannelSelectArgs() { GuildId = guildId, ChannelId = channelId };
            }

            SelectGuildChannelHandler?.Invoke(typeof(App), new GuildChannelSelectArgs() { GuildId = guildId, ChannelId = channelId });
        }
        public static Task SelectGuildChannelTask(string guildId, string channelId)
        {
            SelectGuildChannelHandler?.Invoke(typeof(App), new GuildChannelSelectArgs() { GuildId = guildId, ChannelId = channelId });
            return null;
        }
        #endregion
        
        #region Other

        #region Attachment
        public enum AttachementType { Image, Video, Webpage }
        public static event EventHandler<SharedModels.Attachment> OpenAttachementHandler;
        public static void OpenAttachement(SharedModels.Attachment args)
        {
            OpenAttachementHandler?.Invoke(typeof(App), args);
        }
        #endregion

        public class MentionArgs : EventArgs
        {
            public string Username { get; set; }
            public string Discriminator { get; set; }
        }
        public static event EventHandler<MentionArgs> MentionHandler;
        public static void MentionUser(string username, string disc)
        {
            MentionHandler?.Invoke(typeof(App), new MentionArgs() { Username = username, Discriminator = disc });
        }

        public static event EventHandler PlayHeartBeatHandler;
        public static void PlayHeartBeat()
        {
            PlayHeartBeatHandler?.Invoke(null, null);
        }

        public class UserStatusChangedArgs : EventArgs
        {
            public string Status { get; set; }
        }
        public static event EventHandler<UserStatusChangedArgs> UserStatusChangedHandler;
        public static void UserStatusChanged(string status)
        {
            UserStatusChangedHandler?.Invoke(typeof(App), new UserStatusChangedArgs() { Status = status });
        }

        public static event PointerEventHandler UniversalPointerDownHandler;
        public static void UniversalPointerDown(PointerRoutedEventArgs args)
        {
            UniversalPointerDownHandler?.Invoke(null, args);
        }
        #endregion

        #endregion

        #region Statics
        public static SplashScreen Splash;

        internal static bool CurrentGuildIsDM;
        internal static string CurrentGuildId;
        internal static string CurrentChannelId;
        internal static int FriendNotifications;
        internal static bool HasFocus = true;
        internal static bool ShowAds = true;
        internal static bool CinematicMode = false;
        internal static bool GatewayCreated = false;
        internal static bool FullyLoaded = false;
        internal const string ClientId = "357923233636286475";
        internal const string ClientSecret = "kwZr7BzE-8uRKgXcNcaAsy4vau20xLNX"; //It is inoptimal to store this here, maybe at some point I can justify using azure to send the secret


        public static ResourceLoader ResAbout = ResourceLoader.GetForCurrentView("About");
        public static ResourceLoader ResControls = ResourceLoader.GetForCurrentView("Controls");
        public static ResourceLoader ResDialogs = ResourceLoader.GetForCurrentView("Dialogs");
        public static ResourceLoader ResFlyouts = ResourceLoader.GetForCurrentView("Flyouts");
        public static ResourceLoader ResMain = ResourceLoader.GetForCurrentView("Main");
        public static ResourceLoader ResSettings = ResourceLoader.GetForCurrentView("Settings");
        public static string GetString(string str)
        {
            str = str.Remove(0, 1);
            int index = str.IndexOf('/');
            string map = str.Remove(index);
            str = str.Remove(0, index + 1);
            switch (map)
            {
                case "About": return ResAbout.GetString(str);
                case "Controls": return ResControls.GetString(str);
                case "Dialogs": return ResDialogs.GetString(str);
                case "Flyouts": return ResFlyouts.GetString(str);
                case "Main": return ResMain.GetString(str);
                case "Settings": return ResSettings.GetString(str);
            }
            return "String";
            //   return loader.GetString(str);
        }

        public static bool IsOnline()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        public static bool LoggedIn()
        {
            try //if Contains("LogIn)
            {
                var nullCheck = Storage.PasswordVault.FindAllByResource("Token");
                if (nullCheck.FirstOrDefault() != null && nullCheck.FirstOrDefault().Password != null)
                    return true;
                else
                    return false;
            }
            catch // else
            {
                return false;
            }
        }

        public static bool IsMobile
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }
        #endregion
        #endregion

        private void LoadSettings()
        {

            if (Storage.SavedSettings.Containers.ContainsKey("settings"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    StringReader userNameReader = new StringReader((string)Storage.SavedSettings.Values["settings"]);
                    Storage.Settings = (Settings)serializer.Deserialize(userNameReader);
                }
                catch
                {
                    Storage.Settings.lastVerison = 0;
                    Storage.Settings.AutoHideChannels = true;
                    Storage.Settings.AutoHidePeople = false;
                    Storage.Settings.Toasts = false;
                    Storage.Settings.HighlightEveryone = true;
                    Storage.Settings.RespUiM = 569;
                    Storage.Settings.RespUiL = 768;
                    Storage.Settings.RespUiXl = 1024;
                    //Storage.Settings.AppBarAtBottom = false;
                    Storage.Settings.DiscordLightTheme = false;
                    Storage.Settings.CompactMode = false;
                    Storage.Settings.DevMode = false;
                    Storage.Settings.ExpensiveRender = false;
                    Storage.Settings.Theme = Theme.Dark;
                    Storage.Settings.AccentBrush = false;
                    Storage.Settings.mutedChnEffectServer = false;
                }
            }
            else
            {
                Storage.Settings.lastVerison = 0;
                Storage.Settings.AutoHideChannels = true;
                Storage.Settings.AutoHidePeople = false;
                Storage.Settings.Toasts = false;
                Storage.Settings.HighlightEveryone = true;
                Storage.Settings.CompactMode = false;
                Storage.Settings.Toasts = false;
                Storage.Settings.RespUiM = 569;
                Storage.Settings.RespUiL = 768;
                Storage.Settings.RespUiXl = 1024;
                //Storage.Settings.AppBarAtBottom = false;
                Storage.Settings.DiscordLightTheme = false;
                Storage.Settings.ExpensiveRender = false;
                Storage.Settings.DevMode = false;
                Storage.Settings.Theme = Theme.Dark;
                Storage.Settings.AccentBrush = false;
                Storage.Settings.mutedChnEffectServer = false;

                //MessageDialog msg = new MessageDialog("You had no settings saved. Defaults set.");
            }

            switch (Storage.Settings.Theme)
            {
                case Theme.Dark:
                    this.RequestedTheme = ApplicationTheme.Dark;
                    break;
                case Theme.Light:
                    this.RequestedTheme = ApplicationTheme.Light;
                    break;
                case Theme.Discord:
                    this.RequestedTheme = Storage.Settings.DiscordLightTheme ? ApplicationTheme.Light : ApplicationTheme.Dark;
                    break;
                default:
                    //Windows, already uses system default
                    break;
            }
        }

        private void WindowFocusChanged(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            //TODO: https://www.eternalcoding.com/?p=1952

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                HasFocus = false;
            else
            {
                AckLastMessage?.Invoke(null, null);
                HasFocus = true;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            LaunchProcedure(e.SplashScreen, e.PreviousExecutionState, e.PrelaunchActivated, e.Arguments);
            
        }
        private void LaunchProcedure(SplashScreen splash, ApplicationExecutionState PreviousExecutionState, bool PrelaunchActivated, string Arguments)
        {
            var licenseInformation = CurrentApp.LicenseInformation;
            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
            {
                App.ShowAds = false;
            }

            Frame rootFrame = Window.Current.Content as Frame;

            SetupTitleBar();
            InitializeResources();

            Splash = splash;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (IsOnline())
                    {
                        rootFrame.Navigate(typeof(MainPage), Arguments);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(Offline), Arguments);
                    }
                }

                //Cortana crap
                //var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommands.xml"));
                //await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
                // Ensure the current window is active
                Window.Current.Activate();
                Window.Current.CoreWindow.Activated += WindowFocusChanged;
            }
        }
        /// <summary>
        /// Invoked when the application is not launche normally by the end user
        /// </summary>
        /// <param name="args">Detais about the activation event</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                //If the app isn't already open, do so
                if (args.PreviousExecutionState != ApplicationExecutionState.Running)
                    LaunchProcedure(args.SplashScreen, args.PreviousExecutionState, false, "");

                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                string[] segments = eventArgs.Uri.ToString().Replace("discorduwp://", "").Split('/');
                var count = segments.Count();
                if (count > 0)
                {
                    if(segments[0] == "guild")
                    {
                        if (count == 3)
                            App.SelectGuildChannel(segments[1], segments[2]);
                        else if(count == 2)
                            App.SelectGuildChannel(segments[1], null);
                    }
                };
            }
        }
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public void InitializeResources()
        {
            //if the acrylic brushes exist AND the app is not running in cinematic mode, replace the app resources with them:
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
            {
                var UserBackground = ((SolidColorBrush)App.Current.Resources["AcrylicUserBackground"]).Color;
                if (!App.CinematicMode)
                {
                    var ChannelColor = ((SolidColorBrush)App.Current.Resources["AcrylicChannelPaneBackground"]).Color;
                    App.Current.Resources["AcrylicChannelPaneBackground"] = new AcrylicBrush()
                    {
                        TintOpacity = 0.6,
                        //Opacity = 0.6,
                        TintColor = ChannelColor,
                        FallbackColor = ChannelColor,
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop
                    };
                    var GuildColor = ((SolidColorBrush)App.Current.Resources["AcrylicGuildPaneBackground"]).Color;
                    App.Current.Resources["AcrylicGuildPaneBackground"] = new AcrylicBrush()
                    {
                        TintOpacity = 0.4,
                        //Opacity = 0.4,
                        TintColor = GuildColor,
                        FallbackColor = GuildColor,
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop
                    };
                    var CommandBarColor = ((SolidColorBrush)App.Current.Resources["AcrylicCommandBarBackground"]).Color;
                    App.Current.Resources["AcrylicCommandBarBackground"] = new AcrylicBrush()
                    {
                        TintOpacity = 0.7,
                        //Opacity = 0.7,
                        TintColor = CommandBarColor,
                        FallbackColor = CommandBarColor,
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop
                    };
                    var MessageColor = ((SolidColorBrush)App.Current.Resources["AcrylicMessageBackground"]).Color;
                    App.Current.Resources["AcrylicMessageBackground"] = new AcrylicBrush()
                    {
                        TintOpacity = 0.9,
                        //Opacity = 1,
                        TintColor = UserBackground,
                        FallbackColor = UserBackground,
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop
                    };
                }
                
                App.Current.Resources["AcrylicUserBackground"] = new AcrylicBrush()
                {
                    TintOpacity = 0.3,
                    //Opacity = 1,
                    TintColor = UserBackground,
                    FallbackColor = UserBackground,
                    BackgroundSource = AcrylicBackgroundSource.Backdrop
                };

                var FlyoutColor = ((SolidColorBrush)App.Current.Resources["AcrylicFlyoutBackground"]).Color;
                App.Current.Resources["AcrylicFlyoutBackground"] = new AcrylicBrush()
                {
                    TintOpacity = 0.7,
                    //Opacity = 0.9,
                    TintColor = FlyoutColor,
                    FallbackColor = FlyoutColor,
                    BackgroundSource = AcrylicBackgroundSource.Backdrop
                };

                var DeepBGColor = ((SolidColorBrush)App.Current.Resources["DeepBG"]).Color;
                App.Current.Resources["DeepBG"] = new AcrylicBrush()
                {
                    TintOpacity = 0.9,
                    //Opacity = 1,
                    TintColor = DeepBGColor,
                    FallbackColor = DeepBGColor,
                    BackgroundSource = AcrylicBackgroundSource.Backdrop
                };
            }

            if (App.CinematicMode)
            {
                //Remove the artificial TV-Safe area in cinematic mode:
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
            }
            if (Storage.Settings.AccentBrush)
            {
                var accentColor = (Color)this.Resources["SystemAccentColor"];
                App.Current.Resources["BlurpleTranslucentColor"] = Color.FromArgb(25, accentColor.R, accentColor.G, accentColor.B);
                App.Current.Resources["BlurpleTranslucent"] = new SolidColorBrush((Color)App.Current.Resources["BlurpleTranslucentColor"]);
                App.Current.Resources["Blurple"] = new SolidColorBrush(accentColor); //Set to system accent color
                App.Current.Resources["BlurpleColor"] = accentColor; //Set to system accent color
            }
            else
            {
                var blurple = Color.FromArgb(255, 114, 137, 218);
                App.Current.Resources["Blurple"] = new SolidColorBrush(blurple); //Set to Blurple default
                App.Current.Resources["BlurpleColor"] = blurple;
            }

            //var onlineString = Storage.Settings.OnlineBursh;
            //var onlineColor = onlineString.ToColor();
            //var idleString = Storage.Settings.IdleBrush;
            //var idleColor = idleString.ToColor();
            //var dndString = Storage.Settings.DndBrush;
            //var dndColor = dndString.ToColor();
            //var offlineString = Storage.Settings.OfflineBrush;
            //var offlineColor = offlineString.ToColor();
            //App.Current.Resources["BlurpleColor"] = accentColor;
            //App.Current.Resources["online"] = new SolidColorBrush(onlineColor);
            //App.Current.Resources["idle"] = new SolidColorBrush(idleColor);
            //App.Current.Resources["DndColor"] = dndColor;
            //App.Current.Resources["dnd"] = new SolidColorBrush(dndColor);
            //App.Current.Resources["offline"] = new SolidColorBrush(offlineColor);
        }

        public void SetupTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            //view.TitleBar.BackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            //view.TitleBar.ForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            //view.TitleBar.ButtonBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            //view.TitleBar.ButtonForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            //view.TitleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG"]).Color;
            //view.TitleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            //view.TitleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["LightBG"]).Color;
            //view.TitleBar.ButtonPressedForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            //view.TitleBar.ButtonInactiveBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            //view.TitleBar.ButtonInactiveForegroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG_hover"]).Color;
            //view.TitleBar.InactiveBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            //view.TitleBar.InactiveForegroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG_hover"]).Color;
        }

    }
}
