using GalaSoft.MvvmLight.Messaging;
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
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Views
{
    public sealed partial class GuildView : UserControl
    {
        public GuildView()
        {
            this.InitializeComponent();
            DataContext = new GuildViewModel();
        }

        public GuildViewModel ViewModel => DataContext as GuildViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new GuildNavigateMessage((e.ClickedItem as BindableGuild)));
        }
    }
}
