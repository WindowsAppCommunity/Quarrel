// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Voice.Audio.In;
using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace Quarrel.Services.Voice.Audio.In
{
    /// <summary>
    /// A <see langword="class"/> that manages incoming audio.
    /// </summary>
    public class AudioInService : IAudioInService
    {
        private AudioGraph _graph;
        private AudioFrameOutputNode _frameOutputNode;
        private int _quantum;
        private bool _isSpeaking = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioInService"/> class.
        /// </summary>
        [PreferredConstructor]
        public AudioInService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioInService"/> class with <paramref name="deviceId"/> for the input device.
        /// </summary>
        /// <param name="deviceId">The input device to use.</param>
        public AudioInService(string deviceId)
        {
            DeviceId = deviceId;
        }

        /// <inheritdoc/>
        public event EventHandler<float[]> AudioQueued;

        /// <inheritdoc/>
        public event EventHandler<int> SpeakingChanged;

        /// <inheritdoc/>
        public string DeviceId { get; private set; } // TODO: Public set

        /// <inheritdoc/>
        public int Samples => _graph.SamplesPerQuantum;

        /// <inheritdoc/>
        public bool Muted { get; private set; }

        /// <inheritdoc/>
        public async void CreateGraph(string deviceId = null)
        {
            // Get Default Settings
            var graphResult = await AudioGraph.CreateAsync(GetDefaultGraphSettings());
            if (graphResult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            _graph = graphResult.Graph;

            // Create frameOutputNode
            _frameOutputNode = _graph.CreateFrameOutputNode(_graph.EncodingProperties);

            _quantum = 0;
            _graph.QuantumStarted += Graph_QuantumStarted;

            // Get Device
            if (string.IsNullOrEmpty(deviceId) || deviceId == "Default")
            {
                deviceId = MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default);
                if (string.IsNullOrEmpty(deviceId))
                {
                    return;
                }
            }

            DeviceInformation selectedDevice = await DeviceInformation.CreateFromIdAsync(deviceId);

            CreateAudioDeviceInputNodeResult nodeResult = await _graph.CreateDeviceInputNodeAsync(MediaCategory.Communications, GetDefaultNodeSettings().EncodingProperties, selectedDevice);
            if (nodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // TODO: Handle no mic permission

                // Cannot create device input node
                _graph.Dispose();
                return;
            }

            // Connect Nodes
            nodeResult.DeviceInputNode.AddOutgoingConnection(_frameOutputNode);

            // Finalize
            DeviceId = deviceId;

            // Begin play
            _frameOutputNode.Start();
            _graph.Start();
        }

        /// <inheritdoc/>
        public void Mute()
        {
            Muted = true;
        }

        /// <inheritdoc/>
        public void Unmute()
        {
            Muted = false;
        }

        /// <inheritdoc/>
        public void ToggleMute()
        {
            Muted = !Muted;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _graph?.Dispose();
        }

        private void Graph_QuantumStarted(AudioGraph sender, object args)
        {
            // If odd quantum
            if (++_quantum % 2 == 0)
            {
                try
                {
                    // Record frame
                    AudioFrame frame = _frameOutputNode.GetFrame();
                    ProcessFrameOutput(frame);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        private unsafe void ProcessFrameOutput(AudioFrame frame)
        {
            if (Muted)
            {
                return;
            }

            float[] dataInFloats;
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                float* dataInFloat = (float*)dataInBytes;
                dataInFloats = new float[capacityInBytes / sizeof(float)];

                for (int i = 0; i < capacityInBytes / sizeof(float); i++)
                {
                    dataInFloats[i] = dataInFloat[i];
                }
            }

            double decibels = 0f;
            foreach (var sample in dataInFloats)
            {
                decibels += Math.Abs(sample);
            }

            decibels = 20 * Math.Log10(decibels / dataInFloats.Length);
            if (decibels < -40)
            {
                if (_isSpeaking)
                {
                    SpeakingChanged(this, 0);
                    _isSpeaking = false;
                }
            }
            else
            {
                if (!_isSpeaking)
                {
                    SpeakingChanged(this, 1);
                    _isSpeaking = true;
                }

                AudioQueued?.Invoke(null, dataInFloats);
            }
        }

        private AudioGraphSettings GetDefaultGraphSettings()
        {
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Communications);
            graphsettings.EncodingProperties = new AudioEncodingProperties();
            graphsettings.EncodingProperties.Subtype = "Float";
            graphsettings.EncodingProperties.SampleRate = 48000;
            graphsettings.EncodingProperties.ChannelCount = 2;
            graphsettings.EncodingProperties.BitsPerSample = 32;
            graphsettings.EncodingProperties.Bitrate = 3072000;
            return graphsettings;
        }

        private AudioGraphSettings GetDefaultNodeSettings()
        {
            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.Communications);
            nodesettings.EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32);
            nodesettings.DesiredSamplesPerQuantum = 960;
            nodesettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            return nodesettings;
        }
    }
}
