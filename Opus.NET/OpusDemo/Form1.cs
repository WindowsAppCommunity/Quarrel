using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using FragLabs.Audio.Codecs;

namespace OpusDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                comboBox1.Items.Add(WaveIn.GetCapabilities(i).ProductName);
            }
            if (WaveIn.DeviceCount > 0)
                comboBox1.SelectedIndex = 0;
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                comboBox2.Items.Add(WaveOut.GetCapabilities(i).ProductName);
            }
            if (WaveOut.DeviceCount > 0)
                comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;
            StartEncoding();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            StopEncoding();
        }

        WaveIn _waveIn;
        WaveOut _waveOut;
        BufferedWaveProvider _playBuffer;
        OpusEncoder _encoder;
        OpusDecoder _decoder;
        int _segmentFrames;
        int _bytesPerSegment;
        ulong _bytesSent;
        DateTime _startTime;
        Timer _timer = null;

        void StartEncoding()
        {
            _startTime = DateTime.Now;
            _bytesSent = 0;
            _segmentFrames = 960;
            _encoder = OpusEncoder.Create(48000, 1, FragLabs.Audio.Codecs.Opus.Application.Voip);
            _encoder.Bitrate = 8192;
            _decoder = OpusDecoder.Create(48000, 1);
            _bytesPerSegment = _encoder.FrameByteCount(_segmentFrames);

            _waveIn = new WaveIn(WaveCallbackInfo.FunctionCallback());
            _waveIn.BufferMilliseconds = 50;
            _waveIn.DeviceNumber = comboBox1.SelectedIndex;
            _waveIn.DataAvailable += _waveIn_DataAvailable;
            _waveIn.WaveFormat = new WaveFormat(48000, 16, 1);

            _playBuffer = new BufferedWaveProvider(new WaveFormat(48000, 16, 1));

            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            _waveOut.DeviceNumber = comboBox2.SelectedIndex;
            _waveOut.Init(_playBuffer);

            _waveOut.Play();
            _waveIn.StartRecording();

            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = 1000;
                _timer.Tick += _timer_Tick;
            }
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            var timeDiff = DateTime.Now - _startTime;
            var bytesPerSecond = _bytesSent / timeDiff.TotalSeconds;
            Console.WriteLine("{0} Bps", bytesPerSecond);
        }

        byte[] _notEncodedBuffer = new byte[0];
        void _waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] soundBuffer = new byte[e.BytesRecorded + _notEncodedBuffer.Length];
            for (int i = 0; i < _notEncodedBuffer.Length; i++)
                soundBuffer[i] = _notEncodedBuffer[i];
            for (int i = 0; i < e.BytesRecorded; i++)
                soundBuffer[i + _notEncodedBuffer.Length] = e.Buffer[i];

            int byteCap = _bytesPerSegment;
            int segmentCount = (int)Math.Floor((decimal)soundBuffer.Length / byteCap);
            int segmentsEnd = segmentCount * byteCap;
            int notEncodedCount = soundBuffer.Length - segmentsEnd;
            _notEncodedBuffer = new byte[notEncodedCount];
            for (int i = 0; i < notEncodedCount; i++)
            {
                _notEncodedBuffer[i] = soundBuffer[segmentsEnd + i];
            }

            for (int i = 0; i < segmentCount; i++)
            {
                byte[] segment = new byte[byteCap];
                for (int j = 0; j < segment.Length; j++)
                    segment[j] = soundBuffer[(i*byteCap) + j];
                int len;
                byte[] buff = _encoder.Encode(segment, segment.Length, out len);
                _bytesSent += (ulong)len;
                buff = _decoder.Decode(buff, len, out len);
                _playBuffer.AddSamples(buff, 0, len);
            }
        }

        void StopEncoding()
        {
            _timer.Stop();
            _waveIn.StopRecording();
            _waveIn.Dispose();
            _waveIn = null;
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
            _playBuffer = null;
            _encoder.Dispose();
            _encoder = null;
            _decoder.Dispose();
            _decoder = null;
        }
    }
}
