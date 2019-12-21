using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables;
using Quarrel.Navigation;
using Quarrel.Services.Rest;
using Quarrel.SubPages.Interfaces;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class AddChannelPage : UserControl, IConstrainedSubPage
    {
        private ISubFrameNavigationService subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        public AddChannelPage()
        {
            this.InitializeComponent();
            if (subFrameNavigationService.Parameter != null)
            {
                DataContext = subFrameNavigationService.Parameter;
            }
        }

        public BindableGuild ViewModel => DataContext as BindableGuild;

        public double MaxExpandedHeight { get; } = 300;

        public double MaxExpandedWidth { get; } = 400;

        private void Save(object sender, RoutedEventArgs e)
        {
            CreateGuildChannel createChannel = new CreateGuildChannel();
            
            if (TextRadioButton.IsChecked == true)
            {
                createChannel.Type = 0;
            }
            else if (VoiceRadioButton.IsChecked == true)
            {
                createChannel = new CreateVoiceChannel();
                createChannel.Type = 2;
            }

            createChannel.Name = ChannelNameBox.Text;

            SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.CreateGuildChannel(ViewModel.Model.Id, createChannel);
            subFrameNavigationService.GoBack();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            subFrameNavigationService.GoBack();
        }
    }
}
