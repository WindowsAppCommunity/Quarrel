// Copyright (c) Quarrel. All rights reserved.

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
        /// <summary>
        /// Initializes a new instance of the <see cref="RichPresenceControl"/> class.
        /// </summary>
        public RichPresenceControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the game in view.
        /// </summary>
        public Game ViewModel => DataContext as Game;
    }
}
