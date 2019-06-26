using JetBrains.Annotations;
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
using Windows.UI.Xaml.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using Quarrel.SubPages.Interfaces;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class UserProfilePage : UserControl, IConstrainedSubPage
    {
        public UserProfilePage()
        {
            this.InitializeComponent();
        }

        public UserProfilePage([NotNull] BindableUser member) : this()
        {
            this.DataContext = member;
        }

        BindableUser ViewModel => DataContext as BindableUser;

        private void NoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ServicesManager.Discord.UserService.AddNote(ViewModel.Model.User.Id, new DiscordAPI.API.User.Models.Note() { Content = (sender as TextBox).Text });
        }

        public double MaxExpandedHeight { get; } = 768;

        public double MaxExpandedWidth { get; } = 768;
    }
}
