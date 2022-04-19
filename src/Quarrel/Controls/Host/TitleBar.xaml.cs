// Quarrel © 2022

using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Host
{
    public sealed partial class TitleBar : UserControl
    {
        private DependencyProperty TitleContentProperty =
            DependencyProperty.Register(nameof(TitleContent), typeof(string), typeof(TitleBar), new PropertyMetadata(null, TitleContentPropertyUpdated));

        public TitleBar()
        {
            this.InitializeComponent();
            SetupTitleBar();
        }
        
        private void SetupTitleBar()
        {
            var appView = ApplicationView.GetForCurrentView();
            TitleContent = appView.Title;

            var titlebar = appView.TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Window.Current.SetTitleBar(this);
        }

        private string? TitleContent
        {
            get => (string?)GetValue(TitleContentProperty);
            set => SetValue(TitleContentProperty, value);
        }

        private static void TitleContentPropertyUpdated(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TitleBar titleBar = (TitleBar)d;
            titleBar.TitleTextBlock.Text = (string)args.NewValue;
        }
    }
}
