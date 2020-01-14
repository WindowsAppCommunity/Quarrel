using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell.Views
{
    public sealed partial class MemberListControl : UserControl
    {
        public MemberListControl()
        {
            this.InitializeComponent();
        }

        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                var sourceItem = e.SourceItem.Item as BindableGuildMemberGroup;
                e.DestinationItem.Item = ViewModel.BindableMembersNew.FirstOrDefault(x => x is BindableGuildMemberGroup group && group.Model.Id == sourceItem.Model.Id);
            }
        }
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
                 /*
                 var child = (UIElement)VisualTreeHelper.GetChild(sp, 2);
                 UIElement item1 = FindChildOfType<TextBlock>(child);
                 TranslateTransform targetTransform1 = new TranslateTransform { X = 0, Y = 10 - sv.VerticalOffset };
                 item1.RenderTransform = targetTransform1;
                 Canvas.SetZIndex(item1, 999999);
                 var tmp = ElementCompositionPreview.GetElementVisual(child);
                 tmp.Size = new Vector2(228, 48);
                 ElementCompositionPreview.SetElementChildVisual(child, tmp);
 
                 UIElement item = (UIElement)MemberList.ContainerFromItem(ViewModel.BindableMembersNew[0]);
                 Canvas.SetZIndex(item, 999999);
                 TranslateTransform targetTransform = new TranslateTransform {X = 0, Y = sv.VerticalOffset};
                 item.RenderTransform = targetTransform;
                 Visual handout = ElementCompositionPreview.GetElementVisual(item);
                 handout.Offset = new Vector3();
                 handout.Size = new Vector2();
 
 
                 RectangleGeometry rg = new RectangleGeometry();
                 rg.Transform = new CompositeTransform { TranslateY = sv.VerticalOffset + 49};
                 rg.Rect = new Rect(0, 0, 228, 4808);
                 sp.Clip = rg;
                 Debug.WriteLine(sv.VerticalOffset);*/
             };
        }
    }
}
