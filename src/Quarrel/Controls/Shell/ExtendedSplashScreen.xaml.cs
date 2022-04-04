// Adam Dernis © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.Localization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        private const string Connected = "ExtendedSplash/Connected";
        private const string Connecting = "ExtendedSplash/Connecting";

        private static readonly DependencyProperty IsShowingProperty =
            DependencyProperty.Register(nameof(IsShowing), typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(false, OnIsShowingChanged));

        private ILocalizationService _localizationService;

        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            _localizationService = App.Current.Services.GetRequiredService<ILocalizationService>();
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
            StatusBlock.Text = _localizationService[Connecting];
            QuarrelIcon.BeginAnimation();
        }

        private void FinishAnimation()
        {
            StatusBlock.Text = _localizationService[Connected];
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
