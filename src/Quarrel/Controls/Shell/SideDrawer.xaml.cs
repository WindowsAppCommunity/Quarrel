// Adam Dernis © 2022

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
        private ExpressionAnimation? _mainTranslateAnimation;
        private ExpressionAnimation? _leftTranslateAnimation;
        private ExpressionAnimation? _left2TranslateAnimation;
        private ExpressionAnimation? _rightTranslateAnimation;
        private InteractionTrackerInertiaRestingValue _startpoint;
        private InteractionTrackerInertiaRestingValue _midpoint;
        private InteractionTrackerInertiaRestingValue _endpoint;

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
            _startpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _midpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _endpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);

            Loading += OnLoading;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            Loading -= OnLoading;

            SetupAnimations();
            SetupComposition();
        }

        public object LeftContent
        {
            get => LeftMainContentControl.Content;
            set => LeftMainContentControl.Content = value;
        }

        public object LeftSecondaryContent
        {
            get => LeftSecondaryContentControl.Content;
            set => LeftSecondaryContentControl.Content = value;
        }

        public object MainContent
        {
            get => MainContentControl.Content;
            set => MainContentControl.Content = value;
        }

        public object RightContent
        {
            get => RightContentControl.Content;
            set => RightContentControl.Content = value;
        }

        public double MediumMinSize { get; set; } = 600;

        public double LargeMinSize { get; set; } = 1100;

        public double ExtraLargeMinSize { get; set; } = 1400;

        public float SecondaryPanelWidth
        {
            get => (float)GetValue(SecondaryPanelWidthProperty);
            set => SetValue(SecondaryPanelWidthProperty, value);
        }

        public float PrimaryPanelWidth
        {
            get => (float)GetValue(PrimaryPanelWidthProperty);
            set => SetValue(PrimaryPanelWidthProperty, value);
        }

        public float TotalPanelWidth => PrimaryPanelWidth + SecondaryPanelWidth;

        public bool IsLeftOpen => FlowDirection == FlowDirection.LeftToRight ? _tracker.Position.X < 0 : _tracker.Position.X > 0;

        public bool IsRightOpen => FlowDirection == FlowDirection.LeftToRight ? _tracker.Position.X > 0 : _tracker.Position.X < 0;

        private SideDrawerSize Size
        {
            get => (SideDrawerSize)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        private void SetupComposition()
        {
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

        public void ToggleLeft()
        {
            float closed = 0;
            float open = _tracker.MinPosition.X;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                open = _tracker.MaxPosition.X;
            }

            float target = IsLeftOpen ? closed : open;
            TrackerTranslate(target);
        }

        public void ToggleRight()
        {
            float closed = 0;
            float open = _tracker.MaxPosition.X;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                open = _tracker.MinPosition.X;
            }

            float target = IsRightOpen ? closed : open;
            TrackerTranslate(target);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                {
                    _interactionSource.TryRedirectForManipulation(e.GetCurrentPoint(rootgrid));
                }
            }
            catch
            {
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

            // Update InteractionTracker position using this animation
            visual.StartAnimation("Translation", kfa);
        }

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
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        private void SetSnapPoints(float startPoint, float midPoint, float endPoint)
        {
            _tracker.PositionInertiaDecayRate = Vector3.One;

            _startpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x < -halfwidth");
            _startpoint.Condition.SetScalarParameter("halfwidth", TotalPanelWidth / 2);
            _startpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _startpoint.RestingValue.SetScalarParameter("pos", startPoint);

            _midpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > -halfwidth && this.Target.NaturalRestingPosition.x < halfwidth");
            _midpoint.Condition.SetScalarParameter("halfwidth", TotalPanelWidth / 2);
            _midpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _midpoint.RestingValue.SetScalarParameter("pos", midPoint);

            _endpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > halfwidth");
            _endpoint.Condition.SetScalarParameter("halfwidth", endPoint / 2);
            _endpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _endpoint.RestingValue.SetScalarParameter("pos", endPoint);

            _tracker.ConfigurePositionXInertiaModifiers(new InteractionTrackerInertiaModifier[] { _startpoint, _midpoint, _endpoint });
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
