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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                AudioManager.InputRecieved -= RTProcess;
            } else
            {
                AudioManager.InputRecieved += RTProcess;
            }
        }


        const int TBD = 0;
        private void RTProcess(object sender, float[] e)
        {
            byte[] nonce = new byte[] { /*tbd */}; //Length of 24
            byte[] key = new byte[] { /*tbd */}; //Length of 32

            byte[] opus = new byte[TBD];
            int encodedSize = encoder.Encode(e, 0, (48000 / 1000 * 20), opus, 0, (48000 / 1000 * 20 * sizeof(float) * 2));
            RuntimeComponent.Cypher.encrypt(opus, 0, 0, opus, 0, nonce, key);

            RuntimeComponent.Cypher.decrypt(opus, 0, 0, opus, 0, nonce, key);

            float[] frame = new float[e.Length];
            int samples = decoder.Decode(opus, 0, opus.Length, frame, 0, (20 * 48 * 2));
            AudioManager.AddFrame(frame, (uint)samples);
        }
    }
}
