// Adam Dernis © 2022

using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            QuarrelIcon.BeginAnimation();
        }
    }
}
