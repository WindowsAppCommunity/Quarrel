using DiscordAPI.API.Guild;
using GalaSoft.MvvmLight.Ioc;
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
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Models.Bindables;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages
{
    public sealed partial class MessageFlyout : UserControl
    {
        public MessageFlyout()
        {
            this.InitializeComponent();
        }

        public BindableMessage Message => DataContext as BindableMessage;
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

    }
}
