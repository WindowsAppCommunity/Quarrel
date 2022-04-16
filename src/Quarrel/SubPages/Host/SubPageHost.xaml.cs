// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Navigation.SubPages;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Host
{
    public sealed partial class SubPageHost : UserControl
    {
        private readonly IMessenger _messenger;

        private DependencyProperty ContentViewModelProperty =
            DependencyProperty.Register(nameof(ContentViewModel), typeof(object), typeof(SubPageHost), new PropertyMetadata(null, ContentViewModelUpdated));

        public SubPageHost()
        {
            this.InitializeComponent();
            _messenger = App.Current.Services.GetRequiredService<IMessenger>();

            this.Visibility = Visibility.Collapsed;
            _messenger.Register<NavigateToSubPageMessage>(this, (s, e) => NavigateToSubPage(e.TargetViewModelType));
            _messenger.Register<CloseSubPageMessage>(this, (s, e) => ContentViewModel = null);
        }

        public object? ContentViewModel
        {
            get => GetValue(ContentViewModelProperty);
            set => SetValue(ContentViewModelProperty, value);
        }

        private void NavigateToSubPage(Type viewModelType)
        {
            object viewModel = App.Current.Services.GetRequiredService(viewModelType);
            ContentViewModel = viewModel;
        }

        private static void ContentViewModelUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SubPageHost @this = (SubPageHost)sender; 

            if (args.NewValue is null)
            {
                @this.Visibility = Visibility.Collapsed;
            }
            else
            {
                @this.Visibility = Visibility.Visible;
            }
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            _messenger.Send(new CloseSubPageMessage());
        }
    }
}
