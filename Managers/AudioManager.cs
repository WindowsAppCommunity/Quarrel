using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.UI;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Services.Store;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static Discord_UWP.Common;
using Discord_UWP.LocalModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.Gateway;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
using FFT.Processing;

namespace Discord_UWP
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public static class AudioManager
    {
        private static int inGraphCount = 0;
        private static int outGraphCount = 0;

        public static event EventHandler<float[]> InputRecieved;
        
        static AudioGraph ingraph;
        static AudioGraph outgraph;

        private static AudioDeviceInputNode deviceInputNode;
        private static AudioDeviceOutputNode deviceOutputNode;

        private static string OutputDeviceID;
        private static string InputDeviceID;

        private static AudioFrameOutputNode frameOutputNode;
        private static AudioFrameInputNode frameInputNode;

        private static DeviceWatcher inputDeviceWatcher;
        private static DeviceWatcher outputDeviceWatcher;

        private static int quantum = 0;
        private static double theta = 0;
        private static bool ready = false;
        public static float AudioSpec1 = 0;
        public static float AudioSpec2 = 0;
        public static float AudioSpec3 = 0;
        public static float AudioSpec4 = 0;
        public static float AudioSpec5 = 0;
        public static float AudioSpec6 = 0;
        public static float AudioSpec7 = 0;
        public static float AudioSpec8 = 0;
        public static float AudioSpec9 = 0;
        public static float AudioAverage = 0;

        //private static bool started = false;
        //public static 

        public static async void ChangeVolume(double value)
        {
            if(ready)
                deviceOutputNode.OutgoingGain = value;
        }
        public static async void ChangeDeafStatus(bool deaf)
        {
            if (ready)
                if (deaf)
                {
                    deviceOutputNode.Stop();
                }
                else
                {
                    deviceOutputNode.Start();
                }
        }

        public static async Task CreateAudioGraphs()
        {
            Storage.SettingsChangedHandler += Storage_SettingsChangedHandler;
            await CreateOutputDeviceNode(Storage.Settings.OutputDevice);
            await CreateInputDeviceNode(Storage.Settings.InputDevice);
        }

        private static async void Storage_SettingsChangedHandler(object sender, EventArgs e)
        {
            await CreateOutputDeviceNode(Storage.Settings.OutputDevice);
            await CreateInputDeviceNode(Storage.Settings.InputDevice);
        }

        public static async Task CreateOutputDeviceNode(string deviceId)
        {
            if (outgraph != null && OutputDeviceID != outgraph.PrimaryRenderDevice.Id)
            {
                HeavyDisposeOutGraph();
            } else
            {
                outGraphCount++;
            }

            Console.WriteLine("Creating AudioGraphs");
            // Create an AudioGraph with default settings
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Media);
            graphsettings.EncodingProperties = new AudioEncodingProperties();
            graphsettings.EncodingProperties.Subtype = "Float";
            graphsettings.EncodingProperties.SampleRate = 48000;
            graphsettings.EncodingProperties.ChannelCount = 2;
            graphsettings.EncodingProperties.BitsPerSample = 32;
            graphsettings.EncodingProperties.Bitrate = 3072000;

            DeviceInformation selectedDevice;
            if (deviceId == "Default" || deviceId == null)
            {
                selectedDevice = await DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioRenderId(Windows.Media.Devices.AudioDeviceRole.Default));
                Windows.Media.Devices.MediaDevice.DefaultAudioRenderDeviceChanged += MediaDevice_DefaultAudioRenderDeviceChanged;
                //outputDeviceWatcher = DeviceInformation.CreateWatcher(DeviceClass.AudioRender);
                //outputDeviceWatcher.Added += OutputDeviceWatcher_Added;
                //outputDeviceWatcher.Removed += OutputDeviceWatcher_Removed;
                //outputDeviceWatcher.Updated += OutputDeviceWatcher_Updated;
            }
            else
            {
                try
                {
                    selectedDevice = await DeviceInformation.CreateFromIdAsync(deviceId);
                }
                catch
                {
                    selectedDevice = await DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioRenderId(Windows.Media.Devices.AudioDeviceRole.Default));
                    deviceId = "Default";
                }
            }

            graphsettings.PrimaryRenderDevice = selectedDevice;

            CreateAudioGraphResult graphresult = await AudioGraph.CreateAsync(graphsettings);

            if (graphresult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            outgraph = graphresult.Graph;

            // Create a device output node
            
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await outgraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
            }

            deviceOutputNode = deviceOutputNodeResult.DeviceOutputNode;

            // Create the FrameInputNode at the same format as the graph, except explicitly set stereo.
            frameInputNode = outgraph.CreateFrameInputNode(outgraph.EncodingProperties);
            frameInputNode.AddOutgoingConnection(deviceOutputNode);
            OutputDeviceID = deviceId;
            frameInputNode.Start();
            ready = true;
            outgraph.Start();
        }

        public static async Task CreateInputDeviceNode(string deviceId)
        {
            if (ingraph != null && deviceId != InputDeviceID)
            {
                HeavyDisposeInGraph();
            } else
            {
                inGraphCount++;
            }

            Console.WriteLine("Creating AudioGraphs");
            // Create an AudioGraph with default settings
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Media);
            graphsettings.EncodingProperties = new AudioEncodingProperties();
            graphsettings.EncodingProperties.Subtype = "Float";
            graphsettings.EncodingProperties.SampleRate = 48000;
            graphsettings.EncodingProperties.ChannelCount = 2;
            graphsettings.EncodingProperties.BitsPerSample = 32;
            graphsettings.EncodingProperties.Bitrate = 3072000;
            //settings.DesiredSamplesPerQuantum = 960;
            //settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            CreateAudioGraphResult graphresult = await AudioGraph.CreateAsync(graphsettings);

            if (graphresult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            ingraph = graphresult.Graph;


            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
            nodesettings.EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32);
            nodesettings.DesiredSamplesPerQuantum = 960;
            nodesettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            frameOutputNode = ingraph.CreateFrameOutputNode(ingraph.EncodingProperties);
            quantum = 0;
            ingraph.QuantumStarted += Graph_QuantumStarted;

            DeviceInformation selectedDevice;

            if (deviceId == "Default" || deviceId == null)
            {
                selectedDevice = await DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Default));
                Windows.Media.Devices.MediaDevice.DefaultAudioCaptureDeviceChanged += MediaDevice_DefaultAudioCaptureDeviceChanged;
                //inputDeviceWatcher = DeviceInformation.CreateWatcher(DeviceClass.AudioCapture);
                //inputDeviceWatcher.Added += InputDeviceWatcher_Added;
                //inputDeviceWatcher.Removed += InputDeviceWatcher_Removed;
                //inputDeviceWatcher.Updated += InputDeviceWatcher_Updated;
            }
            else
            {
                try
                {
                    selectedDevice = await DeviceInformation.CreateFromIdAsync(deviceId);
                }
                catch
                {
                    selectedDevice = await DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Default));
                    deviceId = "Default";
                }
            }

            CreateAudioDeviceInputNodeResult result =
                await ingraph.CreateDeviceInputNodeAsync(MediaCategory.Media, nodesettings.EncodingProperties, selectedDevice);
            if (result.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                return;
            }


            deviceInputNode = result.DeviceInputNode;
            deviceInputNode.AddOutgoingConnection(frameOutputNode);
            InputDeviceID = deviceId;
            frameOutputNode.Start();
            ingraph.Start();
        }

        #region OuputUpdate

        private static async void MediaDevice_DefaultAudioRenderDeviceChanged(object sender, Windows.Media.Devices.DefaultAudioRenderDeviceChangedEventArgs args)
        {
            HeavyDisposeOutGraph();
            await CreateOutputDeviceNode(OutputDeviceID);
        }

        //private static void OutputDeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        //{
        //    if (args.IsDefault)
        //    {
        //        UpdateOutputDeviceID("Default");
        //    }
        //}

        //private static void OutputDeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        //{
        //    UpdateOutputDeviceID("Default");
        //}

        //private static void OutputDeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        //{
        //    UpdateOutputDeviceID("Default");
        //}

        #endregion

        #region InputUpdate

        private static void MediaDevice_DefaultAudioCaptureDeviceChanged(object sender, Windows.Media.Devices.DefaultAudioCaptureDeviceChangedEventArgs args)
        {
            //TODO: Update InputDevice
        }

        //private static void InputDeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        //{
        //    //TODO: Update InputDevice
        //}

        //private static void InputDeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        //{

        //}

        //private static void InputDeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        //{

        //}

        #endregion


        public static void LightDisposeAudioGraphs()
        {
            LightDisposeInGraph();
            LightDisposeOutGraph();
        }

        public static void LightDisposeInGraph()
        {
            if (inGraphCount <= 1)
            {
                ingraph?.Dispose();
                frameOutputNode = null;
                deviceInputNode = null;
                ingraph = null;
                inGraphCount = 0;
            }
            else
            {
                inGraphCount--;
            }
        }

        public static void LightDisposeOutGraph()
        {
            if (outGraphCount <= 1)
            {
                outgraph?.Dispose();
                frameInputNode = null;
                deviceOutputNode = null;
                outgraph = null;
                outGraphCount = 0;
            } else
            {
                outGraphCount--;
            }
        }

        public static void HeavyDisposeAudioGraphs()
        {
            HeavyDisposeInGraph();
            HeavyDisposeOutGraph();
        }

        public static void HeavyDisposeInGraph()
        {
            ingraph?.Dispose();
            frameOutputNode = null;
            deviceInputNode = null;
            ingraph = null;
        }

        public static void HeavyDisposeOutGraph()
        {
            outgraph?.Dispose();
            frameInputNode = null;
            deviceOutputNode = null;
            outgraph = null;
        }

        public static unsafe void AddFrame(float[] framedata, uint samples)
        {
            if (!ready)
                return;
            //if (!started)
            //{
            //    //graph.Start();
            //    //started = true;
            //}
                AudioFrame frame = new AudioFrame(samples * 2 * sizeof(float));
                using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
                using (IMemoryBufferReference reference = buffer.CreateReference())
                {
                    // Get the buffer from the AudioFrame
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint _);
                    // Cast to float since the data we are generating is float
                    float* dataInFloat = (float*)dataInBytes;
                    fixed (float* frames = framedata)
                    {
                        for (int i = 0; i < samples * 2; i++)
                        {
                            dataInFloat[i] = frames[i];
                        }
                    }
                }
                if (LocalState.VoiceState.SelfDeaf || LocalState.VoiceState.ServerDeaf)
                {
                    AudioSpec1 = 0;
                    AudioSpec2 = 0;
                    AudioSpec3 = 0;
                    AudioSpec4 = 0;
                    AudioSpec5 = 0;
                    AudioSpec6 = 0;
                    AudioSpec7 = 0;
                    AudioSpec8 = 0;
                    AudioSpec9 = 0;
                    AudioAverage = 0;
                }
                else
                {
                    List<float[]> amplitudeData = FFT.Processing.HelperMethods.ProcessFrameOutput(frame);
                    List<float[]> channelData = FFT.Processing.HelperMethods.GetFftData(FFT.Processing.HelperMethods.ConvertTo512(amplitudeData, outgraph), outgraph);

                    float[] leftChannel = channelData[1];

                    AudioSpec1 = HelperMethods.Max(leftChannel, 0, 1);
                    AudioSpec2 = HelperMethods.Max(leftChannel, 2, 3);
                    AudioSpec3 = HelperMethods.Max(leftChannel, 3, 4);
                    AudioSpec4 = HelperMethods.Max(leftChannel, 4, 5);
                    AudioSpec5 = HelperMethods.Max(leftChannel, 5, 6);
                    AudioSpec6 = HelperMethods.Max(leftChannel, 7, 8);
                    AudioSpec7 = HelperMethods.Max(leftChannel, 9, 10);
                    AudioSpec8 = HelperMethods.Max(leftChannel, 10, 12);
                    AudioSpec9 = HelperMethods.Max(leftChannel, 14, 26);
                    AudioAverage = (AudioSpec1 + AudioSpec2 + AudioSpec3 + AudioSpec4 + AudioSpec5 + AudioSpec5 + AudioSpec6 + AudioSpec7 + AudioSpec8 + AudioSpec9) / 9;
                }
                frameInputNode.AddFrame(frame);
        }

        public static unsafe AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample) * (number of channels)
            uint bufferSize = samples * sizeof(float) * 2;
            AudioFrame frame = new AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint _);

                // Cast to float since the data we are generating is float
                float* dataInFloat = (float*)dataInBytes;

                float freq = 17000; // choosing to generate frequency of 17kHz
                float amplitude = 0.3f;
                int sampleRate = (int)outgraph.EncodingProperties.SampleRate;
                double sampleIncrement = (freq * (Math.PI * 2)) / sampleRate;



                // Generate a 17kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++)
                {
                    double sinValue = amplitude * Math.Sin(theta);
                    dataInFloat[i] = (float)sinValue;
                    theta += sampleIncrement;
                }
            }

            #region Empty Generation
            //using (IMemoryBufferReference reference = buffer.CreateReference())
            //{
            //    byte* dataInBytes;
            //    uint capacityInBytes;
            //    float* dataInFloat;

            //    Get the buffer from the AudioFrame
            //   ((IMemoryBufferByteAccess) reference).GetBuffer(out dataInBytes, out capacityInBytes);

            //    Cast to float since the data we are generating is float
            //   dataInFloat = (float*)dataInBytes;

            //    float freq = 13000; // choosing to generate frequency of 13kHz
            //    float amplitude = 0.3f;
            //    int sampleRate = (int)graph.EncodingProperties.SampleRate;
            //    double sampleIncrement = (freq * (Math.PI * 2)) / sampleRate;



            //    Generate a 1kHz sine wave and populate the values in the memory buffer
            //        for (int i = 0; i < samples; i++)
            //    {
            //        double sinValue = amplitude * Math.Sin(theta);
            //        dataInFloat[i] = (float)sinValue;
            //        theta += sampleIncrement;
            //    }
            //}
            #endregion

            return frame;
        }

        private static void Graph_QuantumStarted(AudioGraph sender, object args)
        {
           if (++quantum % 2 == 0)
           {
               AudioFrame frame = frameOutputNode.GetFrame();
               ProcessFrameOutput(frame);
           }
        }

        private static unsafe void ProcessFrameOutput(AudioFrame frame)
        {
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                float* dataInFloat = (float*)dataInBytes;
                float[] dataInFloats = new float[capacityInBytes/sizeof(float)];

                for (int i = 0; i < capacityInBytes / sizeof(float); i++)
                {
                    dataInFloats[i] = dataInFloat[i];
                }

                InputRecieved?.Invoke(null, dataInFloats);
            }
        }

        private static void node_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;
            if (numSamplesNeeded != 0)
            {
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);
                frameInputNode.AddFrame(audioData);
            }
        }

        public static async void UpdateOutputDeviceID(string outID)
        {
            if (outID != OutputDeviceID)
            {
                await CreateOutputDeviceNode(outID);
            }
        }

        public static async void UpdateInputDeviceID(string inID)
        {
            if (inID != InputDeviceID)
            {
                await CreateInputDeviceNode(inID);
            }
        }


        public static async void PlaySoundEffect(string file)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    MediaElement element = new MediaElement();
                    element.AudioCategory = AudioCategory.Alerts;
                    element.SetPlaybackSource(Windows.Media.Core.MediaSource.CreateFromUri(
                        new Uri("ms-appx:///Assets/SoundEffects/" +
                                (Storage.Settings.DiscordSounds ? "discord" : "windows") + "_" + file + ".mp3")));
                    element.MediaEnded += Element_MediaEnded;
                    element.Play();
                });
        }

        public static async void PlaySoundEffect(string file, string type)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    MediaElement element = new MediaElement();
                    element.AudioCategory = AudioCategory.Alerts;
                    element.SetPlaybackSource(
                        Windows.Media.Core.MediaSource.CreateFromUri(
                            new Uri("ms-appx:///Assets/SoundEffects/" + type + "_" + file + ".mp3")));
                    element.MediaEnded += Element_MediaEnded;
                    element.Play();
                });
        }

        private static void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Dispose?
        }
    }
}
