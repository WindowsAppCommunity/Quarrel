using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using FFT.Processing;
using Quarrel.LocalModels;

namespace Quarrel.Managers
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public class AudioManager
    {
        /// <summary>
        /// Current number of places the ingraph is in use
        /// </summary>
        private static int inGraphCount = 0;

        /// <summary>
        /// Current number of places the outgraph is in use
        /// </summary>
        private static int outGraphCount = 0;

        /// <summary>
        /// Event for when Audio packet is recorded
        /// </summary>
        public static event EventHandler<float[]> InputRecieved;
        
        /// <summary>
        /// AudioGraph for recording
        /// </summary>
        static AudioGraph ingraph;

        /// <summary>
        /// AudioGraph for playing
        /// </summary>
        static AudioGraph outgraph;

        /// <summary>
        /// Audio recording device
        /// </summary>
        private static AudioDeviceInputNode deviceInputNode;

        /// <summary>
        /// Audio playback device
        /// </summary>
        private static AudioDeviceOutputNode deviceOutputNode;

        /// <summary>
        /// Audio playback device Id
        /// </summary>
        private static string OutputDeviceID;

        /// <summary>
        /// Audio recording device Id
        /// </summary>
        private static string InputDeviceID;

        /// <summary>
        /// Frame output node on ingraph
        /// </summary>
        private static AudioFrameOutputNode frameOutputNode;
        
        /// <summary>
        /// Frame input node on outgraph
        /// </summary>
        private static AudioFrameInputNode frameInputNode;

        /// <summary>
        /// Device watcher for input changing
        /// </summary>
        private static DeviceWatcher inputDeviceWatcher;
        
        /// <summary>
        /// Device watcher for output changing
        /// </summary>
        private static DeviceWatcher outputDeviceWatcher;

        /// <summary>
        /// Current quantum
        /// </summary>
        private static int quantum = 0;

        /// <summary>
        /// Current theta
        /// </summary>
        private static double theta = 0;

        /// <summary>
        /// True if ready for playback and recording
        /// </summary>
        private static bool ready = false;

        // Output FFT specs
        public static float AudioOutSpec1 = 0;
        public static float AudioOutSpec2 = 0;
        public static float AudioOutSpec3 = 0;
        public static float AudioOutSpec4 = 0;
        public static float AudioOutSpec5 = 0;
        public static float AudioOutSpec6 = 0;
        public static float AudioOutSpec7 = 0;
        public static float AudioOutSpec8 = 0;
        public static float AudioOutSpec9 = 0;
        public static float AudioOutAverage = 0;

        // Input FFT specs
        public static float AudioInSpec1 = 0;
        public static float AudioInSpec2 = 0;
        public static float AudioInSpec3 = 0;
        public static float AudioInSpec4 = 0;
        public static float AudioInSpec5 = 0;
        public static float AudioInSpec6 = 0;
        public static float AudioInSpec7 = 0;
        public static float AudioInSpec8 = 0;
        public static float AudioInSpec9 = 0;
        public static float AudioInAverage = 0;
        
        /// <summary>
        /// Adjust volume (1 is normal)
        /// </summary>
        /// <param name="value">volume level</param>
        public static void ChangeVolume(double value)
        {
            if(ready)
                deviceOutputNode.OutgoingGain = value;
        }

        /// <summary>
        /// Mute output
        /// </summary>
        /// <param name="deaf">Deafen status</param>
        public static void ChangeDeafStatus(bool deaf)
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

        /// <summary>
        /// Create input and output audio graphs
        /// </summary>
        public static async Task CreateAudioGraphs()
        {
            Storage.SettingsChangedHandler += Storage_SettingsChangedHandler;
            await CreateOutputDeviceNode(Storage.Settings.OutputDevice);
            await CreateInputDeviceNode(Storage.Settings.InputDevice);
        }

        /// <summary>
        /// Handle settings changing to recreate graphs
        /// </summary>
        private static async void Storage_SettingsChangedHandler(object sender, EventArgs e)
        {
            await CreateOutputDeviceNode(Storage.Settings.OutputDevice);
            await CreateInputDeviceNode(Storage.Settings.InputDevice);
        }

        /// <summary>
        /// Create output audio graph
        /// </summary>
        /// <param name="deviceId">Overload for default ouput device id</param>
        public static async Task CreateOutputDeviceNode(string deviceId = null)
        {
            // If not in use, redo dispose
            if (outgraph != null && OutputDeviceID != outgraph.PrimaryRenderDevice.Id)
            {
                HeavyDisposeOutGraph();
            }
            // Increment use counter
            else
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

            // Determine selected device
            DeviceInformation selectedDevice;
            if (deviceId == "Default" || deviceId == null)
            {
                selectedDevice = await DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioRenderId(Windows.Media.Devices.AudioDeviceRole.Default));
                Windows.Media.Devices.MediaDevice.DefaultAudioRenderDeviceChanged += MediaDevice_DefaultAudioRenderDeviceChanged;
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

            // Set selected device
            graphsettings.PrimaryRenderDevice = selectedDevice;

            // Create graph
            CreateAudioGraphResult graphresult = await AudioGraph.CreateAsync(graphsettings);
            if (graphresult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            // "Save" graph
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

            // Begin playing
            frameInputNode.Start();
            ready = true;
            outgraph.Start();
        }

        /// <summary>
        /// Create input audio graph
        /// </summary>
        /// <param name="deviceId">Override for default input device id</param>
        public static async Task<bool> CreateInputDeviceNode(string deviceId = null)
        {
            // If not in use, redo dispose
            if (ingraph != null && deviceId != InputDeviceID)
            {
                HeavyDisposeInGraph();
            }
            // Increment use counter
            else
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
            CreateAudioGraphResult graphresult = await AudioGraph.CreateAsync(graphsettings);

            if (graphresult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                inGraphCount--;
                LocalState.VoiceState.SelfMute = true;
                VoiceManager.lockMute = true;
                return false;
            }

            // "Save" graph
            ingraph = graphresult.Graph;

            // Create frameOutputNode
            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
            nodesettings.EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32);
            nodesettings.DesiredSamplesPerQuantum = 960;
            nodesettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            frameOutputNode = ingraph.CreateFrameOutputNode(ingraph.EncodingProperties);
            quantum = 0;
            ingraph.QuantumStarted += Graph_QuantumStarted;

            // Determine selected device
            DeviceInformation selectedDevice;
            if (deviceId == "Default" || deviceId == null)
            {
                string device = Windows.Media.Devices.MediaDevice.GetDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Default);
                if (!string.IsNullOrEmpty(device))
                {
                    selectedDevice = await DeviceInformation.CreateFromIdAsync(device);
                    Windows.Media.Devices.MediaDevice.DefaultAudioCaptureDeviceChanged += MediaDevice_DefaultAudioCaptureDeviceChanged;
                } else
                {
                    inGraphCount--;
                    LocalState.VoiceState.SelfMute = true;
                    VoiceManager.lockMute = true;
                    return false;
                }
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
                inGraphCount--;
                LocalState.VoiceState.SelfMute = true;
                VoiceManager.lockMute = true;
                return false;
            }


            // Attach input device
            deviceInputNode = result.DeviceInputNode;
            deviceInputNode.AddOutgoingConnection(frameOutputNode);
            InputDeviceID = deviceId;

            // Begin playing
            frameOutputNode.Start();
            ingraph.Start();
            return true;
        }

        #region OuputUpdate

        /// <summary>
        /// When registered input device changes, recreate graph with it
        /// </summary>
        private static async void MediaDevice_DefaultAudioRenderDeviceChanged(object sender, Windows.Media.Devices.DefaultAudioRenderDeviceChangedEventArgs args)
        {
            await CreateOutputDeviceNode(OutputDeviceID);
        }

        #endregion

        #region InputUpdate

        /// <summary>
        /// When registered input device changes, recreate graph with it
        /// </summary>
        private static async void MediaDevice_DefaultAudioCaptureDeviceChanged(object sender, Windows.Media.Devices.DefaultAudioCaptureDeviceChangedEventArgs args)
        {
            await CreateInputDeviceNode(args.Id);
        }

        #endregion

        /// <summary>
        /// Light dispose both graphs
        /// </summary>
        public static void LightDisposeAudioGraphs()
        {
            LightDisposeInGraph();
            LightDisposeOutGraph();
        }

        /// <summary>
        /// Light dispose in graph
        /// </summary>
        public static void LightDisposeInGraph()
        {
            // If last, heavy dipose
            if (inGraphCount <= 1)
            {
                HeavyDisposeInGraph();
                inGraphCount = 0;
            }
            // If not, decrement use count
            else
            {
                inGraphCount--;
            }
        }

        /// <summary>
        /// Light dipose out graph
        /// </summary>
        public static void LightDisposeOutGraph()
        {
            // If last, heavy dipose
            if (outGraphCount <= 1)
            {
                HeavyDisposeOutGraph();
                outGraphCount = 0;
            }
            // If not, decrement use count
            else
            {
                outGraphCount--;
            }
        }

        /// <summary>
        /// Heavy dipose both graphs
        /// </summary>
        public static void HeavyDisposeAudioGraphs()
        {
            HeavyDisposeInGraph();
            HeavyDisposeOutGraph();
        }

        /// <summary>
        /// Heavy dispose in graph
        /// </summary>
        public static void HeavyDisposeInGraph()
        {
            // Clear data
            ingraph?.Dispose();
            frameOutputNode = null;
            deviceInputNode = null;
            ingraph = null;
            AudioInSpec1 = 0;
            AudioInSpec2 = 0;
            AudioInSpec3 = 0;
            AudioInSpec4 = 0;
            AudioInSpec5 = 0;
            AudioInSpec6 = 0;
            AudioInSpec7 = 0;
            AudioInSpec8 = 0;
            AudioInSpec9 = 0;
            AudioInAverage = 0;
        }

        /// <summary>
        /// Heavy dipose out graph
        /// </summary>
        public static void HeavyDisposeOutGraph()
        {
            // Clear data
            outgraph?.Dispose();
            frameInputNode = null;
            deviceOutputNode = null;
            outgraph = null;
            AudioOutSpec1 = 0;
            AudioOutSpec2 = 0;
            AudioOutSpec3 = 0;
            AudioOutSpec4 = 0;
            AudioOutSpec5 = 0;
            AudioOutSpec6 = 0;
            AudioOutSpec7 = 0;
            AudioOutSpec8 = 0;
            AudioOutSpec9 = 0;
            AudioOutAverage = 0;
        }

        /// <summary>
        /// Add frame to out graph queue
        /// </summary>
        /// <param name="framedata">raw frame data</param>
        /// <param name="samples">sample count</param>
        public static unsafe void AddFrame(float[] framedata, uint samples)
        {
            // not ready, return
            if (!ready)
                return;

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

            // Don't bother if deafend
            if (LocalState.VoiceState.SelfDeaf || LocalState.VoiceState.ServerDeaf)
            {
                AudioOutSpec1 = 0;
                AudioOutSpec2 = 0;
                AudioOutSpec3 = 0;
                AudioOutSpec4 = 0;
                AudioOutSpec5 = 0;
                AudioOutSpec6 = 0;
                AudioOutSpec7 = 0;
                AudioOutSpec8 = 0;
                AudioOutSpec9 = 0;
                AudioOutAverage = 0;
            }
            else
            {
                // Determine FFT data
                List<float[]> amplitudeData = FFT.Processing.HelperMethods.ProcessFrameOutput(frame);
                List<float[]> channelData = FFT.Processing.HelperMethods.GetFftData(FFT.Processing.HelperMethods.ConvertTo512(amplitudeData, outgraph), outgraph);

                float[] leftChannel = channelData[1];

                // Assign each FFT data out channel
                AudioOutSpec1 = HelperMethods.Max(leftChannel, 0, 1);
                AudioOutSpec2 = HelperMethods.Max(leftChannel, 2, 3);
                AudioOutSpec3 = HelperMethods.Max(leftChannel, 3, 4);
                AudioOutSpec4 = HelperMethods.Max(leftChannel, 4, 5);
                AudioOutSpec5 = HelperMethods.Max(leftChannel, 5, 6);
                AudioOutSpec6 = HelperMethods.Max(leftChannel, 7, 8);
                AudioOutSpec7 = HelperMethods.Max(leftChannel, 9, 10);
                AudioOutSpec8 = HelperMethods.Max(leftChannel, 10, 12);
                AudioOutSpec9 = HelperMethods.Max(leftChannel, 14, 26);
                AudioOutAverage = (AudioOutSpec1 + AudioOutSpec2 + AudioOutSpec3 + AudioOutSpec4 + AudioOutSpec5 + AudioOutSpec5 + AudioOutSpec6 + AudioOutSpec7 + AudioOutSpec8 + AudioOutSpec9) / 9;
            }

            // Add frame to queue
            frameInputNode.AddFrame(frame);
        }

        /// <summary>
        /// Generates empty data for a neccessary quantity of samples
        /// </summary>
        /// <param name="samples">Sampel count</param>
        /// <returns>AudioFrame of sample count</returns>
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

            return frame;
        }

        /// <summary>
        /// Handle new quantum from in graph
        /// </summary>
        private static void Graph_QuantumStarted(AudioGraph sender, object args)
        {
            // If odd quantum
           if (++quantum % 2 == 0)
           {
               try
               {
                   // Record frame
                   AudioFrame frame = frameOutputNode.GetFrame();
                   ProcessFrameOutput(frame);
               }
               catch (Exception e)
               {
                   Debug.WriteLine(e);
               }
           }
        }

        /// <summary>
        /// Handle frame of mic input
        /// </summary>
        /// <param name="frame"></param>
        private static unsafe void ProcessFrameOutput(AudioFrame frame)
        {
            float[] dataInFloats;
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                float* dataInFloat = (float*)dataInBytes;
                dataInFloats = new float[capacityInBytes/sizeof(float)];

                for (int i = 0; i < capacityInBytes / sizeof(float); i++)
                {
                    dataInFloats[i] = dataInFloat[i];
                }
            }

            // Don't bother if muted
            if (LocalState.VoiceState.SelfMute || LocalState.VoiceState.ServerMute)
            {
                AudioInSpec1 = 0;
                AudioInSpec2 = 0;
                AudioInSpec3 = 0;
                AudioInSpec4 = 0;
                AudioInSpec5 = 0;
                AudioInSpec6 = 0;
                AudioInSpec7 = 0;
                AudioInSpec8 = 0;
                AudioInSpec9 = 0;
                AudioInAverage = 0;
            }
            else
            {
                // Determine FFT data
                List<float[]> amplitudeData = FFT.Processing.HelperMethods.ProcessFrameOutput(frame);
                List<float[]> channelData = FFT.Processing.HelperMethods.GetFftData(FFT.Processing.HelperMethods.ConvertTo512(amplitudeData, ingraph), ingraph);

                float[] leftChannel = channelData[1];

                // Assign each FFT data out channel
                AudioInSpec1 = HelperMethods.Max(leftChannel, 0, 1);
                AudioInSpec2 = HelperMethods.Max(leftChannel, 2, 3);
                AudioInSpec3 = HelperMethods.Max(leftChannel, 3, 4);
                AudioInSpec4 = HelperMethods.Max(leftChannel, 4, 5);
                AudioInSpec5 = HelperMethods.Max(leftChannel, 5, 6);
                AudioInSpec6 = HelperMethods.Max(leftChannel, 7, 8);
                AudioInSpec7 = HelperMethods.Max(leftChannel, 9, 10);
                AudioInSpec8 = HelperMethods.Max(leftChannel, 10, 12);
                AudioInSpec9 = HelperMethods.Max(leftChannel, 14, 26);
                AudioInAverage = (AudioInSpec1 + AudioInSpec2 + AudioInSpec3 + AudioInSpec4 + AudioInSpec5 + AudioInSpec5 + AudioInSpec6 + AudioInSpec7 + AudioInSpec8 + AudioInSpec9) / 9;
            }

            InputRecieved?.Invoke(null, dataInFloats);
        }

        /// <summary>
        /// Handle new quantum for out graph
        /// </summary>
        private static void node_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;
            if (numSamplesNeeded != 0)
            {
                // Generate empty data if extra samples are needed
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);

                // Play recieved data
                frameInputNode.AddFrame(audioData);
            }
        }

        /// <summary>
        /// Update out device by id
        /// </summary>
        /// <param name="outID">New out device Id</param>
        public static async void UpdateOutputDeviceID(string outID)
        {
            if (outID != OutputDeviceID)
            {
                await CreateOutputDeviceNode(outID);
            }
        }

        /// <summary>
        /// Update in device by id
        /// </summary>
        /// <param name="inID">New in device Id</param>
        public static async void UpdateInputDeviceID(string inID)
        {
            if (inID != InputDeviceID)
            {
                await CreateInputDeviceNode(inID);
            }
        }

        /// <summary>
        /// Play sound effect by file
        /// </summary>
        /// <param name="file">File name</param>
        public static async void PlaySoundEffect(string file)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    MediaElement element = new MediaElement();
                    element.Volume = 2;
                    element.AudioCategory = AudioCategory.Other; //Alerts dims other sounds
                    element.SetPlaybackSource(Windows.Media.Core.MediaSource.CreateFromUri(
                        new Uri("ms-appx:///Assets/SoundEffects/" + file + ".wav")));
                    element.Play();
                });
        }
        
    }
}
