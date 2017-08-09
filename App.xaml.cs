using Discord_UWP.API;
using Discord_UWP.API.Channel;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.API.Gateway;
using Discord_UWP.API.Guild;
using Discord_UWP.API.Login;
using Discord_UWP.API.Login.Models;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using Discord_UWP.CacheModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Microsoft.Toolkit.Uwp;
using Windows.ApplicationModel.Resources;

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
            this.InitializeComponent();
            try
            {
                LoadSettings();
                if (Storage.Settings.Theme == Theme.Dark)
                    this.RequestedTheme = ApplicationTheme.Dark;
                else if (Storage.Settings.Theme == Theme.Light)
                    this.RequestedTheme = ApplicationTheme.Light;
                else if (Storage.Settings.Theme == Theme.Discord)
                    if (Storage.Settings.DiscordLightTheme)
                        this.RequestedTheme = ApplicationTheme.Light;
                    else
                        this.RequestedTheme = ApplicationTheme.Dark;
                this.Suspending += OnSuspending;
            }
            catch { }
            
        }

        #region Events

        #region General
        public class ProfileNavigationArgs : EventArgs
        {
            public SharedModels.User User { get; set; }
        }
        #endregion

        #region Flyouts
        public enum Type { Guild, GuildMember, GroupMember, TextChn, DMChn, GroupChn }
        public class MenuArgs : EventArgs
        {
            public Type Type { get; set; }
            public string Id { get; set; }
            public string ParentId { get; set; }
            public Point Point { get; set; }
        }
        public static event EventHandler<MenuArgs> MenuHandler;
        public static void ShowMenuFlyout(object sender, Type type, string Id, string parentId, Point point)
        {
            MenuHandler?.Invoke(sender, new MenuArgs() { Type = type, Id = Id, ParentId = parentId, Point = point });
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
        }
        public static event EventHandler<GuildChannelNavigationArgs> NavigateToGuildChannelHandler;
        public static void NavigateToGuildChannel(string guildId, string channelId, string message = null, bool send = false)
        {
            NavigateToGuildChannelHandler?.Invoke(typeof(App), new GuildChannelNavigationArgs() { GuildId = guildId, ChannelId = channelId, Message = message, Send = send });
        }
        #endregion

        #region DMChannel
        public class DMChannelNavigationArgs : EventArgs
        {
            public string UserId { get; set; }
            public string Message { get; set; }
            public bool Send { get; set; }
        }
        public static event EventHandler<DMChannelNavigationArgs> NavigateToDMChannelHandler;
        public static void NavigateToDMChannel(string userId, string message = null, bool send = false)
        {
            NavigateToDMChannelHandler?.Invoke(typeof(App), new DMChannelNavigationArgs() { UserId = userId, Message = message, Send = send });
        }
        #endregion

        #endregion

        #region Subpages

        #region Profile
        public static event EventHandler<ProfileNavigationArgs> NavigateToProfileHandler;
        public static void NavigateToProfile(SharedModels.User user)
        {
            NavigateToProfileHandler?.Invoke(typeof(App), new ProfileNavigationArgs() { User = user });
        }
        #endregion

        #region CreatBan
        public class CreateBanNavigationArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<CreateBanNavigationArgs> NavigateToCreateBanHandler;
        public static void NavigateToCreateBan( string userId)
        {
            NavigateToCreateBanHandler?.Invoke(typeof(App), new CreateBanNavigationArgs() {UserId = userId});
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

        #region Closed
        public static event EventHandler SubpageClosedHandler;
        public static void SubpageClosed()
        {
            SubpageClosedHandler?.Invoke(typeof(App), EventArgs.Empty);
        }
        #endregion

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
        }
        public static event EventHandler<MentionArgs> MentionHandler;
        public static void MentionUser(string username)
        {
            MentionHandler?.Invoke(typeof(App), new MentionArgs() { Username = username });
        }
        #endregion

        #endregion

        #region Static Objects
        internal static string CurrentGuildId;
        internal static string CurrentChannelId;
        internal static Guild CurrentGuild;
        internal static bool CurrentGuildIsDM = false;
        internal static string CurrentUserId = "";
        internal static bool IsInFocus = true;
        internal static bool ShowAds = true;
        internal static Dictionary<string, string> Notes = new Dictionary<string, string>();
        internal static Dictionary<string, Member> GuildMembers = new Dictionary<string, Member>();
        #endregion

        #region Static Voids
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
            str = str.Remove(0, index+1);
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

        #endregion
        //internal static string ChannelId;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>

        /*protected override async void OnActivated(IActivatedEventArgs e)
        {
            await InitializeApp();

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e is ToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;
                // If empty args, no specific action (just launch the app)
                if (toastActivationArgs.Argument.Length == 0)
                {
                    if (rootFrame.Content == null)
                        rootFrame.Navigate(typeof(MainPage));
                }
                // Otherwise an action is provided
                else
                {
                    // Parse the query string
                    QueryString args = QueryString.Parse(toastActivationArgs.Argument);
                    // See what action is being requested 
                    switch (args["action"])
                    {
                        // Open the image
                        case "reply":
                            await HandleReply(args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    // If we're loading the app for the first time, place the main page on the back stack
                    // so that user can go back after they've been navigated to the specific page
                    if (rootFrame.BackStack.Count == 0)
                        rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));
                }
            }
        }*/

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
                    Storage.Settings.AutoHideChannels = true;
                    Storage.Settings.AutoHidePeople = false;
                    Storage.Settings.Toasts = false;
                    Storage.Settings.HighlightEveryone = true;
                    Storage.Settings.RespUiM = 569;
                    Storage.Settings.RespUiL = 768;
                    Storage.Settings.RespUiXl = 1024;
                    Storage.Settings.AppBarAtBottom = false;
                    Storage.Settings.DiscordLightTheme = false;
                    Storage.Settings.CompactMode = false;
                    Storage.Settings.DevMode = false;
                    Storage.Settings.ExpensiveRender = false;
                    Storage.Settings.Theme = Theme.Dark;
                    Storage.Settings.AccentBrush = Color.FromArgb(255, 114, 137, 218).ToHex();
                    Storage.SaveAppSettings();
                }
            }
            else
            {
                Storage.Settings.AutoHideChannels = true;
                Storage.Settings.AutoHidePeople = false;
                Storage.Settings.Toasts = false;
                Storage.Settings.HighlightEveryone = true;
                Storage.Settings.CompactMode = false;
                Storage.Settings.Toasts = false;
                Storage.Settings.RespUiM = 569;
                Storage.Settings.RespUiL = 768;
                Storage.Settings.RespUiXl = 1024;
                Storage.Settings.AppBarAtBottom = false;
                Storage.Settings.DiscordLightTheme = false;
                Storage.Settings.ExpensiveRender = false;
                Storage.Settings.DevMode = false;
                Storage.Settings.Theme = Theme.Dark;
                Storage.Settings.AccentBrush = Color.FromArgb(255, 114, 137, 218).ToHex();
                Storage.Settings.OnlineBursh = Color.FromArgb(255, 67, 181, 129).ToHex();
                Storage.Settings.IdleBrush = Color.FromArgb(255, 250, 166, 26).ToHex();
                Storage.Settings.DndBrush = Color.FromArgb(255, 240, 71, 71).ToHex();
                Storage.SaveAppSettings();

                MessageDialog msg = new MessageDialog("You had no settings saved. Defaults set.");
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //Set the title bar colors:

            #region Resources
            var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            view.TitleBar.ForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            view.TitleBar.ButtonBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            view.TitleBar.ButtonForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            view.TitleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG"]).Color;
            view.TitleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            view.TitleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["LightBG"]).Color;
            view.TitleBar.ButtonPressedForegroundColor = ((SolidColorBrush)Application.Current.Resources["InvertedBG"]).Color;
            view.TitleBar.ButtonInactiveBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            view.TitleBar.ButtonInactiveForegroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG_hover"]).Color;
            view.TitleBar.InactiveBackgroundColor = ((SolidColorBrush)Application.Current.Resources["DarkBG"]).Color;
            view.TitleBar.InactiveForegroundColor = ((SolidColorBrush)Application.Current.Resources["MidBG_hover"]).Color;

            var accentString = Storage.Settings.AccentBrush;
            var accentColor = accentString.ToColor();
            var onlineString = Storage.Settings.OnlineBursh;
            var onlineColor = onlineString.ToColor();
            var idleString = Storage.Settings.IdleBrush;
            var idleColor = idleString.ToColor();
            var dndString = Storage.Settings.DndBrush;
            var dndColor = dndString.ToColor();
            var offlineString = Storage.Settings.OfflineBrush;
            var offlineColor = offlineString.ToColor();
            App.Current.Resources["Blurple"] = new SolidColorBrush(accentColor);
            App.Current.Resources["BlurpleColor"] = accentColor;
            App.Current.Resources["online"] = new SolidColorBrush(onlineColor);
            App.Current.Resources["idle"] = new SolidColorBrush(idleColor);
            App.Current.Resources["DndColor"] = dndColor;
            App.Current.Resources["dnd"] = new SolidColorBrush(dndColor);
            App.Current.Resources["offline"] = new SolidColorBrush(offlineColor);
            App.Current.Resources["BlurpleTranslucentColor"] = Color.FromArgb(25, accentColor.R, accentColor.G, accentColor.B);
            App.Current.Resources["BlurpleTranslucent"] = new SolidColorBrush((Color)App.Current.Resources["BlurpleTranslucentColor"]);
            #endregion

            //Set the minimum window size:
            view.SetPreferredMinSize(new Size(128,128));

            //RegisterBackgroundTask();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            rootFrame.Background = (SolidColorBrush)Application.Current.Resources["DeepBG"];

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    bool loggedIn;
                    if (Storage.SavedSettings.Containers.ContainsKey("user"))
                    {
                        try
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(string));
                            StringReader userNameReader = new StringReader((string)Storage.SavedSettings.Values["user"]);
                            Storage.Token = (string)serializer.Deserialize(userNameReader);
                            loggedIn = true;
                        }
                        catch
                        {
                            loggedIn = false;
                        }
                    }
                    else
                    {
                        loggedIn = false;
                    }

                    if (loggedIn)
                    {
                        rootFrame.Navigate(typeof(Main), e.Arguments);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(LockScreen), e.Arguments);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        public static bool IsConsole
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Console");
            }
        }

        public static bool IsMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
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

        private async void RegisterBackgroundTask()
        {
            //
            // A friendly task name.
            //
            String name = "BackgroundActivity";

            //
            // Must be the same entry point that is specified in the manifest.
            //
            String taskEntryPoint = "Discord_UWP.BackgroundActivity";

            //
            // A time trigger that repeats at 15-minute intervals.
            //
            IBackgroundTrigger trigger = new TimeTrigger(15, false);
            SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);

            //
            // Builds the background task.
            //
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);
            builder.AddCondition(internetCondition);

            //
            // Registers the background task, and get back a BackgroundTaskRegistration object representing the registered task.
            //
            BackgroundTaskRegistration task = builder.Register();
            BackgroundExecutionManager.RemoveAccess();
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            
        }

    }
}
