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


        public enum Type { Guild, GuildMember, GroupMember, TextChn, DMChn, GroupChn}

        public class MenuArgs : EventArgs
        {
            public Type Type { get; set; }
            public string Id { get; set; }
            public string ParentId { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }
        public static event EventHandler<MenuArgs> MenuHandler;
        public static void ShowUserFlyout(Type type, string Id, string parentId, double x, double y)
        {
            MenuHandler?.Invoke(typeof(App), new MenuArgs() { Type = type, Id = Id, ParentId = parentId, X = x, Y = y });
        }

        internal static string CurrentGuildId;
        internal static string CurrentChannelId;
        internal static Guild CurrentGuild;
        internal static bool CurrentGuildIsDM = false;
        internal static string CurrentUserId = "";
        internal static bool IsInFocus = true;
        internal static bool ShowAds = true;
        internal static Dictionary<string, string> Notes = new Dictionary<string, string>();
        internal static Dictionary<string, Member> GuildMembers = new Dictionary<string, Member>();
        /// <summary>
        /// Fired when a link element in the markdown was tapped.
        /// </summary>
        public static event EventHandler<MarkdownTextBlock.LinkClickedEventArgs> LinkClicked;

        public static void FireLinkClicked(MarkdownTextBlock.LinkClickedEventArgs LinkeventArgs)
        {
            LinkClicked?.Invoke(typeof(App), LinkeventArgs);
        }

        public static event EventHandler SubpageClosedHandler;
        public static void SubpageClosed()
        {
            SubpageClosedHandler?.Invoke(typeof(App), EventArgs.Empty);
        }

        public class GuildNavigationArgs : EventArgs
        {
            public string Guildid { get; set; }
        }
        public static event EventHandler<GuildNavigationArgs> NavigateToGuildHandler;
        public static void NavigateToGuild(string guildId)
        {
            NavigateToGuildHandler?.Invoke(typeof(App), new GuildNavigationArgs(){Guildid = guildId});
        }

        public class ChannelNavigationArgs : EventArgs
        {
            public string Guildid { get; set; }
            public string Channelid { get; set; }
        }
        public static event EventHandler<ChannelNavigationArgs> NavigateToChannelHandler;
        public static void NavigateToChannel(string guildId, string channelId)
        {
            NavigateToChannelHandler?.Invoke(typeof(App), new ChannelNavigationArgs() { Guildid = guildId, Channelid = channelId });
        }

        public class ProfileNavigationArgs : EventArgs
        {
            public string UserId { get; set; }
        }
        public static event EventHandler<ProfileNavigationArgs> NavigateToProfileHandler;
        public static void NavigateToProfile(string userId)
        {
            NavigateToProfileHandler?.Invoke(typeof(App), new ProfileNavigationArgs() { UserId = userId });
        }

        public enum AttachementType { Image, Video, Webpage }
        public static event EventHandler<SharedModels.Attachment> OpenAttachementHandler;
        public static void OpenAttachement(SharedModels.Attachment args)
        {
            OpenAttachementHandler?.Invoke(typeof(App), args);
        }
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
            var dndColor = onlineString.ToColor();
            var offlineString = Storage.Settings.OfflineBrush;
            var offlineColor = offlineString.ToColor();
            App.Current.Resources["Blurple"] = new SolidColorBrush(accentColor);
            App.Current.Resources["BlurpleColor"] = accentColor;
            App.Current.Resources["Online"] = new SolidColorBrush(onlineColor);
            App.Current.Resources["Idle"] = new SolidColorBrush(idleColor);
            App.Current.Resources["Dnd"] = new SolidColorBrush(dndColor);
            App.Current.Resources["Offline"] = new SolidColorBrush(offlineColor);
            App.Current.Resources["BlurpleTranslucentColor"] = Color.FromArgb(25, accentColor.R, accentColor.G, accentColor.B);
            App.Current.Resources["BlurpleTranslucent"] = new SolidColorBrush((Color)App.Current.Resources["BlurpleTranslucentColor"]);

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
                        // await Session.AutoLogin();
                        // Session.Online = true;
                        //rootFrame.Navigate(typeof(Main), e.Arguments);
                        if (IsConsole)
                        {
                            if (e.Arguments != "")
                            {
                                rootFrame.Navigate(typeof(Main), e.Arguments);
                            }
                            else
                            {
                                rootFrame.Navigate(typeof(Main));
                            }
                        }
                        else
                        {
                            if (e.Arguments != "")
                            {
                                rootFrame.Content = new Main(e.Arguments);
                            }
                            else
                            {
                                rootFrame.Content = new Main();
                            }
                        }
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
