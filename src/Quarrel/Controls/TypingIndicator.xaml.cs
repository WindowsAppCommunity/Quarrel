// Quarrel © 2022

using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
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
