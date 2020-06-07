// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace Quarrel.Controls.Shell
{
    /// <summary>
    /// Control for showing 4 panels stacked and sliding.
    /// </summary>
    public sealed partial class SideDrawer : UserControl
    {
        private readonly int width = 300;
        private bool _fullscreen = false;
        private Visual _rootVisual;
        private Compositor _compositor;
        private InteractionTracker _tracker;
        private VisualInteractionSource _interactionSource;
        private ExpressionAnimation _contentAnimation;
        private ExpressionAnimation _leftPanelFadeAnimation;
        private ExpressionAnimation _leftPanelTranslateAnimation2;
        private ExpressionAnimation _leftPanelFadeAnimation2;
        private InteractionTrackerInertiaRestingValue _startpoint;
        private InteractionTrackerInertiaRestingValue _midpoint;
        private InteractionTrackerInertiaRestingValue _endpoint;
        private Visual contentVis;
        private Visual contentVis2;
        private Visual leftcache1;
        private Visual leftpanel1;
        private ExpressionAnimation leftPanelTranslateAnimation;
        private Visual leftcache2;
        private Visual rightcache;
        private Visual rightpanel;
        private Visual leftpanel2;
        private ExpressionAnimation rightPanelFadeAnimation;
        private ExpressionAnimation rightPanelTranslateAnimation;
        private Vector3 _vector3Zero = new Vector3(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="SideDrawer"/> class.
        /// </summary>
        public SideDrawer()
        {
            this.InitializeComponent();

            // Link objects for easy access
            _rootVisual = ElementCompositionPreview.GetElementVisual(maingrid);
            _compositor = _rootVisual.Compositor;
        }

        /// <summary>
        /// Invoked when the Left Drawer is opened
        /// </summary>
        public event EventHandler DrawOpenedLeft;

        /// <summary>
        /// Invoked when the Right Drawer is opened
        /// </summary>
        public event EventHandler DrawOpenedRight;

        /// <summary>
        /// Invoked when a Drawer closes
        /// </summary>
        public event EventHandler DrawsClosed;

        /// <summary>
        /// Invoked when the SecondaryLeft panel gains focus.
        /// </summary>
        public event EventHandler SecondaryLeftFocused;

        /// <summary>
        /// Gets or sets control to display in Left Panel.
        /// </summary>
        public object ContentLeft
        {
            get { return contentLeft1.Content; }
            set { contentLeft1.Content = value; }
        }

        /// <summary>
        /// Gets or sets control to display in Left Secondary Panel.
        /// </summary>
        public object ContentLeftSecondary
        {
            get { return contentLeft2.Content; }
            set { contentLeft2.Content = value; }
        }

        /// <summary>
        /// Gets or sets control to display in Main Panel.
        /// </summary>
        public object ContentMain
        {
            get { return ContentControl1.Content; }
            set { ContentControl1.Content = value; }
        }

        /// <summary>
        /// Gets or sets control to display in Rigth Panel.
        /// </summary>
        public object ContentRight
        {
            get { return contentRight.Content; }
            set { contentRight.Content = value; }
        }

        /// <summary>
        /// Setup interactions with sliding touch controls.
        /// </summary>
        /// <param name="detachedHeader">Uneffected element.</param>
        public void SetupInteraction(UIElement detachedHeader = null)
        {
            // Set up tracker
            var containerVisual = _compositor.CreateContainerVisual();
            contentVis = ElementCompositionPreview.GetElementVisual(content);
            if (detachedHeader != null)
            {
                contentVis2 = ElementCompositionPreview.GetElementVisual(detachedHeader);
            }

            ElementCompositionPreview.SetIsTranslationEnabled(content, true);
            if (detachedHeader != null)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(detachedHeader, true);
            }

            contentVis.Properties.InsertVector3("Translation", Vector3.Zero);
            if (detachedHeader != null)
            {
                contentVis2.Properties.InsertVector3("Translation", Vector3.Zero);
            }

            _interactionSource = VisualInteractionSource.Create(_rootVisual);
            _interactionSource.IsPositionXRailsEnabled = true;

            // If the chaining mode is set to always, it is impossible to go past by the minimum and maximum scrolling points by "stretching" the X axis
            _interactionSource.PositionXChainingMode = InteractionChainingMode.Always;
            _interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;
            _tracker = InteractionTracker.Create(_compositor);
            _tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            _tracker.MinPosition = new Vector3(-width, 0, 0);
            _tracker.TryUpdatePosition(new Vector3(0, 0, 0));
            _tracker.InteractionSources.Add(_interactionSource);

            // Define expression animations
            _contentAnimation = _compositor.CreateExpressionAnimation("-tracker.Position.X");
            _contentAnimation.SetReferenceParameter("tracker", _tracker);
            contentVis.StartAnimation("Translation.X", _contentAnimation);
            if (detachedHeader != null)
            {
                contentVis2.StartAnimation("Translation.X", _contentAnimation);
            }

            // LEFT PANEL
            leftcache1 = ElementCompositionPreview.GetElementVisual(leftPanelCache);
            leftpanel1 = ElementCompositionPreview.GetElementVisual(leftPanel);
            ElementCompositionPreview.SetIsTranslationEnabled(leftPanel, true);
            _leftPanelFadeAnimation = _compositor.CreateExpressionAnimation("Clamp((-tracker.Position.X/width)*(-0.5)+0.25,0,1)");
            _leftPanelFadeAnimation.SetScalarParameter("width", width);
            _leftPanelFadeAnimation.SetReferenceParameter("tracker", _tracker);
            leftcache1.StartAnimation("Opacity", _leftPanelFadeAnimation);
            leftPanelTranslateAnimation = _compositor.CreateExpressionAnimation("-24+((-tracker.Position.X/width)*24)");
            leftPanelTranslateAnimation.SetReferenceParameter("tracker", _tracker);
            leftPanelTranslateAnimation.SetScalarParameter("width", width);
            leftpanel1.StartAnimation("Translation.X", leftPanelTranslateAnimation);

            // SECONDARY LEFT PANEL
            leftcache2 = ElementCompositionPreview.GetElementVisual(leftSecondaryPanelCache);
            leftpanel2 = ElementCompositionPreview.GetElementVisual(leftSecondaryPanel);
            ElementCompositionPreview.SetIsTranslationEnabled(leftSecondaryPanel, true);
            _leftPanelFadeAnimation2 = _compositor.CreateExpressionAnimation("Clamp((-tracker.Position.X/width)*(-0.25)+0.25, 0, 1)");
            _leftPanelFadeAnimation2.SetScalarParameter("width", width);
            _leftPanelFadeAnimation2.SetReferenceParameter("tracker", _tracker);
            leftcache2.StartAnimation("Opacity", _leftPanelFadeAnimation2);
            _leftPanelTranslateAnimation2 = _compositor.CreateExpressionAnimation("-tracker.Position.X/width*72");
            _leftPanelTranslateAnimation2.SetReferenceParameter("tracker", _tracker);
            _leftPanelTranslateAnimation2.SetScalarParameter("width", width);
            leftpanel2.StartAnimation("Translation.X", _leftPanelTranslateAnimation2);

            // RIGHT PANEL
            rightcache = ElementCompositionPreview.GetElementVisual(rightPanelCache);
            rightpanel = ElementCompositionPreview.GetElementVisual(rightSide);
            ElementCompositionPreview.SetIsTranslationEnabled(rightSide, true);
            rightPanelFadeAnimation = _compositor.CreateExpressionAnimation("Clamp((tracker.Position.X/width)*(-0.25)+0.25, 0, 1)");
            rightPanelFadeAnimation.SetScalarParameter("width", width - 72);
            rightPanelFadeAnimation.SetReferenceParameter("tracker", _tracker);
            rightcache.StartAnimation("Opacity", rightPanelFadeAnimation);
            rightPanelTranslateAnimation = _compositor.CreateExpressionAnimation("-tracker.Position.X/width*72");
            rightPanelTranslateAnimation.SetReferenceParameter("tracker", _tracker);
            rightPanelTranslateAnimation.SetScalarParameter("width", width - 72);
            rightpanel.StartAnimation("Translation.X", rightPanelTranslateAnimation);
            SetSnapPoints(-width, 0, width - 72);

            // UI Stuff
            var state = VisualStateGroup.CurrentState;
            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = Small });
        }

        /// <summary>
        /// Opens the left drawer if closed.
        /// </summary>
        public void OpenLeft()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, _tracker.MinPosition, cubicBezier);
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Closes the left drawer if open.
        /// </summary>
        public void CloseLeft()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, _vector3Zero, cubicBezier);
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Opens the right drawer if closed.
        /// </summary>
        public void OpenRight()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, _tracker.MaxPosition, cubicBezier);
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Closes the right rawer if open.
        /// </summary>
        public void CloseRight()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, _vector3Zero, cubicBezier);
            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Toggles the left drawer's open status.
        /// </summary>
        public void ToggleLeft()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            if (_tracker.Position.X < 0)
            {
                kfa.InsertKeyFrame(1.0f, _vector3Zero, cubicBezier);
                DrawsClosed?.Invoke(null, null);
            }
            else
            {
                kfa.InsertKeyFrame(1.0f, _tracker.MinPosition, cubicBezier);
                DrawOpenedLeft?.Invoke(null, null);
            }

            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Returnes whether the left drawer is open or not.
        /// </summary>
        public bool IsLeftOpen()
        {
            return _tracker.Position.X < 0;
        }

        /// <summary>
        /// Returnes whether the right drawer is open or not.
        /// </summary>
        public bool IsRightOpen()
        {
            return _tracker.Position.X > 0;
        }

        /// <summary>
        /// Toggles the right drawer's open status.
        /// </summary>
        public void ToggleRight()
        {
            CompositionEasingFunction cubicBezier = _compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = _compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            if (_tracker.Position.X > 0)
            {
                DrawsClosed?.Invoke(null, null);
                kfa.InsertKeyFrame(1.0f, _vector3Zero, cubicBezier);
            }
            else
            {
                DrawOpenedRight?.Invoke(null, null);
                kfa.InsertKeyFrame(1.0f, _tracker.MaxPosition, cubicBezier);
            }

            _tracker.TryUpdatePositionWithAnimation(kfa);
        }

        /// <summary>
        /// Toggles if the view is being used full screen.
        /// </summary>
        public void ToggleFullScreen()
        {
            if (!_fullscreen)
            {
                SmallTrigger.MinWindowWidth = 0;
                MediumTrigger.MinWindowWidth = 100000;
                LargeTrigger.MinWindowWidth = 100000;
                ExtraLargeTrigger.MinWindowWidth = 100000;
                _fullscreen = true;
            }
            else
            {
                SmallTrigger.MinWindowWidth = 100000;
                MediumTrigger.MinWindowWidth = 0;
                LargeTrigger.MinWindowWidth = 100000;
                ExtraLargeTrigger.MinWindowWidth = 100000;
                _fullscreen = false;
            }

            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = VisualStateGroup.CurrentState });
        }

        private void App_LocalSettingsUpdatedHandler(object sender, EventArgs e)
        {
            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = VisualStateGroup.CurrentState });
        }

        private async void SmallInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Visible;
            leftSecondaryPanelCache.Visibility = Visibility.Visible;
            rightPanelCache.Visibility = Visibility.Visible;

            _tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            _tracker.MinPosition = new Vector3(-width, 0, 0);
            content.Margin = new Thickness(0, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            contentVis.StartAnimation("Translation.X", _contentAnimation);
            if (contentVis2 != null)
            {
                contentVis2.StartAnimation("Translation.X", _contentAnimation);
            }

            leftcache1.StartAnimation("Opacity", _leftPanelFadeAnimation);
            Translate(leftpanel1, 0);
            leftpanel1.StartAnimation("Translation.X", leftPanelTranslateAnimation);
            leftcache2.StartAnimation("Opacity", _leftPanelFadeAnimation2);
            Translate(leftpanel2, 0);
            leftpanel2.StartAnimation("Translation.X", _leftPanelTranslateAnimation2);

            if (previous == Mid)
            {
                Translate(contentVis, 0, 72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, 72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
            }

            SetSnapPoints(-width, 0, width - 72);
        }

        private async void MidInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Visible;
            rightPanelCache.Visibility = Visibility.Visible;

            _tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            _tracker.MinPosition = new Vector3(-width + 72, 0, 0);
            content.Margin = new Thickness(72, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            SetSnapPoints(-width + 72, 0, width - 72);

            contentVis.StartAnimation("Translation.X", _contentAnimation);
            if (contentVis2 != null)
            {
                contentVis2.StartAnimation("Translation.X", _contentAnimation);
            }

            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", _leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            if (previous == Small)
            {
                Translate(contentVis, 0, -72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, -72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
            }

            if (previous == Large)
            {
                Translate(contentVis, 0, width - 72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, width - 72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
            }
        }

        private async void LargeInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Collapsed;
            rightPanelCache.Visibility = Visibility.Visible;

            _tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            _tracker.MinPosition = new Vector3(0, 0, 0);
            content.Margin = new Thickness(width, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            SetSnapPoints(0, 0, width - 72);

            contentVis.StartAnimation("Translation.X", _contentAnimation);
            if (contentVis2 != null)
            {
                contentVis2.StartAnimation("Translation.X", _contentAnimation);
            }

            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", _leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            if (previous == Small)
            {
                Translate(contentVis, 0, -width + 72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, -width + 72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
            }

            if (previous == Mid)
            {
                Translate(contentVis, 0, -width + 72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, -width + 72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
            }
        }

        private async void ExtraLargeInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Collapsed;
            rightPanelCache.Visibility = Visibility.Collapsed;

            _tracker.MaxPosition = new Vector3(0, 0, 0);
            _tracker.MinPosition = new Vector3(0, 0, 0);
            content.Margin = new Thickness(width, 0, width - 72, 0);
            rightSide.Margin = new Thickness(0, 0, 0, 0);
            SetSnapPoints(0, 0, 0);

            contentVis.StartAnimation("Translation.X", _contentAnimation);
            if (contentVis2 != null)
            {
                contentVis2.StartAnimation("Translation.X", _contentAnimation);
            }

            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", _leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            if (previous == Large)
            {
                Translate(contentVis, 0, width - 72);
                if (contentVis2 != null)
                {
                    Translate(contentVis2, 0, width - 72);
                }

                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", _contentAnimation);
                if (contentVis2 != null)
                {
                    contentVis2.StartAnimation("Translation.X", _contentAnimation);
                }
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
                kfa.InsertKeyFrame(0.0f, value: new Vector3(from.Value, 0, 0));
            }

            kfa.InsertKeyFrame(1.0f, new Vector3(to, 0, 0), cubicBezier);

            // Update InteractionTracker position using this animation
            visual.StartAnimation("Translation", kfa);
        }

        private void SetSnapPoints(float startPoint, float midPoint, float endPoint)
        {
            _tracker.PositionInertiaDecayRate = new Vector3(1f);
            var trackerTarget = ExpressionValues.Target.CreateInteractionTrackerTarget();

            _startpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _startpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x < -halfwidth");
            _startpoint.Condition.SetScalarParameter("halfwidth", width / 2);
            _startpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _startpoint.RestingValue.SetScalarParameter("pos", startPoint);

            _midpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _midpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > -halfwidth && this.Target.NaturalRestingPosition.x < halfwidth");
            _midpoint.Condition.SetScalarParameter("halfwidth", width / 2);
            _midpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _midpoint.RestingValue.SetScalarParameter("pos", midPoint);

            _endpoint = InteractionTrackerInertiaRestingValue.Create(_compositor);
            _endpoint.Condition = _compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > halfwidth");
            _endpoint.Condition.SetScalarParameter("halfwidth", endPoint / 2);
            _endpoint.RestingValue = _compositor.CreateExpressionAnimation("pos");
            _endpoint.RestingValue.SetScalarParameter("pos", endPoint);

            _tracker.ConfigurePositionXInertiaModifiers(new InteractionTrackerInertiaModifier[] { _startpoint, _midpoint, _endpoint });
        }

        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                try
                {
                    _interactionSource.TryRedirectForManipulation(e.GetCurrentPoint(maingrid));
                }
                catch
                {
                }
            }
        }

        private void UCLoaded(object sender, RoutedEventArgs e)
        {
            _rootVisual = ElementCompositionPreview.GetElementVisual(maingrid);
            _compositor = _rootVisual.Compositor;
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var state = VisualStateGroup.CurrentState;
            if (state == Small)
            {
                SmallInteractable(e.OldState);
            }
            else if (state == Mid)
            {
                MidInteractable(e.OldState);
            }
            else if (state == Large)
            {
                LargeInteractable(e.OldState);
            }
            else if (state == ExtraLarge)
            {
                ExtraLargeInteractable(e.OldState);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void ContentLeft2_GotFocus(object sender, RoutedEventArgs e)
        {
            SecondaryLeftFocused?.Invoke(null, null);
        }
    }
}
