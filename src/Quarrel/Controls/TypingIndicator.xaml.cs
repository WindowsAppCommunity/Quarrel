// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    /// <summary>
    /// Shows three dots vary in opacity as a typing animation.
    /// </summary>
    public sealed partial class TypingIndicator : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypingIndicator"/> class.
        /// </summary>
        public TypingIndicator()
        {
            this.InitializeComponent();
            Typing.Begin();
        }
    }
}
