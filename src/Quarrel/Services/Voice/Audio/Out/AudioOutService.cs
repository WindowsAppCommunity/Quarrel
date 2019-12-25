using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using GalaSoft.MvvmLight.Ioc;

namespace Quarrel.Services.Voice.Audio.Out
{

    public class AudioOutService : IAudioOutService
    {
        #region Public Properties

        // TODO: Public set
        public string DeviceId { get; private set; }

        #endregion

        #region Variables

        private AudioGraph _Graph;
        private AudioFrameInputNode _FrameInputNode;
        private bool _Ready = false;

        #endregion

        #region Constructors

        [PreferredConstructor]
        public AudioOutService()
        {

        }
        public AudioOutService(string deviceId = null)
        {

        }

        #endregion

        #region Methods

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
            _Ready = true;
            _Graph.Start();
        }

        public unsafe void AddFrame(float[] framedata, uint samples)
        {
            if (!_Ready)
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

            // TODO: FFT
            
            // Add frame to queue
            _FrameInputNode.AddFrame(frame);
        }

        public void Dispose()
        {
            _Ready = false;
            _Graph.Dispose();
        }
        #endregion

        #region Helper Methods

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
                    Bitrate = 64000
                }
            };
            return graphsettings;
        }

        private AudioGraphSettings GetDefaultNodeSettings()
        {
            AudioGraphSettings nodesettings = new AudioGraphSettings(AudioRenderCategory.GameChat)
            {
                EncodingProperties = AudioEncodingProperties.CreatePcm(48000, 2, 32),
                DesiredSamplesPerQuantum = 960,
                QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired
            };
            return nodesettings;
        }

        #endregion
    }

    #region Dependencies

    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
    #endregion
}
