// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle MessageList and Message Drafting.
    /// </summary>
    public sealed partial class MessageListControl : UserControl
    {
        private ItemsStackPanel _itemsStackPanel;
        private ScrollViewer _messageScrollViewer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListControl"/> class.
        /// </summary>
        public MessageListControl()
        {
            this.InitializeComponent();

            ViewModel.ScrollTo += ViewModel_ScrollTo;
            Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                });
            });
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Finds ItemStackPanel and MessageScroller from MessageList once loaded.
        /// </summary>
        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _messageScrollViewer = MessageList.FindChild<ScrollViewer>();
            _itemsStackPanel = sender as ItemsStackPanel;
            if (_messageScrollViewer != null)
            {
                _messageScrollViewer.ViewChanged += MessageScrollViewer_ViewChanged;
            }
        }

        /// <summary>
        /// Checks margins from end of view when messages are scrolled.
        /// </summary>
        private void MessageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.CurrentChannel == null || ViewModel.ItemsLoading)
            {
                return;
            }

            if (MessageList.Items.Count > 0)
            {
                // Distance from top
                double fromTop = _messageScrollViewer.VerticalOffset;

                // Distance from bottom
                double fromBottom = _messageScrollViewer.ScrollableHeight - fromTop;

                // Load messages
                if (fromTop < 100)
                {
                    ViewModel.LoadOlderMessages();
                }

                // All messages seen, mark as read
                if (fromBottom < 10)
                {
                    ViewModel.CurrentChannel.MarkAsRead.Execute(null);
                }
            }
        }

        /// <summary>
        /// Scrolls <paramref name="e"/> into view.
        /// </summary>
        private void ViewModel_ScrollTo(object sender, BindableMessage e)
        {
            if (e != null)
            {
                MessageList.ScrollIntoView(e);
            }
        }
    }
}
