using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using Discord_UWP.SimpleClasses;

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
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if(e.Parameter.GetType() == typeof(DataPackageView))
                {
                    var param = await (e.Parameter as DataPackageView).GetStorageItemsAsync();
                    foreach(var file in param)
                        AddAttachement(file as StorageFile);
                }
                else if(e.Parameter.GetType() == typeof(App.MessageEditorNavigationArgs))
                {
                    Editor.Text = (e.Parameter as App.MessageEditorNavigationArgs).Content;
                    if ((e.Parameter as App.MessageEditorNavigationArgs).Paste == true)
                    {
                        DataPackageView dataPackageView = Clipboard.GetContent();
                        HandleDataPackage(dataPackageView, "Clipboard");
                    }
                }
                else
                {
                    Editor.Text = e.Parameter.ToString();
                }   
            }

            if (App.shareop != null)
            {
                header.Text = App.GetString("/Dialogs/Share");
                shareTarget.Visibility = Visibility.Visible;
                mediumTrigger.MinWindowWidth = 10000;
                mediumTrigger.MinWindowHeight = 10000;
                SaveButton.IsEnabled = false;
                if (App.LoggedIn() == false)
                {
                    MessageDialog md = new MessageDialog(App.GetString("/Dialogs/MustBeLoggedIn"), App.GetString("/Dialogs/Sorry"));
                    await md.ShowAsync();
                    App.shareop.DismissUI();
                }
                else
                {
                    HandleDataPackage(App.shareop.Data, "Shared Image");
                    List<SimpleGuild> guilds = new List<SimpleGuild>();
                    await RESTCalls.SetupToken(true);
                    var userguilds = await RESTCalls.GetGuilds();
                    guilds.Add(new SimpleGuild() { Id = "@me", Name = App.GetString("/Main/DirectMessages"), ImageURL= "https://discordapp.com/assets/89576a4bb71f927eb20e8aef987b499b.svg" });
                    foreach (var guild in userguilds)
                        guilds.Add(GuildManager.CreateGuild(guild));
                    serverOption.ItemsSource = guilds;
                }
            }
            
        }
        private async void HandleDataPackage(DataPackageView data, string imagefilename)
        {
            if (data.Contains(StandardDataFormats.StorageItems))
            {
                foreach (var file in await data.GetStorageItemsAsync())
                    AddAttachement(file as StorageFile);
            }
            else if (data.Contains(StandardDataFormats.Bitmap))
            {
                var bmpDPV = await data.GetBitmapAsync();
                var bmpSTR = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(imagefilename+".png", CreationCollisionOption.OpenIfExists);
                using (var writeStream = (await bmpSTR.OpenStreamForWriteAsync()).AsRandomAccessStream())
                using (var readStream = await bmpDPV.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(readStream.CloneStream());
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);
                    encoder.SetSoftwareBitmap(await decoder.GetSoftwareBitmapAsync());
                    await encoder.FlushAsync();
                    AddAttachement(bmpSTR);
                }
            }
            else if (data.Contains(StandardDataFormats.Text))
            {
                Editor.Text = await data.GetTextAsync();
            }
            else if (data.Contains(StandardDataFormats.WebLink))
            {
                Editor.Text = (await data.GetWebLinkAsync()).ToString();
            }
            else if (data.Contains(StandardDataFormats.ApplicationLink))
            {
                Editor.Text = (await data.GetApplicationLinkAsync()).ToString();
            }
            else if (data.Contains(StandardDataFormats.Html))
            {
                var converter = new Html2Markdown.Converter();
                Editor.Text = converter.Convert(await data.GetHtmlFormatAsync());
            }
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
            RESTCalls.MessageUploadProgress -= Session_MessageUploadProgress; //TODO: Rig to App.Events
        }
        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");
            StorageFile file = await picker.PickSingleFileAsync();
            if ((await file.GetBasicPropertiesAsync()).Size > 8388608)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("file must be less than 8MBs");
                await dialog.ShowAsync();
                return;
            }
            if (file != null)
            {
                AddAttachement(file);
            }
        }

        ulong FullUploadSize = 0;
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(App.shareop != null)
            {
                if (channelOption.SelectedItem != null)
                    App.CurrentChannelId = ((SimpleChannel)channelOption.SelectedItem).Id;
                else
                    return;
            }
            ProgressViewer.Visibility = Visibility.Visible;
            RESTCalls.MessageUploadProgress += Session_MessageUploadProgress; //TODO: Rig to App.Events
            
            FullUploadSize = 0;
            foreach(var file in attachements)
            {
                var props = await file.Value.GetBasicPropertiesAsync();
                FullUploadSize = FullUploadSize + props.Size + 444;
            }
            FullUploadSize += Convert.ToUInt64(System.Text.Encoding.Unicode.GetByteCount(Editor.Text));
            bool first = true;
            int attachCount = attachements.Count();
            if (attachCount > 1)
                FileNB.Visibility = Visibility.Visible;
            string FileStr = App.GetString("/Dialogs/File");
            for (int i = 0; i < attachCount; i++)
            {
                FileNB.Text = FileStr + " " + (i+1).ToString() + "/" + attachCount;
                var file = attachements.ElementAt(i).Value;
                if (first)
                {
                    await RESTCalls.CreateMessage(App.CurrentChannelId, Editor.Text, file); //TODO: Rig to App.Events
                    first = false;
                }
                else
                {
                    await RESTCalls.CreateMessage(App.CurrentChannelId, Editor.Text, file); //TODO: Rig to App.Events
                }
                var props = await file.GetBasicPropertiesAsync();
                //444 is an approximation of the http request overhead
                ulong overheadsize = 444;
                if(first)
                    overheadsize += Convert.ToUInt64(System.Text.Encoding.Unicode.GetByteCount(Editor.Text));
                FullBytesSentBuffer = FullBytesSentBuffer + props.Size + overheadsize;
            }
            RESTCalls.MessageUploadProgress -= Session_MessageUploadProgress; //TODO: Rig to App.Events
            if (App.shareop == null)
                CloseButton_Click(null, null);
            else
                App.shareop.DismissUI();
        }

        ulong FullBytesSentBuffer = 0;
        private async void Session_MessageUploadProgress(IAsyncOperationWithProgress<Windows.Web.Http.HttpResponseMessage, Windows.Web.Http.HttpProgress> asyncInfo, Windows.Web.Http.HttpProgress progressInfo)
        {
            double percentage = Convert.ToDouble((100 * (FullBytesSentBuffer + progressInfo.BytesSent)) / FullUploadSize);
            if (percentage > 100) percentage = 100;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressVal.Text = percentage.ToString() + "%";
                progressBar.Value = percentage;
            });
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            DragLeave.Begin();
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                AddAttachement((await e.DataView.GetStorageItemsAsync()).First() as StorageFile);
            }
        }

        private Dictionary<Guid, StorageFile> attachements = new Dictionary<Guid, StorageFile>();
        private async void AddAttachement(StorageFile file)
        {
            if (file == null) return;
            var props = await file.GetBasicPropertiesAsync();
            var guid = Guid.NewGuid();
            var attachement = new Controls.AttachementControl()
            {
                IsFake = true,
                DisplayedAttachement = 
                new SharedModels.Attachment()
                {
                    Filename = file.Name,
                    Url = file.Path,
                    Size = props.Size
                },
                
                Tag = guid
            };
            stacker.Children.Add(attachement);
            attachements.Add(guid, file);
            attachement.Delete += (sender, e) =>
            {
                var id = (Guid)(sender as Controls.AttachementControl).Tag;
                attachements.Remove(id);
                stacker.Children.Remove(sender as Controls.AttachementControl);
            };
        }

        private void rectangle_DragEnter(object sender, DragEventArgs e)
        {
            DragOver.Begin();
        }

        private void rectangle_DragLeave(object sender, DragEventArgs e)
        {
            DragLeave.Begin();
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            
            AddAttachement(photo);
        }

        private MediaCapture CaptureMedia;
        private IRandomAccessStream AudioStream;
        private DispatcherTimer DishTImer;
        private TimeSpan SpanTime;
        bool recording = false;
        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (!recording)
            {
                try
                {
                    CaptureMedia = new MediaCapture();
                    var captureSettings = new MediaCaptureInitializationSettings();
                    captureSettings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                    await CaptureMedia.InitializeAsync(captureSettings);
                    CaptureMedia.Failed += CaptureMedia_Failed;
                    CaptureMedia.RecordLimitationExceeded += CaptureMedia_RecordLimitationExceeded;

                    DishTImer = new DispatcherTimer();
                    DishTImer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    DishTImer.Tick += DishTImer_Tick;

                    AudioStream = new InMemoryRandomAccessStream();
                    try
                    {
                        MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
                        await CaptureMedia.StartRecordToStreamAsync(encodingProfile, AudioStream);
                    }
                    catch
                    {
                        MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
                        await CaptureMedia.StartRecordToStreamAsync(encodingProfile, AudioStream);
                    }

                    DishTImer.Start();
                    recording = true;
                }
                catch { }
                
            }
            else
            {
                await CaptureMedia.StopRecordAsync();
                RecordButton.Text = App.GetString("/Dialogs/SavingAudio");
                recording = false;
                RecordHyperlink.IsEnabled = false;
                DishTImer.Stop();
                var mediaFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Audio.mp3",CreationCollisionOption.GenerateUniqueName);
                using (var dataReader = new DataReader(AudioStream.GetInputStreamAt(0)))
                {
                    await dataReader.LoadAsync((uint)AudioStream.Size);
                    byte[] buffer = new byte[(int)AudioStream.Size];
                    dataReader.ReadBytes(buffer);
                    await FileIO.WriteBytesAsync(mediaFile, buffer);
                    RecordButton.Text = App.GetString("/Dialogs/AdvancedRecordSoundTB");
                    RecordHyperlink.IsEnabled = true;
                }
                AddAttachement(mediaFile);
            }
        }
        string recordingstr = App.GetString("/Dialogs/RecordingAudio");
        private void DishTImer_Tick(object sender, object e)
        {
            SpanTime = SpanTime.Add(DishTImer.Interval);
            RecordButton.Text = recordingstr + " " + SpanTime.ToString("mm\\:ss\\.f");
        }

        private async void CaptureMedia_RecordLimitationExceeded(MediaCapture sender)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await sender.StopRecordAsync();
            });
        }

        private async void CaptureMedia_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await sender.StopRecordAsync();
            });
        }

        private async void serverOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var guild = (SimpleGuild)e.AddedItems[0];
            List<SimpleChannel> channels = new List<SimpleChannel>();
            if(guild.Id == "@me")
            {
                var userchannels = await RESTCalls.GetDMs();
                foreach(var channel in userchannels)
                {
                    SimpleChannel c = new SimpleChannel();
                    c.Id = channel.Id;
                    if (!string.IsNullOrEmpty(channel.Name))
                        c.Name = channel.Name;
                    else
                        c.Name = channel.Users.First().Username;
                    c.LastMessageId = "@";
                    channels.Add(c);
                }
            }
            else
            {
                var userchannels = await RESTCalls.GetGuildChannels(guild.Id);
                foreach (var channel in userchannels)
                    if (channel.Type != 2 && channel.Type != 4)
                        channels.Add(ChannelManager.MakeChannel(channel, "#"));
            }         
            channelOption.ItemsSource = channels;
        }

        private void channelOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
                SaveButton.IsEnabled = true;
            else
                SaveButton.IsEnabled = false;
        }
    }
}
