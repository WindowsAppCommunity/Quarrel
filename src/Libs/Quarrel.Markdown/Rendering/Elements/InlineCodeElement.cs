// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel.Markdown
{
    public sealed class InlineCodeElement : Control
    {
        public PathGeometry PathGeometry
        {
            get { return (PathGeometry)GetValue(PathGeometryProperty); }
            set { SetValue(PathGeometryProperty, value); }
        }

        public static readonly DependencyProperty PathGeometryProperty =
            DependencyProperty.Register(nameof(PathGeometry), typeof(PathGeometry), typeof(InlineCodeElement), new PropertyMetadata(null));

        internal InlineCodeElement()
        {
            this.DefaultStyleKey = typeof(InlineCodeElement);
        }
    }
}
