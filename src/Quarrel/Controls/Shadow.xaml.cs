// Adam Dernis © 2022

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    public sealed partial class Shadow : UserControl
    {
        private static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(nameof(Direction), typeof(Direction), typeof(Shadow), new PropertyMetadata(Direction.Left, OnDirectionChanged));

        private static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(nameof(Start), typeof(Point), typeof(Shadow), new PropertyMetadata(default));

        private static readonly DependencyProperty EndProperty =
            DependencyProperty.Register(nameof(End), typeof(Point), typeof(Shadow), new PropertyMetadata(default));

        public Shadow()
        {
            this.InitializeComponent();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnDirectionChanged(this, null);
        }

        public Direction Direction
        {
            get => (Direction)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        private Point Start
        {
            get => (Point)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        private Point End
        {
            get => (Point)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs? args)
        {
            Shadow shadow = (Shadow)d;
            bool horizontal = shadow.Direction == Direction.Left || shadow.Direction == Direction.Right;
            bool inverse = shadow.Direction == Direction.Up || shadow.Direction == Direction.Right;

            float u = 0;
            float v = 1;

            if (horizontal)
            {
                // Swap
                (u, v) = (v, u);
            }

            Point start = new Point(u, v);
            Point end = new Point(0, 0);

            if (inverse)
            {
                (start, end) = (end, start);
            }

            shadow.Start = start;
            shadow.End = end;
        }
    }
}
