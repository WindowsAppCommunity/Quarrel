using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.ViewModels;
using DiscordAPI.Models;
using Microsoft.Advertising.WinRT.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Views
{
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

            nativeAdsManager.AdReady += NativeAdsManager_AdReady;
            nativeAdsManager.RequestAd();
        }

        private void NativeAdsManager_AdReady(object sender, NativeAdReadyEventArgs e)
        {
            PendingAd = e.NativeAd;
        }

        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        private ItemsStackPanel _ItemsStackPanel;
        private ScrollViewer _MessageScrollViewer;
        private NativeAdV2 _PendingAd;
        private NativeAdV2 PendingAd
        {
            get
            {
                nativeAdsManager.RequestAd();
                return _PendingAd;
            }
            set 
            {
                _PendingAd = value;
            }
        }
        private NativeAdsManagerV2 nativeAdsManager = new NativeAdsManagerV2("d25517cb-12d4-4699-8bdc-52040c712cab", "test");

        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _MessageScrollViewer = MessageList.FindChild<ScrollViewer>();
            _ItemsStackPanel = (sender as ItemsStackPanel);
            _ItemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepItemsInView;
            if (_MessageScrollViewer != null) _MessageScrollViewer.ViewChanged += _messageScrollViewer_ViewChanged;


            for (int i = 0; i < ViewModel.BindableMessages.Count; i++)
            {
                if (ViewModel.BindableMessages[i] == null)
                {
                    ViewModel.BindableMessages[i] = PendingAd;
                }
            }
        }

        private void _messageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.Channel == null)
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
                if (fromBottom < 200)
                    ViewModel.LoadNewerMessages();
            }

            for (int i = 0; i < ViewModel.BindableMessages.Count; i++)
            {
                if (ViewModel.BindableMessages[i] == null)
                {
                    ViewModel.BindableMessages[i] = PendingAd;
                }
            }
        }

        private void ViewModel_ScrollTo(object sender, BindableMessage e)
        {
            MessageList.ScrollIntoView(e);
        }
    }
}
