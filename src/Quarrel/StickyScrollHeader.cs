using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel
{
    public static class StickyScrollHeader
    {

        private static List<FrameworkElement> elements = new List<FrameworkElement>();
        public static FrameworkElement GetAttachToControl(FrameworkElement obj)
        {
            return (FrameworkElement)obj.GetValue(AttachToControlProperty);
        }

        public static void SetAttachToControl(FrameworkElement obj, FrameworkElement value)
        {
            obj.SetValue(ListViewProperty, value);
        }
        public static FrameworkElement GetListView(FrameworkElement obj)
        {
            return (FrameworkElement)obj.GetValue(ListViewProperty);
        }

        public static void SetListView(FrameworkElement obj, FrameworkElement value)
        {
            obj.SetValue(AttachToControlProperty, value);
        }
        private static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem item)
                    return item;
                childItem childOfChild = FindVisualChild<childItem>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static readonly DependencyProperty ListViewProperty =
            DependencyProperty.RegisterAttached("ListView", typeof(ListView), typeof(StickyScrollHeader), new PropertyMetadata(null, (s, e) =>
            {
                if (!(s is ListView targetControl))
                    return;
                ScrollViewer sv = FindVisualChild<ScrollViewer>(s);

                sv.ViewChanged += (sender, args) => { Debug.WriteLine(sv.VerticalOffset); };

            }));

        public static readonly DependencyProperty AttachToControlProperty =
            DependencyProperty.RegisterAttached("AttachToControl", typeof(FrameworkElement), typeof(StickyScrollHeader), new PropertyMetadata(null, (s, e) =>
            {
                if (!(s is FrameworkElement targetControl))
                    return; 

                Canvas.SetZIndex(targetControl, 999);
                elements.Add(targetControl);
            }));
    }

}
