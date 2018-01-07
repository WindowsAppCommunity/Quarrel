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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (oog.Text == "")
            {
                byte[] iogAsByteArray = Encoding.ASCII.GetBytes(iog.Text);
                byte[] iuwpAsByteArray = Encoding.ASCII.GetBytes(iuwp.Text);
                byte[] oogAsByteArray = new byte[iogAsByteArray.Length];
                byte[] ouwpAsByteArray = new byte[iuwpAsByteArray.Length];
                //Discord_UWP.Voice.Cypher.process(iuwpAsByteArray, 0, iuwpAsByteArray.Length, ouwpAsByteArray, 0, , );
                //
                oog.Text = Encoding.ASCII.GetString(iogAsByteArray);
                ouwp.Text = Encoding.ASCII.GetString(iuwpAsByteArray);
            } else
            {
                byte[] iogAsByteArray = Encoding.ASCII.GetBytes(iog.Text);
                byte[] iuwpAsByteArray = Encoding.ASCII.GetBytes(iuwp.Text);
                byte[] oogAsByteArray = new byte[iogAsByteArray.Length];
                byte[] ouwpAsByteArray = new byte[iuwpAsByteArray.Length];
                //Discord_UWP.Voice.Cypher.process(iuwpAsByteArray, 0, iuwpAsByteArray.Length, ouwpAsByteArray, 0, , );

                oog.Text = Encoding.ASCII.GetString(iogAsByteArray);
                ouwp.Text = Encoding.ASCII.GetString(iuwpAsByteArray);
            }
        }
    }
}
