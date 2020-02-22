// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Voice.Audio.Out;
using System;
using System.Runtime.InteropServices;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace Quarrel.Services.Voice.Audio.Out
{
    /// <summary>
    /// A <see langword="class"/> that manages incoming audio.
    /// </summary>
    public class AudioOutService : IAudioOutService
    {
        private AudioGraph _graph;
        private AudioFrameInputNode _frameInputNode;
        private bool _ready = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioOutService"/> class.
        /// </summary>
        [PreferredConstructor]
        public AudioOutService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioOutService"/> class with <paramref name="deviceId"/> as the output device.
        /// </summary>
        /// <param name="deviceId">The Id of the output device.</param>
        public AudioOutService(string deviceId = null)
        {
            DeviceId = deviceId;
        }

        /// <summary>
        /// Invoked when audio is queued for playing.
        /// </summary>
        public event EventHandler<float[]> AudioQueued;

        /// <inheritdoc/>
        public string DeviceId { get; private set; }// TODO: Public set

        /// <inheritdoc/>
        public int Samples => _graph.SamplesPerQuantum;

        /// <inheritdoc/>
        public bool Deafened { get; private set; }

        /// <inheritdoc/>
        public async void CreateGraph(string deviceId = null)
        {
            // Get Default Settings
            var graphSettings = GetDefaultGraphSettings();

            // Get Device
            if (string.IsNullOrEmpty(deviceId) || deviceId == "Default")
            {
                deviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            }

            DeviceInformation selectedDevice = await DeviceInformation.CreateFromIdAsync(deviceId);
            graphSettings.PrimaryRenderDevice = selectedDevice;

            // Create Graph
            CreateAudioGraphResult graphResult = await AudioGraph.CreateAsync(graphSettings);
            if (graphResult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            _graph = graphResult.Graph;

            // Create Nodes
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await _graph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                _graph.Dispose();
                return;
            }

            // Connect Nodes
            _frameInputNode = _graph.CreateFrameInputNode(_graph.EncodingProperties);
            _frameInputNode.AddOutgoingConnection(deviceOutputNodeResult.DeviceOutputNode);

            // Finalize
            DeviceId = deviceId;

            // Begin play
            _frameInputNode.Start();
            _ready = true;
            _graph.Start();
        }

        /// <inheritdoc/>
        public unsafe void AddFrame(float[] framedata, uint samples)
        {
            if (!_ready || Deafened)
            {
                return;
            }

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

            AudioQueued?.Invoke(this, framedata);

            // Add frame to queue
            _frameInputNode.AddFrame(frame);
        }

        /// <inheritdoc/>
        public void Deafen()
        {
            Deafened = true;
        }

        /// <inheritdoc/>
        public void Undeafen()
        {
            Deafened = false;
        }

        /// <inheritdoc/>
        public void ToggleDeafen()
        {
            Deafened = !Deafened;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _ready = false;
            _graph.Dispose();
        }

        private AudioGraphSettings GetDefaultGraphSettings()
        {
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Communications)
            {
                EncodingProperties = new AudioEncodingProperties
                {
                    Subtype = "Float",
                    SampleRate = 48000,
                    ChannelCount = 2,
                    BitsPerSample = 32,
                    Bitrate = 64000,
                },
            };
            return graphsettings;
        }

        private AudioGraphSettings GetDefaultNodeSettings()
        {
            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat)
            {
                EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32),
                DesiredSamplesPerQuantum = 960,
                QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired,
            };
            return nodesettings;
        }
    }
}
