// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace Quarrel.Controls.Shell
{
    public sealed partial class SideDrawer : UserControl
    {
        private const string TranslationX = "Translation.X";

        private static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(Direction), typeof(Shadow), new PropertyMetadata(SideDrawerSize.Small, OnSizePropertyChanged));

        private static readonly DependencyProperty PrimaryPanelWidthProperty =
            DependencyProperty.Register(nameof(PrimaryPanelWidth), typeof(float), typeof(Shadow), new PropertyMetadata(72f, OnPanelWidthPropertyChanged));

        private static readonly DependencyProperty SecondaryPanelWidthProperty =
            DependencyProperty.Register(nameof(SecondaryPanelWidth), typeof(float), typeof(Shadow), new PropertyMetadata(228f, OnPanelWidthPropertyChanged));

        private InteractionTracker _tracker;
        private VisualInteractionSource _interactionSource;
        private Compositor _compositor;

        private Visual _mainVisual;
        private Visual _leftVisual;
        private Visual _left2Visual;
        private Visual _rightVisual;

        // These animations represent the position of the panes relative to the tracker position.
        private ExpressionAnimation? _mainTranslateAnimation;
        private ExpressionAnimation? _leftTranslateAnimation;
        private ExpressionAnimation? _left2TranslateAnimation;
        private ExpressionAnimation? _rightTranslateAnimation;

        // These mark the positions where the tracker will lock to
        // The start point is where the panel rests when the left panel is open
        // The mid point is where the panel rests when the panels are closed
        // The end point is where the panel rests when the right panel is open
        private InteractionTrackerInertiaRestingValue _startPoint;
        private InteractionTrackerInertiaRestingValue _midPoint;
        private InteractionTrackerInertiaRestingValue _endPoint;

        /// <summary>
        /// Inializes a new instance of the <see cref="SideDrawer"/> class.
        /// </summary>
        public SideDrawer()
        {
            this.InitializeComponent();

            Visual rootVisual = ElementCompositionPreview.GetElementVisual(rootgrid);
            _compositor = rootVisual.Compositor;

            // Main content
            _mainVisual = ElementCompositionPreview.GetElementVisual(main);
            ElementCompositionPreview.SetIsTranslationEnabled(main, true);

            // Main left panel
            _leftVisual = ElementCompositionPreview.GetElementVisual(left1);
            ElementCompositionPreview.SetIsTranslationEnabled(left1, true);

            // Secondary left panel
            _left2Visual = ElementCompositionPreview.GetElementVisual(left2);
            ElementCompositionPreview.SetIsTranslationEnabled(left2, true);

            // Right panel
            _rightVisual = ElementCompositionPreview.GetElementVisual(right);
            ElementCompositionPreview.SetIsTranslationEnabled(right, true);

            // Tracker
            _tracker = InteractionTracker.Create(_compositor);
            _interactionSource = VisualInteractionSource.Create(_mainVisual);
            _interactionSource.IsPositionXRailsEnabled = true;
            _interactionSource.PositionXChainingMode = InteractionChainingMode.Always;
            _interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;

            // Inertia resting
            _startPoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _midPoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _endPoint = InteractionTrackerInertiaRestingValue.Create(_compositor);

            Loading += OnLoading;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            // Defer setting up the animations and composition because
            // the animations depend on the FlowDirection, which is not set yet.
            // The composition is dependent on the animations.
            Loading -= OnLoading;
            SetupAnimations();
            SetupComposition();
        }

        /// <summary>
        /// Gets or sets the content displayed in the primary left panel.
        /// </summary>
        public object LeftContent
        {
            get => LeftMainContentControl.Content;
            set => LeftMainContentControl.Content = value;
        }

        /// <summary>
        /// Gets or sets the content displayed in the secondary left panel.
        /// </summary>
        public object LeftSecondaryContent
        {
            get => LeftSecondaryContentControl.Content;
            set => LeftSecondaryContentControl.Content = value;
        }

        /// <summary>
        /// Gets or sets the content displayed in the main panel.
        /// </summary>
        public object MainContent
        {
            get => MainContentControl.Content;
            set => MainContentControl.Content = value;
        }

        /// <summary>
        /// Gets or sets the content displayed in the right panel.
        /// </summary>
        public object RightContent
        {
            get => RightContentControl.Content;
            set => RightContentControl.Content = value;
        }

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.Medium"/> size UI.
        /// </summary>
        public double MediumMinSize { get; set; } = 600;

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.Large"/> size UI.
        /// </summary>
        public double LargeMinSize { get; set; } = 1100;

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.ExtraLarge"/> size UI.
        /// </summary>
        public double ExtraLargeMinSize { get; set; } = 1400;

        /// <summary>
        /// Gets or sets the width of the secondary panels.
        /// </summary>
        public float SecondaryPanelWidth
        {
            get => (float)GetValue(SecondaryPanelWidthProperty);
            set => SetValue(SecondaryPanelWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the primary panel.
        /// </summary>
        public float PrimaryPanelWidth
        {
            get => (float)GetValue(PrimaryPanelWidthProperty);
            set => SetValue(PrimaryPanelWidthProperty, value);
        }

        /// <summary>
        /// Gets the width of the total panel gap on the left.
        /// </summary>
        public float TotalPanelWidth => PrimaryPanelWidth + SecondaryPanelWidth;

        /// <summary>
        /// Gets whether or not the left panel is open.
        /// </summary>
        /// <remarks>
        /// This does not indicate whether or not the left panel is displayed.
        /// In <see cref="SideDrawerSize.Large"/> and <see cref="SideDrawerSize.ExtraLarge"/> the entire left panel is shown while not explictly open and
        /// in <see cref="SideDrawerSize.Medium"/> the primary left panel is shown while not explictly open.
        /// </remarks>
        public bool IsLeftOpen => FlowDirection == FlowDirection.LeftToRight ? _tracker.Position.X < 0 : _tracker.Position.X > 0;

        /// <summary>
        /// Gets whether or not the right panel is open.
        /// </summary>
        /// <remarks>
        /// This does not indicate whether or not the right panel is displayed.
        /// In <see cref="SideDrawerSize.ExtraLarge"/> the right panel is displayed while not explictly open.
        /// </remarks>
        public bool IsRightOpen => FlowDirection == FlowDirection.LeftToRight ? _tracker.Position.X > 0 : _tracker.Position.X < 0;

        private SideDrawerSize Size
        {
            get => (SideDrawerSize)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        /// <summary>
        /// Opens the left panel if it is current closed, closes the left panel if it is currently open.
        /// </summary>
        public void ToggleLeft() => ToggleLeft(null);

        /// <summary>
        /// Opens the right panel if it is current closed, closes the right panel if it is currently open.
        /// </summary>
        public void ToggleRight() => ToggleRight(null);

        /// <summary>
        /// Opens the left panel.
        /// </summary>
        public void OpenLeft() => ToggleLeft(true);

        /// <summary>
        /// Opens the right panel.
        /// </summary>
        public void OpenRight() => ToggleRight(true);

        /// <summary>
        /// Closes the left panel.
        /// </summary>
        public void CloseLeft() => ToggleLeft(false);

        /// <summary>
        /// Closes the right panel.
        /// </summary>
        public void CloseRight() => ToggleRight(false);

        private void ToggleLeft(bool? open)
        {
            float xClosed = 0;
            float xOpen = _tracker.MinPosition.X;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                xOpen = _tracker.MaxPosition.X;
            }

            if (!open.HasValue)
            {
                open = !IsLeftOpen;
            }

            float target = open.Value ? xOpen : xClosed;
            TrackerTranslate(target);
        }

        private void ToggleRight(bool? open)
        {
            float xClosed = 0;
            float xOpen = _tracker.MaxPosition.X;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                xOpen = _tracker.MinPosition.X;
            }

            if (!open.HasValue)
            {
                open = !IsRightOpen;
            }

            float target = open.Value ? xOpen : xClosed;
            TrackerTranslate(target);
        }

        private void SetupComposition()
        {
            Guard.IsNotNull(_mainTranslateAnimation, nameof(_mainTranslateAnimation));
            Guard.IsNotNull(_leftTranslateAnimation, nameof(_leftTranslateAnimation));
            Guard.IsNotNull(_left2TranslateAnimation, nameof(_left2TranslateAnimation));
            Guard.IsNotNull(_rightTranslateAnimation, nameof(_rightTranslateAnimation));

            main.Margin = new Thickness(0, 0, 0, 0);
            right.Margin = new Thickness(0, 0, -PrimaryPanelWidth, 0);

            _tracker.MaxPosition = new Vector3(SecondaryPanelWidth, 0, 0);
            _tracker.MinPosition = new Vector3(-TotalPanelWidth, 0, 0);
            _tracker.TryUpdatePosition(Vector3.Zero);
            _tracker.InteractionSources.Add(_interactionSource);

            // Main panel
            _mainTranslateAnimation.SetReferenceParameter("tracker", _tracker);
            _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);

            // Left main panel
            _leftTranslateAnimation.SetReferenceParameter("tracker", _tracker);
            _leftTranslateAnimation.SetScalarParameter("width", TotalPanelWidth);
            _leftVisual.StartAnimation(TranslationX, _leftTranslateAnimation);

            // Left secondary panel
            _left2TranslateAnimation.SetReferenceParameter("tracker", _tracker);
            _left2TranslateAnimation.SetScalarParameter("width", TotalPanelWidth);
            _left2TranslateAnimation.SetScalarParameter("primarywidth", PrimaryPanelWidth);
            _left2Visual.StartAnimation(TranslationX, _left2TranslateAnimation);

            // Right panel
            _rightTranslateAnimation.SetReferenceParameter("tracker", _tracker);
            _rightTranslateAnimation.SetScalarParameter("width", SecondaryPanelWidth);
            _rightTranslateAnimation.SetScalarParameter("primarywidth", PrimaryPanelWidth);
            _rightVisual.StartAnimation(TranslationX, _rightTranslateAnimation);

            SetSnapPoints(-TotalPanelWidth, 0, SecondaryPanelWidth);
        }

        private void SetupAnimations()
        {
            // The animations are dependent on the flow direction because
            // the interaction tracker does not acknowledge the flow direction
            // and will always treat left as negative and right as positive.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                _mainTranslateAnimation = _compositor.CreateExpressionAnimation("-tracker.Position.X");
                _leftTranslateAnimation = _compositor.CreateExpressionAnimation("-24+((-tracker.Position.X/width)*24)");
                _left2TranslateAnimation = _compositor.CreateExpressionAnimation("-tracker.Position.X/width*primarywidth");
                _rightTranslateAnimation = _compositor.CreateExpressionAnimation("-tracker.Position.X/width*primarywidth");
            }
            else
            {
                _mainTranslateAnimation = _compositor.CreateExpressionAnimation("tracker.Position.X");
                _leftTranslateAnimation = _compositor.CreateExpressionAnimation("-24+((tracker.Position.X/width)*24)");
                _left2TranslateAnimation = _compositor.CreateExpressionAnimation("tracker.Position.X/width*primarywidth");
                _rightTranslateAnimation = _compositor.CreateExpressionAnimation("tracker.Position.X/width*primarywidth");
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                try
                {
                    _interactionSource.TryRedirectForManipulation(e.GetCurrentPoint(rootgrid));
                }
                catch
                {
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SideDrawerSize size = SideDrawerSize.ExtraLarge;
            double width = e.NewSize.Width;
            if (width < MediumMinSize)
            {
                size = SideDrawerSize.Small;
            }
            else if (width < LargeMinSize)
            {
                size = SideDrawerSize.Medium;
            }
            else if (width < ExtraLargeMinSize)
            {
                size = SideDrawerSize.Large;
            }

            Size = size;
        }

        private async void SetSizeSmall(SideDrawerSize previous)
        {
            main.Margin = new Thickness(0, 0, 0, 0);
            right.Margin = new Thickness(0, 0, -PrimaryPanelWidth, 0);

            // Tracker bounds
            _tracker.MaxPosition = new Vector3(SecondaryPanelWidth, 0, 0);
            _tracker.MinPosition = new Vector3(-TotalPanelWidth, 0, 0);

            // Main panel
            _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);

            // Left panels
            Translate(_leftVisual, 0);
            Translate(_left2Visual, 0);
            _leftVisual.StartAnimation(TranslationX, _leftTranslateAnimation);
            _left2Visual.StartAnimation(TranslationX, _left2TranslateAnimation);

            if (previous == SideDrawerSize.Medium)
            {
                Translate(_mainVisual, 0, PrimaryPanelWidth);
                await Task.Delay(300);
                _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);
            }

            SetSnapPoints(-TotalPanelWidth, 0, SecondaryPanelWidth);
        }

        private async void SetSizeMedium(SideDrawerSize previous)
        {
            main.Margin = new Thickness(PrimaryPanelWidth, 0, 0, 0);
            right.Margin = new Thickness(0, 0, -PrimaryPanelWidth, 0);

            // Tracker bounds
            _tracker.MaxPosition = new Vector3(SecondaryPanelWidth, 0, 0);
            _tracker.MinPosition = new Vector3(-TotalPanelWidth + PrimaryPanelWidth, 0, 0);

            SetSnapPoints(-TotalPanelWidth + PrimaryPanelWidth, 0, SecondaryPanelWidth);

            // Main panel
            _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);

            // Left panels
            _leftVisual.StopAnimation(TranslationX);
            _left2Visual.StopAnimation(TranslationX);
            Translate(_leftVisual, 0);
            Translate(_left2Visual, PrimaryPanelWidth);

            if (previous == SideDrawerSize.Small)
            {
                Translate(_mainVisual, 0, -PrimaryPanelWidth);
                await Task.Delay(300);
                _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);
            }

            if (previous == SideDrawerSize.Large)
            {
                Translate(_mainVisual, 0, SecondaryPanelWidth);
                await Task.Delay(300);
                _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);
            }
        }

        private async void SetSizeLarge(SideDrawerSize previous)
        {
            main.Margin = new Thickness(TotalPanelWidth, 0, 0, 0);
            right.Margin = new Thickness(0, 0, -PrimaryPanelWidth, 0);

            // Tracker bounds
            _tracker.MaxPosition = new Vector3(SecondaryPanelWidth, 0, 0);
            _tracker.MinPosition = new Vector3(0, 0, 0);
            SetSnapPoints(0, 0, SecondaryPanelWidth);

            // Main panel
            _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);

            // Left panels
            _leftVisual.StopAnimation(TranslationX);
            _left2Visual.StopAnimation(TranslationX);
            Translate(_leftVisual, 0);
            Translate(_left2Visual, PrimaryPanelWidth);

            if (previous == SideDrawerSize.Small || previous == SideDrawerSize.Medium)
            {
                Translate(_mainVisual, 0, -TotalPanelWidth + PrimaryPanelWidth);
                await Task.Delay(300);
                _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);
            }
        }

        private async void SetSizeExtraLarge(SideDrawerSize previous)
        {
            main.Margin = new Thickness(TotalPanelWidth, 0, SecondaryPanelWidth, 0);
            right.Margin = new Thickness(0);

            // Tracker bounds
            _tracker.MaxPosition = Vector3.Zero;
            _tracker.MinPosition = Vector3.Zero;
            SetSnapPoints(0, 0, 0);

            // Main panel
            _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);

            // Left panels
            _leftVisual.StopAnimation(TranslationX);
            _left2Visual.StopAnimation(TranslationX);
            Translate(_leftVisual, 0);
            Translate(_left2Visual, PrimaryPanelWidth);

            if (previous == SideDrawerSize.Large)
            {
                Translate(_mainVisual, 0, SecondaryPanelWidth);
                await Task.Delay(300);
                _mainVisual.StartAnimation(TranslationX, _mainTranslateAnimation);
            }
        }

        /// <summary>
        /// Moves the translation of a single visual.
        /// </summary>
        private void Translate(Visual visual, float to, float? from = null)
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));

            // Create the Vector3 KFA
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.3);
            if (from.HasValue)
            {
                kfa.InsertKeyFrame(0.0f, Vector3.UnitX * from.Value);
            }

            kfa.InsertKeyFrame(1.0f, Vector3.UnitX * to, cubicBezier);
            visual.StartAnimation("Translation", kfa);
        }

        /// <summary>
        /// Programmatically moves the tracker position.
        /// </summary>
        private void TrackerTranslate(float to, float? from = null)
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            if (from.HasValue)
            {
                kfa.InsertKeyFrame(0.0f, Vector3.UnitX * from.Value);
            }

            kfa.InsertKeyFrame(1.0f, Vector3.UnitX * to, cubicBezier);
            // Update InteractionTracker position using this animation
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        private void SetSnapPoints(float startPoint, float midPoint, float endPoint)
        {
            _tracker.PositionInertiaDecayRate = Vector3.One;

            // Snap to left if more than halfway to starting point from mid point.
            _startPoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x < -halfwidth");
            _startPoint.Condition.SetScalarParameter("halfwidth", TotalPanelWidth / 2);
            _startPoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _startPoint.RestingValue.SetScalarParameter("pos", startPoint);

            // Snap to center if not on left or right.
            _midPoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > -halfwidth && this.Target.NaturalRestingPosition.x < halfwidth");
            _midPoint.Condition.SetScalarParameter("halfwidth", TotalPanelWidth / 2);
            _midPoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _midPoint.RestingValue.SetScalarParameter("pos", midPoint);

            // Snap to right if more than halfway to ending point from mid point.
            _endPoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > halfwidth");
            _endPoint.Condition.SetScalarParameter("halfwidth", endPoint / 2);
            _endPoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _endPoint.RestingValue.SetScalarParameter("pos", endPoint);

            _tracker.ConfigurePositionXInertiaModifiers(new InteractionTrackerInertiaModifier[] { _startPoint, _midPoint, _endPoint });
        }

        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SideDrawer sideDrawer = (SideDrawer)d;
            SideDrawerSize newSize = (SideDrawerSize)args.NewValue;
            SideDrawerSize oldSize = (SideDrawerSize)args.OldValue;

            if (newSize == oldSize)
            {
                return;
            }

            sideDrawer.TrackerTranslate(0);

            switch (newSize)
            {
                case SideDrawerSize.Small:
                    sideDrawer.SetSizeSmall(oldSize);
                    break;
                case SideDrawerSize.Medium:
                    sideDrawer.SetSizeMedium(oldSize);
                    break;
                case SideDrawerSize.Large:
                    sideDrawer.SetSizeLarge(oldSize);
                    break;
                case SideDrawerSize.ExtraLarge:
                    sideDrawer.SetSizeExtraLarge(oldSize);
                    break;
            }

            // Adjust tracker bounds for right to left flow direction
            if (sideDrawer.FlowDirection == FlowDirection.RightToLeft)
            {
                float rawMin = sideDrawer._tracker.MinPosition.X;
                float rawMax = sideDrawer._tracker.MaxPosition.X;
                sideDrawer._tracker.MinPosition = -rawMax * Vector3.UnitX;
                sideDrawer._tracker.MaxPosition = -rawMin * Vector3.UnitX;
            }
        }

        private static void OnPanelWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }
    }
}
