using DiscordAPI.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    /// <summary>
    /// Control to display a single Game.
    /// </summary>
    public sealed partial class RichPresenceControl : UserControl
    {
        private DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>
        /// Initializes a new instance of the <see cref="RichPresenceControl"/> class.
        /// </summary>
        public RichPresenceControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if ((e.NewValue is Game game) && game.TimeStamps != null)
                {
                    _timer.Interval = new TimeSpan(10000);
                    _timer.Tick += Timer_Tick;
                    _timer.Start();
                }
                else
                {
                    _timer.Tick -= Timer_Tick;
                    _timer.Stop();
                }

                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the game in view.
        /// </summary>
        public Game ViewModel => DataContext as Game;

        private void Timer_Tick(object sender, object e)
        {
            this.Bindings.Update();
        }
    }
}
