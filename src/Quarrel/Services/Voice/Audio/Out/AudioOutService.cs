using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace Quarrel.Services.Voice.Audio
{

    public class AudioOutService : IAudioService
    {
        #region Public Properties

        // TODO: Public set
        public string DeviceId { get; private set; }

        #endregion

        #region Variables

        private AudioGraph _Graph;
        private AudioFrameInputNode _FrameInputNode;

        #endregion

        #region Constructors

        public AudioOutService(string deviceId = null)
        {
            CreateGraph(deviceId);
        }

        #endregion

        #region Helper Methods

        private async void CreateGraph(string deviceId = null)
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
            _Graph = graphResult.Graph;

            // Create Nodes
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await _Graph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                _Graph.Dispose();
                return;
            }

            // Connect Nodes
            _FrameInputNode = _Graph.CreateFrameInputNode(_Graph.EncodingProperties);
            _FrameInputNode.AddOutgoingConnection(deviceOutputNodeResult.DeviceOutputNode);

            // Finalize
            DeviceId = deviceId;

            // Begin play
            _FrameInputNode.Start();
            _Graph.Start();
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
            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat);
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
