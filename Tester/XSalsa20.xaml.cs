using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using RuntimeComponent;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XSalsa20 : Page
    {
        public XSalsa20()
        {
            this.InitializeComponent();
        }
        private bool encrypted = false;
        private byte[] data;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (encrypted)
            {
                //byte[] iogAsByteArray = Encoding.ASCII.GetBytes(iog.Text);
                //byte[] iuwpAsByteArray = Encoding.ASCII.GetBytes(iuwp.Text);
                //byte[] oogAsByteArray = new byte[iogAsByteArray.Length];
                //byte[] ouwpAsByteArray = new byte[iuwpAsByteArray.Length];
                byte[] testo = new byte[data.Length-16];
                Cypher.decrypt(data, 0, data.Length, testo, 0, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 255, 165, 107, 19, 208, 79, 17, 42, 205, 232, 65, 32, 26, 84, 141, 70, 246, 33, 116, 212, 245, 35, 131, 172, 109, 58, 165, 231, 205, 15, 34, 191 });
                data = testo;
                encrypted = false;
            }
            else
            {
                //byte[] iogAsByteArray = Encoding.ASCII.GetBytes(iog.Text);
                //byte[] iuwpAsByteArray = Encoding.ASCII.GetBytes(iuwp.Text);
                //byte[] oogAsByteArray = new byte[iogAsByteArray.Length];
                //byte[] ouwpAsByteArray = new byte[iuwpAsByteArray.Length];
                byte[] testi = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35};
                data = new byte[testi.Length+16];
                Cypher.encrypt(testi, 0, testi.Length, data, 0, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 255, 165, 107, 19, 208, 79, 17, 42, 205, 232, 65, 32, 26, 84, 141, 70, 246, 33, 116, 212, 245, 35, 131, 172, 109, 58, 165, 231, 205, 15, 34, 191 });
                encrypted = true;
            }
        }
    }
}
