using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class UserProfile : Page
    {
        public UserProfile()
        {
            this.InitializeComponent();
        }
        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private User? user;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string)
            {
                user = await Session.GetUser(e.Parameter as string);
            }
            else if (e.Parameter is User)
            {
                user = e.Parameter as User?;
            }
            if (!user.HasValue) CloseButton_Click(null, null);

            username.Text = user.Value.Username;
            discriminator.Text = "#" + user.Value.Discriminator;
            if(user.Value.Note != null)
                NoteBox.Text = user.Value.Note;

            if (App.UpdatedNotes.Any())
            {
                var newNote = App.UpdatedNotes.FirstOrDefault(x => x.Key == user.Value.Id);
                if (newNote.Value != null)
                    NoteBox.Text = newNote.Value;
            }
            Session.Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;

            BackgroundGrid.Blur(8,200).Start();
            var image = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + user.Value.Id + "/" + user.Value.Avatar + ".png?size=64"));
            AvatarFull.ImageSource = image;
            AvatarBlurred.Source = image;
        }

        private async void Gateway_UserNoteUpdated(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
        {
            if (e.EventData.UserId == user.Value.Id)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        NoteBox.Text = e.EventData.Note;
                    });
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            Session.Gateway.UserNoteUpdated -= Gateway_UserNoteUpdated;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Session.AddNote(user.Value.Id, NoteBox.Text);
        }

        private void AvatarBlurred_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as Image).Fade(0.2f).Start();
        }
    }
}
