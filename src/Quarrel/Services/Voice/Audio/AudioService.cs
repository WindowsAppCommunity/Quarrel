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
    public enum AudioType { In, Out }

    public class AudioService : IAudioService
    {
        #region Public Properties

        // TODO: Public set
        public string DeviceId { get; private set; }

        #endregion

        #region Variables

        private AudioGraph _Graph;

        #endregion

        #region Events

        public event EventHandler<float[]> InputRecieved;

        #endregion

        #region Constructors

        public AudioService(AudioType type, string deviceId = null)
        {

        }

        #endregion

        #region Helper Methods

        private async void CreateAsOutGraph(string deviceId = null)
        {
            // Get Default Settings
            var graphSettings = GetDefaultGraphSetting();

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
            AudioFrameInputNode frameInputNode = _Graph.CreateFrameInputNode(_Graph.EncodingProperties);
            frameInputNode.AddOutgoingConnection(deviceOutputNodeResult.DeviceOutputNode);

            // Finalize
            DeviceId = deviceId;

            // Begin play
            frameInputNode.Start();
            _Graph.Start();
        }

        private AudioGraphSettings GetDefaultGraphSetting()
        {
            AudioGraphSettings graphsettings = new AudioGraphSettings(AudioRenderCategory.Media);
            graphsettings.EncodingProperties = new AudioEncodingProperties();
            graphsettings.EncodingProperties.Subtype = "Float";
            graphsettings.EncodingProperties.SampleRate = 48000;
            graphsettings.EncodingProperties.ChannelCount = 2;
            graphsettings.EncodingProperties.BitsPerSample = 32;
            graphsettings.EncodingProperties.Bitrate = 3072000;
            return graphsettings;
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
