using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class ActivitesControl : UserControl
    {
        /// <summary>
        /// The API ActivityData to display
        /// </summary>
        public ActivityData Activity
        {
            get { return (ActivityData)GetValue(ActivityProperty); }
            set { SetValue(ActivityProperty, value); }
        }
        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register(
            nameof(Activity),
            typeof(ActivityData),
            typeof(ActivitesControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ActivitesControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == ActivityProperty)
            {
                // If the Activity isn't null and it's a supported game
                if (Activity != null && !string.IsNullOrEmpty(Activity.ApplicationId))
                {
                    if (LocalState.SupportedGames.ContainsKey(Activity.ApplicationId))
                    {
                        // Determine the game
                        var game = LocalState.SupportedGames[Activity.ApplicationId];
                        GameIcon.Source = new BitmapImage(new Uri("https://cdn.discordapp.com/game-assets/"+game.Id+"/"+game.Icon+".png?size=256"));
                        GameTitle.Text = game.Name;
                        GameSectionBg.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/game-assets/" + game.Id + "/" + game.Splash));

                        // If a friend is playing it, show that
                        if (LocalState.Friends.ContainsKey(Activity.UserId))
                        {
                            var friend = LocalState.Friends[Activity.UserId];
                            var membercontrol = new MemberControl();
                            membercontrol.Username = friend.user.Username;
                            if (LocalState.PresenceDict.ContainsKey(Activity.UserId))
                                membercontrol.Status = LocalState.PresenceDict[Activity.UserId];
                            membercontrol.RawMember = new GuildMember(){Deaf=false,User = friend.user };
                            Users.Children.Add(membercontrol);
                        }
                        //TODO Support multiple users

                        if (LocalState.GameNews.ContainsKey(Activity.ApplicationId))
                        {
                            //TODO ADD LATEST NEWS
                        }
                    }
                }
            }
        }

        public ActivitesControl()
        {
            this.InitializeComponent();
        }
    }
}
