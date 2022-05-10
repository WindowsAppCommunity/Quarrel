// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Messages
{
    public sealed partial class MessagePanel : UserControl
    {
        private const double ScrollPadding = 100;

        public MessagePanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<MessagesViewModel>();
        }

        public MessagesViewModel ViewModel => (MessagesViewModel)DataContext;

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView messageList = (ListView)sender;
            var scrollViewer = messageList.FindDescendant<ScrollViewer>();
            scrollViewer.ViewChanged += OnViewChanged;
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var viewer = (ScrollViewer)sender;
            if (viewer.VerticalOffset <= ScrollPadding)
            {
                if (!ViewModel.IsLoading)
                {
                    ViewModel.LoadOlderMessages();
                }
            }
            else if (viewer.VerticalOffset >= viewer.ScrollableHeight - ScrollPadding)
            {
                // TODO: Scrolled to bottom
            }
        }
    }
}
