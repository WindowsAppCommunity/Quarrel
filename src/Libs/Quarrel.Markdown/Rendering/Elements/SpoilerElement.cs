// Quarrel © 2022

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel.Markdown
{
    public class SpoilerElement : Control
    {
        public PathGeometry PathGeometry
        {
            get { return (PathGeometry)GetValue(PathGeometryProperty); }
            set { SetValue(PathGeometryProperty, value); }
        }

        public static readonly DependencyProperty PathGeometryProperty =
            DependencyProperty.Register(nameof(PathGeometry), typeof(PathGeometry), typeof(SpoilerElement), null);

        public Brush Overlay
        {
            get { return (Brush)GetValue(OverlayProperty); }
            set { SetValue(OverlayProperty, value); }
        }

        public static readonly DependencyProperty OverlayProperty =
            DependencyProperty.Register(nameof(Overlay), typeof(Brush), typeof(SpoilerElement), null);

        public Brush OverlayHover
        {
            get { return (Brush)GetValue(OverlayHoverProperty); }
            set { SetValue(OverlayHoverProperty, value); }
        }

        public static readonly DependencyProperty OverlayHoverProperty =
            DependencyProperty.Register(nameof(OverlayHover), typeof(Brush), typeof(SpoilerElement), null);
        

        internal SpoilerElement()
        {
            this.DefaultStyleKey = typeof(SpoilerElement);
        }
    }
}
