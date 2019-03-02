using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Concentus.Structs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoundTrip : Page
    {
        public RoundTrip()
        {
            this.InitializeComponent();
        }

        private OpusEncoder encoder = new OpusEncoder(48000, 2, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP);
        private OpusDecoder decoder = new OpusDecoder(48000, 2);

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AudioManager.CreateAudioGraphs();
            
            if ((sender as ToggleButton).IsChecked == true)
            {
                AudioManager.InputRecieved += RTProcess;
            } else
            {
                AudioManager.InputRecieved -= RTProcess;
            }
        }

        private void RTProcess(object sender, float[] e)
        {
            try
            {
                byte[] nonce = new byte[] { 128, 120, 192, 46, 6, 144, 172, 128, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; //Length of 24
                byte[] key = new byte[] { 40, 221, 122, 207, 253, 63, 24, 97, 28, 168, 80, 250, 98, 165, 166, 32, 161, 61, 248, 51, 84, 26, 171, 14, 139, 17, 174, 121, 9, 74, 181, 33 }; //Length of 32

                byte[] opus = new byte[1820 * sizeof(float) + 16];
                int encodedSize = encoder.Encode(e, 0, 960, opus, 0, 1820 * sizeof(float));
                RuntimeComponent.Cypher.encrypt(opus, 0, encodedSize, opus, 0, nonce, key);

                RuntimeComponent.Cypher.decrypt(opus, 0, encodedSize+16, opus, 0, nonce, key);

                float[] frame = new float[e.Length];
                int samples = decoder.Decode(opus, 0, encodedSize, frame, 0, (20 * 48 * 2));
                AudioManager.AddFrame(frame, (uint)samples);
            }
            catch (Exception exc)
            {

            }
        }
    }
}
