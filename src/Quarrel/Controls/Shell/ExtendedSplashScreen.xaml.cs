// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Controls.Shell.Enums;
using Quarrel.Services.Localization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        private const string LoadingString = "ExtendedSplash/Loading";
        private const string ConnectedString = "ExtendedSplash/Connected";
        private const string ConnectingString = "ExtendedSplash/Connecting";

        private static readonly DependencyProperty IsShowingProperty =
            DependencyProperty.Register(nameof(IsShowing), typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(false, OnIsShowingChanged));

        private static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(SplashStatus), typeof(ExtendedSplashScreen), new PropertyMetadata(false, OnStatusChanged));

        private readonly ILocalizationService _localizationService;

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

        public SplashStatus Status
        {
            get => (SplashStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private void BeginAnimation()
        {
            this.Visibility = Visibility.Visible;
            StatusBlock.Text = _localizationService[ConnectingString];
            QuarrelIcon.BeginAnimation();
        }

        private void FinishAnimation()
        {
            StatusBlock.Text = _localizationService[ConnectedString];
            QuarrelIcon.FinishAnimation();
            HideAnimation.Begin();
        }

        private static void OnIsShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ExtendedSplashScreen splash = (ExtendedSplashScreen)d;
            bool newValue = (bool)args.NewValue;
            bool oldValue = (bool)args.OldValue;

            if (newValue != oldValue)
            {
                if (splash.IsShowing)
                {
                    splash.BeginAnimation();
                }
                else
                {
                    splash.FinishAnimation();
                }
            }
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ExtendedSplashScreen splash = (ExtendedSplashScreen)d;
            SplashStatus status = (SplashStatus)args.NewValue;
            string messageString = status switch
            {
                SplashStatus.Connecting => ConnectingString,
                SplashStatus.Connected => ConnectedString,
                _ => LoadingString,
            };
            string messageText = splash._localizationService[messageString];
            splash.StatusBlock.Text = messageText;
        }

        private void HideAnimation_Completed(object sender, object e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
