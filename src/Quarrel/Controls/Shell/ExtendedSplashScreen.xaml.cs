// Adam Dernis © 2022

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        private static readonly DependencyProperty IsShowingProperty =
            DependencyProperty.Register(nameof(IsShowing), typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(false, OnIsShowingChanged));

        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        public bool IsShowing
        {
            get => (bool)GetValue(IsShowingProperty);
            set => SetValue(IsShowingProperty, value);
        }

        private void BeginAnimation()
        {
            this.Visibility = Visibility.Visible;
            QuarrelIcon.BeginAnimation();
        }

        private void FinishAnimation()
        {
            QuarrelIcon.FinishAnimation();
        }

        private void QuarrelIcon_AnimationFinished(object sender, System.EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private static void OnIsShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ExtendedSplashScreen extendedSplashScreen = (ExtendedSplashScreen)d;
            bool newValue = (bool)args.NewValue;
            bool oldValue = (bool)args.OldValue;

            if (newValue != oldValue)
            {
                if (extendedSplashScreen.IsShowing)
                {
                    extendedSplashScreen.BeginAnimation();
                }
                else
                {
                    extendedSplashScreen.FinishAnimation();
                }
            }
        }
    }
}
