using DiscordAPI.API;
using DiscordAPI.API.User;
using DiscordAPI.API.User.Models;
using Quarrel.Authentication;
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
using DiscordAPI.API.Gateway.DownstreamEvents;
using DiscordAPI.API.Gateway;
using DiscordAPI.SharedModels;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace Tester
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
        public static event EventHandler<float[]> InputRecieved;
        
        static AudioGraph ingraph;
        static AudioGraph outgraph;

        private static AudioDeviceInputNode deviceInputNode;
        private static AudioDeviceOutputNode deviceOutputNode;

        private static AudioFrameOutputNode frameOutputNode;
        private static AudioFrameInputNode frameInputNode;

        private static int quantum = 0;
        private static double theta = 0;
        private static bool ready = false;
        //public static float AudioSpec1 = 0;
        //public static float AudioSpec2 = 0;
        //public static float AudioSpec3 = 0;
        //public static float AudioSpec4 = 0;
        //public static float AudioSpec5 = 0;
        //public static float AudioSpec6 = 0;
        //public static float AudioSpec7 = 0;
        //public static float AudioSpec8 = 0;
        //public static float AudioSpec9 = 0;


        //private static bool started = false;
        //public static 

        public static void ChangeVolume(double value)
        {
            if(ready)
                deviceOutputNode.OutgoingGain = value;
        }
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
        public static async Task CreateAudioGraphs()
        {
            await CreateDeviceOutputNode();
            await CreateDeviceInputNode();
        }

        public static async Task CreateDeviceOutputNode()
        {
            Console.WriteLine("Creating AudioGraphs");
            // Create an AudioGraph with default settings
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
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
            frameInputNode.Start();
            ready = true;
            outgraph.Start();
        }

        public static async Task CreateDeviceInputNode()
        {
            Console.WriteLine("Creating AudioGraphs");
            // Create an AudioGraph with default settings
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
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
            nodesettings.EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 16);
            nodesettings.DesiredSamplesPerQuantum = 960;
            nodesettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            frameOutputNode = ingraph.CreateFrameOutputNode(outgraph.EncodingProperties);
            quantum = 0;
            ingraph.QuantumStarted += Graph_QuantumStarted;

            Windows.Devices.Enumeration.DeviceInformation selectedDevice =
             await Windows.Devices.Enumeration.DeviceInformation.CreateFromIdAsync(Windows.Media.Devices.MediaDevice.GetDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Default));

            CreateAudioDeviceInputNodeResult result =
                await ingraph.CreateDeviceInputNodeAsync(MediaCategory.Media, nodesettings.EncodingProperties, selectedDevice);
            if (result.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                return;
            }

            deviceInputNode = result.DeviceInputNode;
            deviceInputNode.AddOutgoingConnection(frameOutputNode);
            frameOutputNode.Start();
            ingraph.Start();
        }

        unsafe public static void AddFrame(float[] framedata, uint samples)
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
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);
                // Cast to float since the data we are generating is float
                float* dataInFloat = (float *)dataInBytes;
                fixed (float* frames = framedata)
                {
                    for (int i = 0; i< samples*2; i++)
                    {
                        dataInFloat[i] = frames[i];
                    }
                }
            }
            //List<float[]> amplitudeData = FFT.Processing.HelperMethods.ProcessFrameOutput(frame);
            //List<float[]> channelData = FFT.Processing.HelperMethods.GetFftData(FFT.Processing.HelperMethods.ConvertTo512(amplitudeData, outgraph), outgraph);

            //float[] leftChannel = channelData[1];

            //AudioSpec1 = HelperMethods.Max(leftChannel, 0, 1);
            //AudioSpec2 = HelperMethods.Max(leftChannel, 2, 3);
            //AudioSpec3 = HelperMethods.Max(leftChannel, 3, 4);
            //AudioSpec4 = HelperMethods.Max(leftChannel, 4, 5);
            //AudioSpec5 = HelperMethods.Max(leftChannel, 5, 6);
            //AudioSpec6 = HelperMethods.Max(leftChannel, 7, 8);
            //AudioSpec7 = HelperMethods.Max(leftChannel, 9, 10);
            //AudioSpec8 = HelperMethods.Max(leftChannel, 10, 12);
            //AudioSpec9 = HelperMethods.Max(leftChannel, 14, 26);
            frameInputNode.AddFrame(frame);
        }

        unsafe static public AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample) * (number of channels)
            uint bufferSize = samples * sizeof(float) * 2;
            AudioFrame frame = new AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInFloat = (float*)dataInBytes;

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

        unsafe private static void ProcessFrameOutput(AudioFrame frame)
        {
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                dataInFloat = (float*)dataInBytes;
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
    }
}
