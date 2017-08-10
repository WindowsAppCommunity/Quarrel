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
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ExtendedMessageEditor : Page
    {
        public ExtendedMessageEditor()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
                BodyText.Text = e.Parameter.ToString();

        }
        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
            Session.MessageUploadProgress -= Session_MessageUploadProgress;
        }
        public Windows.Storage.StorageFile file = null;
        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");
            file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var props = await file.GetBasicPropertiesAsync();
                FileName.Content = file.Name;
                FileSize.Text = Common.HumanizeFileSize(Convert.ToInt32(props.Size));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            ProgressViewer.Visibility = Visibility.Visible;
            Session.MessageUploadProgress += Session_MessageUploadProgress;
            Session.CreateMessage(App.CurrentChannelId, Editor.Text, file);
        }

        private async void Session_MessageUploadProgress(IAsyncOperationWithProgress<Windows.Web.Http.HttpResponseMessage, Windows.Web.Http.HttpProgress> asyncInfo, Windows.Web.Http.HttpProgress progressInfo)
        {
            var percentage = (progressInfo.BytesSent / progressInfo.TotalBytesToSend) * 100;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressVal.Text = Convert.ToInt16(percentage).ToString() + "%";
                progressBar.Value = Convert.ToDouble(percentage);
                if (asyncInfo.Status == AsyncStatus.Completed)
                    CloseButton_Click(null, null);
            });
        }
    }
}
