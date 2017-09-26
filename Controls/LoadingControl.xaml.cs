using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
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
    public sealed partial class LoadingControl : UserControl
    {
        public string Message
        { get => MessageBlock.Text; set => MessageBlock.Text = value; }

        public string Status
        { get => StatusBlock.Text; set => StatusBlock.Text = value; }

        public LoadingControl()
        {
            this.InitializeComponent();
            var message = EntryMessages.GetMessage();
            MessageBlock.Text = message.Key.ToUpper();
            if(message.Value != "")
                CreditBlock.Text = App.GetString("/Main/SubmittedBy") + " " + message.Value;
            Animation.Begin();
        }
        public void AdjustSize()
        {
            var location = App.Splash.ImageLocation;
            viewbox.Width = location.Width;
            this.Focus(FocusState.Pointer);
            viewbox.Margin = new Thickness(0, location.Top - 32, 0, 0);
        }
        public void Show(bool animate)
        {
            this.Visibility = Visibility.Visible;
            if(animate) LoadIn.Begin();
        }
        public void Hide(bool animate)
        {
            if (animate)
            {
                LoadOut.Begin();
            }
            else this.Visibility = Visibility.Collapsed;
        }
        private void LoadIn_Completed(object sender, object e)
        {
            Animation.Begin();
        }

        private void LoadOut_Completed(object sender, object e)
        {
            this.Visibility = Visibility.Collapsed;
            Animation.Stop();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustSize();
        }
    }
}
