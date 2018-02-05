﻿using Discord_UWP.API;
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
        public static event EventHandler<float[]> InputRecieved;

        static AudioGraph graph;

        private static AudioDeviceInputNode deviceInputNode;
        private static AudioDeviceOutputNode deviceOutputNode;

        private static AudioFrameOutputNode frameOutputNode;
        private static AudioFrameInputNode frameInputNode;


        private static double theta = 0;
        private static bool ready = false;
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
        public static async Task CreateAudioGraph()
        {
            Console.WriteLine("Creating AudioGraph");
            // Create an AudioGraph with default settings
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Communications);
            //settings.EncodingProperties.SampleRate = 48000;
            //settings.EncodingProperties.ChannelCount = 2;
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            graph = result.Graph;

            // Create a device output node
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await graph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
            }

            deviceOutputNode = deviceOutputNodeResult.DeviceOutputNode;

            // Create the FrameInputNode at the same format as the graph, except explicitly set stereo.
            AudioEncodingProperties nodeEncodingProperties = graph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 2;
            frameInputNode = graph.CreateFrameInputNode(nodeEncodingProperties);
            //frameInputNode = await AudioManager.CreateDeviceInputNode();
            frameInputNode.AddOutgoingConnection(deviceOutputNode);
            await CreateDeviceInputNode();

            // Initialize the Frame Input Node in the stopped state
            frameInputNode.Start();

            // Hook up an event handler so we can start generating samples when needed
            // This event is triggered when the node is required to provide data
            // frameInputNode.QuantumStarted += node_QuantumStarted;

            // Start the graph since we will only start/stop the frame input node
            graph.Start();
            ready = true;
            
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
                int sampleRate = (int)graph.EncodingProperties.SampleRate;
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

        public static async Task CreateDeviceInputNode()
        {
            frameOutputNode = graph.CreateFrameOutputNode();
            graph.QuantumStarted += Graph_QuantumStarted;

            Windows.Devices.Enumeration.DeviceInformationCollection devices =
             await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Media.Devices.MediaDevice.GetAudioCaptureSelector());

            // Show UI to allow the user to select a device
            Windows.Devices.Enumeration.DeviceInformation selectedDevice = devices[0];

            CreateAudioDeviceInputNodeResult result =
                await graph.CreateDeviceInputNodeAsync(MediaCategory.Media, graph.EncodingProperties, selectedDevice);
            if (result.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                return;
            }
            
            
            deviceInputNode = result.DeviceInputNode;
            deviceInputNode.AddOutgoingConnection(frameOutputNode);
            frameOutputNode.Start();
        }

        private static void Graph_QuantumStarted(AudioGraph sender, object args)
        {
            AudioFrame frame = frameOutputNode.GetFrame();
            ProcessFrameOutput(frame);
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
