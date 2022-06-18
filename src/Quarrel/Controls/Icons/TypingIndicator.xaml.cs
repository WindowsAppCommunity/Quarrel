// Quarrel © 2022

using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Icons
{
    public sealed partial class TypingIndicator : UserControl
    {
        public TypingIndicator()
        {
            this.InitializeComponent();
            Typing.Begin();
        }
    }
}
