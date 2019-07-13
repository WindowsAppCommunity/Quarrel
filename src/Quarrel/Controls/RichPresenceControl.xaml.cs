using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

namespace Quarrel.Controls
{
    public sealed partial class RichPresenceControl : UserControl
    {
        public RichPresenceControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if ((e.NewValue is Game game) && game.TimeStamps != null)
                {
                    Timer.Interval = new TimeSpan(10000);
                    Timer.Tick += Timer_Tick;
                    Timer.Start();
                }else
                {
                    Timer.Tick -= Timer_Tick;
                    Timer.Stop();
                }

                this.Bindings.Update();
            };
        }

        private void Timer_Tick(object sender, object e)
        {
            this.Bindings.Update();
        }

        DispatcherTimer Timer = new DispatcherTimer();

        public Game ViewModel => DataContext as Game;
    }
}
