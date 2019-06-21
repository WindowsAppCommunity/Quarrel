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
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.ApplicationModel.Contacts;
using Quarrel.LocalModels;
using Quarrel.Managers;
using Quarrel.SimpleClasses;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
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
        Contact shareContact = null;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter != null)
            {
                if(e.Parameter is DataPackageView view)
                {
                    var param = await view.GetStorageItemsAsync();
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
                else if(e.Parameter is Contact contact)
                {
                    shareContact = contact;
                }
                else
                {
                    Editor.Text = e.Parameter.ToString();
                }   
            }

            if (App.shareop != null)
            {
                header.Text = App.GetString("/Dialogs/Share");
                if(shareContact == null)
                {
                    shareTarget.Visibility = Visibility.Visible;
                    SaveButton.IsEnabled = false;
                }
                    
                mediumTrigger.MinWindowWidth = 10000;
                mediumTrigger.MinWindowHeight = 10000;
                if (App.LoggedIn() == false)
                {
                    MessageDialog md = new MessageDialog(App.GetString("/Dialogs/MustBeLoggedIn"), App.GetString("/Dialogs/Sorry"));
                    await md.ShowAsync();
                    App.shareop.DismissUI();
                }
                else
                {
                    HandleDataPackage(App.shareop.Data, "Shared Image");
                    await RESTCalls.SetupToken(true);
                    
                    if (shareContact == null)
                    {
                        List<SimpleGuild> guilds = new List<SimpleGuild>();
                        var userguilds = await RESTCalls.GetGuilds();
                        guilds.Add(new SimpleGuild() { Id = "@me", Name = App.GetString("/Main/DirectMessages"), ImageURL = "https://discordapp.com/assets/89576a4bb71f927eb20e8aef987b499b.svg" });
                        foreach (var guild in userguilds)
                            guilds.Add(GuildManager.CreateGuild(guild));
                        serverOption.ItemsSource = guilds;
                    }
                    else
                    {
                       
                    }
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
            if (App.shareop != null)
                App.shareop.DismissUI();
            else
            {
                scale.CenterY = this.ActualHeight / 2;
                scale.CenterX = this.ActualWidth / 2;
                NavAway.Begin();
                App.SubpageClosed();
                RESTCalls.MessageUploadProgress -= Session_MessageUploadProgress;
            }
        }
        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            if (App.IsDesktop)
            {
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            } else
            {
                //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            }
            picker.FileTypeFilter.Add("*");
            var files = await picker.PickMultipleFilesAsync();
            List<string> FilesTooLarge = new List<string>();
            foreach (var file in files)
            {
                if (file == null) continue;
                var filesize = (await file.GetBasicPropertiesAsync()).Size;
                if(LocalState.CurrentUser != null && LocalState.CurrentUser.Premium && filesize > 52428800)
                {
                    FilesTooLarge.Add(file.DisplayName);
                }
                else if (filesize > 8388608)
                {
                    FilesTooLarge.Add(file.DisplayName);
                }
                else
                {
                    AddAttachement(file);
                }
            }

            if (FilesTooLarge.Count == 1)
            {
                Windows.UI.Popups.MessageDialog dialog = new MessageDialog("The file " + FilesTooLarge[0] + " is too big to attach (>8MB)");
                await dialog.ShowAsync();
            }
            else if (FilesTooLarge.Count > 1)
            {
                Windows.UI.Popups.MessageDialog dialog = new MessageDialog("The files " + string.Join(",", FilesTooLarge) + " are too big to attach (>8MB)");
                await dialog.ShowAsync();
            }
        }

        ulong FullUploadSize = 0;
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(App.shareop != null)
            {
                if(shareContact != null)
                {
                    if (shareContact.Id != null)
                        App.CurrentChannelId = shareContact.RemoteId;
                }
                else
                {
                    if (channelOption.SelectedItem != null)
                        App.CurrentChannelId = ((SimpleChannel)channelOption.SelectedItem).Id;
                    else
                        return;
                }

            }
            ProgressViewer.Visibility = Visibility.Visible;
            RESTCalls.MessageUploadProgress += Session_MessageUploadProgress;
            
            FullUploadSize = 0;
            foreach(var file in attachements)
            {
                var props = await file.Value.GetBasicPropertiesAsync();
                FullUploadSize = FullUploadSize + props.Size + 444;
            }
            FullUploadSize += Convert.ToUInt64(System.Text.Encoding.Unicode.GetByteCount(Editor.Text));
            int attachCount = attachements.Count();
            if (attachCount > 1)
                FileNB.Visibility = Visibility.Visible;
            string FileStr = App.GetString("/Dialogs/File");
            for (int i = 0; i < attachCount; i++)
            {
                FileNB.Text = FileStr + " " + (i+1) + "/" + attachCount;
                var file = attachements.ElementAt(i).Value;
                var props = await file.GetBasicPropertiesAsync();
                //444 is an approximation of the http request overhead
                ulong overheadsize = 444;
                if (i == 0)
                    overheadsize += Convert.ToUInt64(System.Text.Encoding.Unicode.GetByteCount(Editor.Text));
                _fullBytesSentBuffer = _fullBytesSentBuffer + props.Size + overheadsize;
                _lockWaitState = false;
                progressBar.Value = 0;
                if (i==0)
                {
                    await RESTCalls.CreateMessage(App.CurrentChannelId, Editor.Text, file);
                }
                else
                {
                    await RESTCalls.CreateMessage(App.CurrentChannelId, "", file);
                }
                
            }
            RESTCalls.MessageUploadProgress -= Session_MessageUploadProgress;
            if (App.shareop == null)
                CloseButton_Click(null, null);
            else
                App.shareop.ReportCompleted();
        }

        private ulong _fullBytesSentBuffer;

        private bool _lockWaitState = false;
        private async void Session_MessageUploadProgress(IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> asyncInfo, HttpProgress progressInfo)
        {
            float dest = Convert.ToSingle((100 * (_fullBytesSentBuffer + progressInfo.BytesSent))/ _fullBytesSentBuffer);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                while (progressBar.Value < dest)
                {
                    progressVal.Text = progressBar.Value + "%";
                    progressBar.Value++;
                    await Task.Delay(10);
                }
                if (_lockWaitState || progressInfo.Stage == HttpProgressStage.WaitingForResponse || progressInfo.Stage == HttpProgressStage.ReceivingHeaders || progressInfo.Stage == HttpProgressStage.ReceivingContent)
                {
                    _lockWaitState = true;
                    progressVal.Visibility = Visibility.Collapsed;
                    Waiting.Visibility = Visibility.Visible;
                }
                else
                {
                    progressVal.Visibility = Visibility.Visible;
                    Waiting.Visibility = Visibility.Collapsed;
                }
            });
        }

        
        public static class MathHelper
        {
            public const float Pi = (float)Math.PI;
            public const float HalfPi = (float)(Math.PI / 2);

            public static float Lerp(double from, double to, double step)
            {
                return (float)((to - from) * step + from);
            }
        }
        public static float EaseOut(double s, int power)
        {
            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(s - 1, power) + sign));
        }
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
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
                new DiscordAPI.SharedModels.Attachment()
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await sender.StopRecordAsync();
            });
        }

        private async void CaptureMedia_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
                    else if(channel.Users != null && channel.Users.Any())
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
