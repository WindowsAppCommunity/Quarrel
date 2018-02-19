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
            //TODO: Call Opus lib
            RuntimeComponent.Cypher.encrypt(opus, 0, 0, opus, 0, nonce, key);
            RuntimeComponent.Cypher.decrypt(opus, 0, 0, opus, 0, nonce, key);

            float[] frame = new float[e.Length];
            uint samples = 0; //TODO: Call Opus lib
            AudioManager.AddFrame(frame, samples);
        }
    }
}
