using DiscordAPI.Models;
using Quarrel.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages.Embeds
{
    public sealed partial class DefaultEmbedTemplate : UserControl
    {
        public DefaultEmbedTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                try
                {
                    this.Bindings.Update();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }                
            };
        }

        public Embed ViewModel => DataContext as Embed;

        private void ShareEmbed(object sender, RoutedEventArgs e)
        {
            // TODO: 
        }
    }
}
