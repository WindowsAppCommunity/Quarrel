using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace Quarrel.Services.Voice.Audio
{

    public class AudioInService : IAudioService
    {
        #region Public Properties

        // TODO: Public set
        public string DeviceId { get; private set; }

        #endregion

        #region Variables

        private AudioGraph _Graph;
        private AudioFrameOutputNode _FrameOutputNode;
        private int _Quantum;

        #endregion

        #region Events

        public event EventHandler<float[]> InputRecieved;

        #endregion

        #region Constructors

        public AudioInService(string deviceId = null)
        {
            CreateGraph(deviceId);
        }

        #endregion

        #region Helper Methods

        private async void CreateGraph(string deviceId = null)
        {
            // Get Default Settings
            var graphResult = await AudioGraph.CreateAsync(GetDefaultGraphSettings());
            if (graphResult.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }
            _Graph = graphResult.Graph;

            // Create frameOutputNode
            _FrameOutputNode = _Graph.CreateFrameOutputNode(_Graph.EncodingProperties);

            _Quantum = 0;
            _Graph.QuantumStarted += _Graph_QuantumStarted;

            // Get Device
            if (string.IsNullOrEmpty(deviceId) || deviceId == "Default")
            {
                deviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            }
            DeviceInformation selectedDevice = await DeviceInformation.CreateFromIdAsync(deviceId);

            CreateAudioDeviceInputNodeResult nodeResult = await _Graph.CreateDeviceInputNodeAsync(MediaCategory.Communications, GetDefaultNodeSettings().EncodingProperties, selectedDevice);
            if (nodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device input node
                _Graph.Dispose();
                return;
            }

            // Connect Nodes
            nodeResult.DeviceInputNode.AddOutgoingConnection(_FrameOutputNode);

            // Finalize
            DeviceId = deviceId;

            // Begin play
            _FrameOutputNode.Start();
            _Graph.Start();
        }

        private void _Graph_QuantumStarted(AudioGraph sender, object args)
        {
            throw new NotImplementedException();
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

        #endregion

        #region Dependencies
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }
        #endregion
    }
}
