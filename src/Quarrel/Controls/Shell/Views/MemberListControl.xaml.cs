using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle member list
    /// </summary>
    public sealed partial class MemberListControl : UserControl
    {
        public MemberListControl()
        {
            this.InitializeComponent();
            // Scrolls the MemberList to the top when the Channel changes
            Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
            {
                MemberList.ScrollIntoView(ViewModel.CurrentBindableMembers.FirstOrDefault());
            });
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Zooms to approipate header when Header selected from Semantic Out view
        /// </summary>
        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                var sourceItem = e.SourceItem.Item as BindableGuildMemberGroup;
                e.DestinationItem.Item = ViewModel.CurrentBindableMembers.FirstOrDefault(x => x is BindableGuildMemberGroup group && group.Model.Id == sourceItem.Model.Id);
            }
        }
        
        /// <summary>
        /// Finds first child of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of child to find</typeparam>
        /// <param name="root">Item must be a child of root</param>
        /// <returns>First item on Visual Stack, under root of type <typeparamref name="T"/></returns>
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    if (child is T typedChild)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        private long lastTime;

        private Timer timer;

        private void MemberListControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Todo: sticky headers
            ScrollViewer sv = FindChildOfType<ScrollViewer>(MemberList);
            ItemsStackPanel sp = FindChildOfType<ItemsStackPanel>(sv);
            timer = new Timer((state) =>
            {
                double top = sv.VerticalOffset;
                double bottom = sv.VerticalOffset + sv.ViewportHeight;
                double total = sv.ScrollableHeight + sv.ViewportHeight;
                ViewModel.UpdateGuildSubscriptionsCommand.Execute((top / total, bottom / total));
            }, null, Timeout.Infinite, Timeout.Infinite);
            sv.ViewChanging += (sender1, args) =>
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (currentTime - lastTime > 100)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    double top = sv.VerticalOffset;
                    double bottom = sv.VerticalOffset + sv.ViewportHeight;
                    double total = sv.ScrollableHeight + sv.ViewportHeight;
                    ViewModel.UpdateGuildSubscriptionsCommand.Execute((top / total, bottom / total));
                }
                else
                {
                    timer.Change(110, Timeout.Infinite);
                }

                lastTime = currentTime;
             };
        }
    }
}
