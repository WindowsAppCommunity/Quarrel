using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using DiscordAPI.Models;
using Quarrel.Services;
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
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Gateway;
using Quarrel.Services.Rest;
using Quarrel.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class VoiceConnection : UserControl
    {
        public MainViewModel ViewModel => (Application.Current.Resources["ViewModelLocator"] as ViewModelLocator).Main;

        public VoiceConnection()
        {
            this.InitializeComponent();
        }
    }
}
