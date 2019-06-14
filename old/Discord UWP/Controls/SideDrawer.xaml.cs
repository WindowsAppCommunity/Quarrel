using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class SideDrawer : UserControl
    {
        /// <summary>
        /// Control to display in Left Panel
        /// </summary>
        public object ContentLeft
        {
            get { return contentLeft1.Content; }
            set { contentLeft1.Content = value; }
        }

        /// <summary>
        /// Control to display in Left Secondary Panel
        /// </summary>
        public object ContentLeftSecondary
        {
            get { return contentLeft2.Content; }
            set { contentLeft2.Content = value; }
        }

        /// <summary>
        /// Control to display in Main Panel
        /// </summary>
        public object ContentMain
        {
            get { return ContentControl1.Content; }
            set { ContentControl1.Content = value; }
        }

        /// <summary>
        /// Control to display in Rigth Panel
        /// </summary>
        public object ContentRight
        {
            get { return contentRight.Content; }
            set { contentRight.Content = value; }
        }

        public event EventHandler SecondaryLeftFocused;
        private Visual rootVisual;
        private Compositor compositor;
        private InteractionTracker tracker;
        private VisualInteractionSource interactionSource;
        public event EventHandler DrawOpenedLeft;
        public event EventHandler DrawOpenedRight;
        public event EventHandler DrawsClosed;

        /// <summary>
        /// Intializes SideDrawer
        /// </summary>
        public SideDrawer()
        {
            this.InitializeComponent();

            // Handle Settings changing
            Storage.SettingsChangedHandler += App_LocalSettingsUpdatedHandler;

            // Link objects for easy access
            rootVisual = ElementCompositionPreview.GetElementVisual(maingrid);
            compositor = rootVisual.Compositor;  

            // Handle dragging
            App.UniversalPointerDownHandler += Content_PointerPressed;

            // Adjust for Cinematic View
            if (App.CinematicMode)
            {
                leftPanel.Margin = new Thickness(9, 0, 0, 0);
                contentRight.Margin = new Thickness(0, 0, 48, 0);
                rightSide.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
        }

        bool fullscreen = false;
        public void ToggleFullScreen()
        {
            if (!fullscreen)
            {
                SmallTrigger.MinWindowWidth = 0;
                MediumTrigger.MinWindowWidth = 100000;
                LargeTrigger.MinWindowWidth = 100000;
                ExtraLargeTrigger.MinWindowWidth = 100000;
                fullscreen = true;
            }
            else
            {
                SmallTrigger.MinWindowWidth = 100000;
                MediumTrigger.MinWindowWidth = 0;
                LargeTrigger.MinWindowWidth = 100000;
                ExtraLargeTrigger.MinWindowWidth = 100000;
                fullscreen = false;
            }
            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = VisualStateGroup.CurrentState });
        }

        private void App_LocalSettingsUpdatedHandler(object sender, EventArgs e)
        {
            MediumTrigger.MinWindowWidth = Storage.Settings.RespUiM;
            LargeTrigger.MinWindowWidth = Storage.Settings.RespUiL;
            ExtraLargeTrigger.MinWindowWidth = Storage.Settings.RespUiXl;
            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = VisualStateGroup.CurrentState });
        }

        //private InteractionTrackerInertiaModifier[] inertiaModifiers;

        private async void SmallInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Visible;
            leftSecondaryPanelCache.Visibility = Visibility.Visible;
            rightPanelCache.Visibility = Visibility.Visible;

            tracker.MaxPosition = new Vector3(width-72,0,0);
            tracker.MinPosition = new Vector3(-width, 0, 0);
            content.Margin = new Thickness(0, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            contentVis.StartAnimation("Translation.X", contentAnimation);
            if (contentVis2 != null)
                contentVis2.StartAnimation("Translation.X", contentAnimation);
            leftcache1.StartAnimation("Opacity", leftPanelFadeAnimation);
            Translate(leftpanel1, 0);
            leftpanel1.StartAnimation("Translation.X", leftPanelTranslateAnimation);
            leftcache2.StartAnimation("Opacity", leftPanelFadeAnimation2);
            Translate(leftpanel2, 0);
            leftpanel2.StartAnimation("Translation.X", leftPanelTranslateAnimation2);

            if (previous == Mid)
            {
                Translate(contentVis, 0, 72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, 72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
            SetSnapPoints(-width, 0, width - 72);
        }
        private async void MidInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Visible;
            rightPanelCache.Visibility = Visibility.Visible;

            tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            tracker.MinPosition = new Vector3(-width + 72, 0, 0);
            content.Margin = new Thickness(72, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            SetSnapPoints(-width + 72, 0, width - 72);

            contentVis.StartAnimation("Translation.X", contentAnimation);
            if (contentVis2 != null)
                contentVis2.StartAnimation("Translation.X", contentAnimation);
            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            //CloseLeft();
            if (previous == Small)
            {
                Translate(contentVis, 0, -72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, -72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
            if (previous == Large)
            {
                Translate(contentVis, 0, width - 72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, width - 72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
        }
        private async void LargeInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Collapsed;
            rightPanelCache.Visibility = Visibility.Visible;

            tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            tracker.MinPosition = new Vector3(0, 0, 0);
            content.Margin = new Thickness(width, 0, 0, 0);
            rightSide.Margin = new Thickness(0, 0, -72, 0);
            SetSnapPoints(0, 0, width - 72);

            contentVis.StartAnimation("Translation.X", contentAnimation);
            if (contentVis2 != null)
                contentVis2.StartAnimation("Translation.X", contentAnimation);
            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            if (previous == Small)
            {
                Translate(contentVis, 0, -width + 72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, -width + 72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
            if (previous == Mid)
            {
                Translate(contentVis, 0, -width + 72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, -width + 72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
        }
        private async void ExtraLargeInteractable(VisualState previous)
        {
            leftPanelCache.Visibility = Visibility.Collapsed;
            leftSecondaryPanelCache.Visibility = Visibility.Collapsed;
            rightPanelCache.Visibility = Visibility.Collapsed;

            tracker.MaxPosition = new Vector3(0, 0, 0);
            tracker.MinPosition = new Vector3(0, 0, 0);
            content.Margin = new Thickness(width, 0, width - 72, 0);
            rightSide.Margin = new Thickness(0, 0, 0, 0);
            SetSnapPoints(0, 0, 0);

            contentVis.StartAnimation("Translation.X", contentAnimation);
            if(contentVis2 != null)
                contentVis2.StartAnimation("Translation.X", contentAnimation);
            leftcache1.StopAnimation("Opacity");
            leftpanel1.StopAnimation("Translation.X");
            Translate(leftpanel1, 0);
            leftcache2.StartAnimation("Opacity", leftPanelFadeAnimation2);
            leftpanel2.StopAnimation("Translation.X");
            Translate(leftpanel2, 72);
            if (previous == Large)
            {
                Translate(contentVis, 0, width - 72);
                if (contentVis2 != null)
                    Translate(contentVis2, 0, width - 72);
                await Task.Delay(300);
                contentVis.StartAnimation("Translation.X", contentAnimation);
                if (contentVis2 != null)
                    contentVis2.StartAnimation("Translation.X", contentAnimation);
            }
        }

        private ExpressionAnimation contentAnimation;
        private ExpressionAnimation leftPanelFadeAnimation;
        private ExpressionAnimation leftPanelTranslateAnimation2;
        private ExpressionAnimation leftPanelFadeAnimation2;
        private void Translate(Visual visual, float to, float? from = null, bool RestartContentAnimationAfter = false)
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            // Create the Vector3 KFA
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.3);
            if (from.HasValue)
                kfa.InsertKeyFrame(0.0f, new Vector3(from.Value, 0, 0));
            kfa.InsertKeyFrame(1.0f, new Vector3(to, 0, 0), cubicBezier);
            // Update InteractionTracker position using this animation
            visual.StartAnimation("Translation", kfa);

        }
        public void SetupInteraction(UIElement DetachedHeader = null)
        {
            //Set up tracker
            var containerVisual = compositor.CreateContainerVisual();
            contentVis = ElementCompositionPreview.GetElementVisual(content);
            if(DetachedHeader != null)
                contentVis2 = ElementCompositionPreview.GetElementVisual(DetachedHeader);
            ElementCompositionPreview.SetIsTranslationEnabled(content, true);
            if(DetachedHeader != null)
                ElementCompositionPreview.SetIsTranslationEnabled(DetachedHeader, true);
            contentVis.Properties.InsertVector3("Translation", Vector3.Zero);
            if (DetachedHeader != null)
                contentVis2.Properties.InsertVector3("Translation", Vector3.Zero);

            interactionSource = VisualInteractionSource.Create(rootVisual);
            interactionSource.IsPositionXRailsEnabled = true;
            //If the chaining mode is set to always, it is impossible to go past by the minimum and maximum scrolling points by "stretching" the X axis
            interactionSource.PositionXChainingMode = InteractionChainingMode.Always;
            interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;


            tracker = InteractionTracker.Create(compositor);

            tracker.MaxPosition = new Vector3(width - 72, 0, 0);
            tracker.MinPosition = new Vector3(-width, 0, 0);
            tracker.TryUpdatePosition(new Vector3(0, 0, 0));
            tracker.InteractionSources.Add(interactionSource);


            // Define expression animations
            contentAnimation = compositor.CreateExpressionAnimation("-tracker.Position.X");
            contentAnimation.SetReferenceParameter("tracker", tracker);
            contentVis.StartAnimation("Translation.X", contentAnimation);
            if (DetachedHeader != null)
                contentVis2.StartAnimation("Translation.X", contentAnimation);
            //LEFT PANEL
            leftcache1 = ElementCompositionPreview.GetElementVisual(leftPanelCache);
            leftpanel1 = ElementCompositionPreview.GetElementVisual(leftPanel);
            ElementCompositionPreview.SetIsTranslationEnabled(leftPanel, true);

            leftPanelFadeAnimation = compositor.CreateExpressionAnimation("Clamp((-tracker.Position.X/width)*(-0.5)+0.25,0,1)");
            leftPanelFadeAnimation.SetScalarParameter("width", width);
            leftPanelFadeAnimation.SetReferenceParameter("tracker", tracker);
            leftcache1.StartAnimation("Opacity", leftPanelFadeAnimation);

            leftPanelTranslateAnimation = compositor.CreateExpressionAnimation("-24+((-tracker.Position.X/width)*24)");
            leftPanelTranslateAnimation.SetReferenceParameter("tracker", tracker);
            leftPanelTranslateAnimation.SetScalarParameter("width", width);
            leftpanel1.StartAnimation("Translation.X", leftPanelTranslateAnimation);

            //SECONDARY LEFT PANEL
            leftcache2 = ElementCompositionPreview.GetElementVisual(leftSecondaryPanelCache);
            leftpanel2 = ElementCompositionPreview.GetElementVisual(leftSecondaryPanel);
            ElementCompositionPreview.SetIsTranslationEnabled(leftSecondaryPanel, true);

            leftPanelFadeAnimation2 = compositor.CreateExpressionAnimation("Clamp((-tracker.Position.X/width)*(-0.25)+0.25, 0, 1)");
            leftPanelFadeAnimation2.SetScalarParameter("width", width);
            leftPanelFadeAnimation2.SetReferenceParameter("tracker", tracker);
            leftcache2.StartAnimation("Opacity", leftPanelFadeAnimation2);

            leftPanelTranslateAnimation2 = compositor.CreateExpressionAnimation("-tracker.Position.X/width*72");
            leftPanelTranslateAnimation2.SetReferenceParameter("tracker", tracker);
            leftPanelTranslateAnimation2.SetScalarParameter("width", width);
            leftpanel2.StartAnimation("Translation.X", leftPanelTranslateAnimation2);

            //RIGHT PANEL
            rightcache = ElementCompositionPreview.GetElementVisual(rightPanelCache);
            rightpanel = ElementCompositionPreview.GetElementVisual(rightSide);
            ElementCompositionPreview.SetIsTranslationEnabled(rightSide, true);

            rightPanelFadeAnimation = compositor.CreateExpressionAnimation("Clamp((tracker.Position.X/width)*(-0.25)+0.25, 0, 1)");
            rightPanelFadeAnimation.SetScalarParameter("width", width - 72);
            rightPanelFadeAnimation.SetReferenceParameter("tracker", tracker);
            rightcache.StartAnimation("Opacity", rightPanelFadeAnimation);

            rightPanelTranslateAnimation = compositor.CreateExpressionAnimation("-tracker.Position.X/width*72");
            rightPanelTranslateAnimation.SetReferenceParameter("tracker", tracker);
            rightPanelTranslateAnimation.SetScalarParameter("width", width - 72);
            rightpanel.StartAnimation("Translation.X", rightPanelTranslateAnimation);

            SetSnapPoints(-width, 0, width - 72);

            //UI Stuff
            MediumTrigger.MinWindowWidth = Storage.Settings.RespUiM;
            LargeTrigger.MinWindowWidth = Storage.Settings.RespUiL;
            ExtraLargeTrigger.MinWindowWidth = Storage.Settings.RespUiXl;
            var state = VisualStateGroup.CurrentState;

            //First, check for cinematic mode
            if (App.CinematicMode)
            {
                SmallTrigger.MinWindowWidth = 0;
                MediumTrigger.MinWindowWidth = 100000;
                LargeTrigger.MinWindowWidth = 100000;
                ExtraLargeTrigger.MinWindowWidth = 100000;
                maingrid.Margin = new Thickness(0, 0, 0, 0);
                ContentControl1.Margin = new Thickness(54, 0, 54, 0);
                leftSide.Margin = new Thickness(54, 0, 0, 0);
                leftPanel.Margin = new Thickness(12, 0, 0, 0);
                rightSide.Margin = new Thickness(-54, 0, 54, 0);
                width = 372;
            }


            VisualStateGroup_CurrentStateChanged(null, new VisualStateChangedEventArgs() { OldState = Small });
        }
        InteractionTrackerInertiaRestingValue startpoint;
        InteractionTrackerInertiaRestingValue midpoint;
        InteractionTrackerInertiaRestingValue endpoint;
        private void SetSnapPoints(float StartPoint, float MidPoint, float EndPoint)
        {
            tracker.PositionInertiaDecayRate = new Vector3(1f);
            var trackerTarget = ExpressionValues.Target.CreateInteractionTrackerTarget();

            startpoint = InteractionTrackerInertiaRestingValue.Create(compositor);
            startpoint.Condition = compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x < -halfwidth");
            startpoint.Condition.SetScalarParameter("halfwidth", width / 2);
            startpoint.RestingValue = compositor.CreateExpressionAnimation("pos");
            startpoint.RestingValue.SetScalarParameter("pos", StartPoint);

            midpoint = InteractionTrackerInertiaRestingValue.Create(compositor);
            midpoint.Condition = compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > -halfwidth && this.Target.NaturalRestingPosition.x < halfwidth");
            midpoint.Condition.SetScalarParameter("halfwidth", width / 2);
            midpoint.RestingValue = compositor.CreateExpressionAnimation("pos");
            midpoint.RestingValue.SetScalarParameter("pos", MidPoint);

            endpoint = InteractionTrackerInertiaRestingValue.Create(compositor);
            endpoint.Condition = compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.x > halfwidth");
            endpoint.Condition.SetScalarParameter("halfwidth", (EndPoint) / 2);
            endpoint.RestingValue = compositor.CreateExpressionAnimation("pos");
            endpoint.RestingValue.SetScalarParameter("pos", EndPoint);

            tracker.ConfigurePositionXInertiaModifiers(new InteractionTrackerInertiaModifier[] { startpoint, midpoint, endpoint });
        }
        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                try
                {
                    interactionSource.TryRedirectForManipulation(e.GetCurrentPoint(maingrid));
                }
                catch { }
            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            rootVisual = ElementCompositionPreview.GetElementVisual(maingrid);
            compositor = rootVisual.Compositor;
        }

        //bool animout = false;
        int width = 300;
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

        private Vector3 Vector3Zero = new Vector3(0);
        public void OpenLeft()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, tracker.MinPosition, cubicBezier);
            tracker.TryUpdatePositionWithAnimation(kfa);
        }

        public void CloseLeft()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, Vector3Zero, cubicBezier);
            tracker.TryUpdatePositionWithAnimation(kfa);
        }
        public void OpenRight()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, tracker.MaxPosition, cubicBezier);
            tracker.TryUpdatePositionWithAnimation(kfa);
        }
        public void CloseRight()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            kfa.InsertKeyFrame(1.0f, Vector3Zero, cubicBezier);
            tracker.TryUpdatePositionWithAnimation(kfa);
        }
        public void ToggleLeft()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            if (tracker.Position.X < 0)
            {
                kfa.InsertKeyFrame(1.0f, Vector3Zero, cubicBezier);
                DrawsClosed?.Invoke(null, null);
            }
            else
            {
                kfa.InsertKeyFrame(1.0f, tracker.MinPosition, cubicBezier);
                DrawOpenedLeft?.Invoke(null, null);
            }
                
            tracker.TryUpdatePositionWithAnimation(kfa);
        }
        public void ToggleRight()
        {
            CompositionEasingFunction cubicBezier = compositor.CreateCubicBezierEasingFunction(new Vector2(.45f, 1.5f), new Vector2(.45f, 1f));
            Vector3KeyFrameAnimation kfa = compositor.CreateVector3KeyFrameAnimation();
            kfa.Duration = TimeSpan.FromSeconds(0.5);
            if (tracker.Position.X > 0)
            {
                DrawsClosed?.Invoke(null, null);
                kfa.InsertKeyFrame(1.0f, Vector3Zero, cubicBezier);
            }
                
            else{
                DrawOpenedRight?.Invoke(null, null);
                kfa.InsertKeyFrame(1.0f, tracker.MaxPosition, cubicBezier);
            }
               
            tracker.TryUpdatePositionWithAnimation(kfa);
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (App.CinematicMode)
                SmallInteractable(e.OldState);
            else
            {
                var state = VisualStateGroup.CurrentState;
                if (state == Small)
                    SmallInteractable(e.OldState);
                else if (state == Mid)
                    MidInteractable(e.OldState);
                else if (state == Large)
                    LargeInteractable(e.OldState);
                else if (state == ExtraLarge)
                    ExtraLargeInteractable(e.OldState);
            }

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void contentLeft2_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {
           
        }

        private void contentLeft1_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {

        }

        private void contentRight_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {

        }

        private void ContentControl1_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {

        }

        private void contentLeft2_GotFocus(object sender, RoutedEventArgs e)
        {
            SecondaryLeftFocused?.Invoke(null, null);
        }

        public void Dispose()
        {
            Storage.SettingsChangedHandler -= App_LocalSettingsUpdatedHandler;
            App.UniversalPointerDownHandler -= Content_PointerPressed;
        }
    }
}
