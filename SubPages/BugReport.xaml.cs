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
using Microsoft.Toolkit.Uwp;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BugReport : Page
    {
        public BugReport()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Exception except = (Exception)e.Parameter;
            BugDetails.Text += "Data: " + except.Data + Environment.NewLine;
            BugDetails.Text += "HelpLink: " + except.HelpLink + Environment.NewLine;
            BugDetails.Text += "HResult (as int): " + except.HResult + Environment.NewLine;
            //TODO: Innerexception BugDetails.Text += "InnerException: " + except.InnerException + Environment.NewLine;
            BugDetails.Text += "Message: " + except.Message + Environment.NewLine;
            BugDetails.Text += "Source: " + except.Source + Environment.NewLine;
            BugDetails.Text += "StackTrace: " + except.StackTrace + Environment.NewLine;
        }

        private async void Report(object sender, RoutedEventArgs e)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Subject = "Discord Bug Report";
            emailMessage.Body = "Comment:" + Comment.Text + "\n\nDetails:\n" + BugDetails.Text + "\n\nBuild#:5.3.2";

            var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient("avid29@live.com");
            emailMessage.To.Add(emailRecipient);

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

            CloseButton_Click();
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click();
        }

        private void CloseButton_Click()
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void DontReport(object sender, RoutedEventArgs e)
        {
            CloseButton_Click();
        }
    }
}
