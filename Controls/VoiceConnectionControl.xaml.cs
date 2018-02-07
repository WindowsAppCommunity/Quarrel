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

namespace Discord_UWP.Controls
{
    public sealed partial class VoiceConnectionControl : UserControl
    {
        public VoiceConnectionControl()
        {
            this.InitializeComponent();
            App.VoiceConnectHandler += App_VoiceConnectHandler;
        }

        private void App_VoiceConnectHandler(object sender, App.VoiceConnectArgs e)
        {
            ChannelName.Text = e.ChannelName;
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            App.ConnectToVoice(null, null, "");
        }

        private void MiniView_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Enter miniview
        }

        private void Deafen_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Toggle local deafen
            AudioManager.ChangeDeafStatus(Deafen.IsChecked.Value);
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Toggle local mute
            
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AudioManager.ChangeVolume(e.NewValue/100);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Expanded.Visibility = Expanded.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
