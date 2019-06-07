
using System;
using System.IO;
using System.IO.Compression;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Debug : Page
    {
        public Debug()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
            foreach (string item in App.eventList)
            {
                Events.Items.Add(new ListViewItem() { Content = item });
            }
            App.EventListUpdatedHandler += App_EventListUpdatedHandler;
            _compressed = new MemoryStream();
            _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void App_EventListUpdatedHandler(object sender, string e)
        {
            Events.Items.Add(new ListViewItem() { Content = e });
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
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
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
           if (e.Key == Windows.System.VirtualKey.Enter)
            {
                HandleMessage(decodeBox.Text);
            }
        }

        private MemoryStream _compressed;
        private DeflateStream _decompressor;
        private async void HandleMessage(string base64)
        {
            try
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    ms.Position = 0;
                    byte[] data = new byte[ms.Length];
                    ms.Read(data, 0, (int)ms.Length);
                    int index = 0;
                    int count = data.Length;
                    using (var decompressed = new MemoryStream())
                    {
                        if (data[0] == 0x78)
                        {
                            _compressed.Write(data, index + 2, count - 2);
                            _compressed.SetLength(count - 2);
                        }
                        else
                        {
                            _compressed.Write(data, index, count);
                            _compressed.SetLength(count);
                        }

                        _compressed.Position = 0;
                        _decompressor.CopyTo(decompressed);
                        _compressed.Position = 0;
                        decompressed.Position = 0;

                        using (var reader = new StreamReader(decompressed))
                        {
                            string content = await reader.ReadToEndAsync();
                            decodeBox.Text = content;
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                decodeBox.Text = "Failed to decode text";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("BSOD Test");
        }
    }
}