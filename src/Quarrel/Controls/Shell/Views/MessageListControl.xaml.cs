using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle MessageList and Message Drafting
    /// </summary>
    public sealed partial class MessageListControl : UserControl
    {
        public MessageListControl()
        {
            this.InitializeComponent();

            ViewModel.ScrollTo += ViewModel_ScrollTo;
            Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
            {
                _ItemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
            });
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        private ItemsStackPanel _ItemsStackPanel;
        private ScrollViewer _MessageScrollViewer;
        
        /// <summary>
        /// Finds ItemStackPanel and MessageScroller from MessageList once loaded
        /// </summary>
        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _MessageScrollViewer = MessageList.FindChild<ScrollViewer>();
            _ItemsStackPanel = (sender as ItemsStackPanel);
            if (_MessageScrollViewer != null) _MessageScrollViewer.ViewChanged += _messageScrollViewer_ViewChanged;
        }

        /// <summary>
        /// Checks margins from end of view when messages are scrolled
        /// </summary>
        private void _messageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.CurrentChannel == null)
                return;

            if (ViewModel.ItemsLoading)
                return;

            if (MessageList.Items.Count > 0)
            {
                // Distance from top
                double fromTop = _MessageScrollViewer.VerticalOffset;

                //Distance from bottom
                double fromBottom = _MessageScrollViewer.ScrollableHeight - fromTop;

                // Load messages
                if (fromTop < 100)
                    ViewModel.LoadOlderMessages();

                // All messages seen, mark as read
                if (fromBottom < 10)
                    ViewModel.CurrentChannel.MarkAsRead.Execute(null);
            }
        }

        /// <summary>
        /// Scrolls <paramref name="e"/> into view
        /// </summary>
        private void ViewModel_ScrollTo(object sender, BindableMessage e)
        {
            MessageList.ScrollIntoView(e);
        }
    }
}
