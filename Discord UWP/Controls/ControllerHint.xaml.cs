using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class ControllerHint : UserControl
    {
        /// <summary>
        /// Name of key
        /// </summary>
        public string Label
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        /// <summary>
        /// Icon of key
        /// </summary>
        public string ButtonGlyph
        {
            get { return glyph.Glyph; }
            set { glyph.Glyph = value; }
        }

        private Windows.System.VirtualKey key;
        /// <summary>
        /// Vitual Key it's bound to
        /// </summary>
        public Windows.System.VirtualKey Key
        {
            get { return Key; }
            set { key = value; }
        }

        public ControllerHint()
        {
            this.InitializeComponent();
            if (!App.CinematicMode)
                Visibility = Visibility.Collapsed;
        }

        List<AnimationSet> animations = new List<AnimationSet>();
        AnimationSet anim;
        AnimationSet anim2;
        AnimationSet anim3;
        AnimationSet anim4;
        
        /// <summary>
        /// Run when the user presses bound Controller Butto 
        /// </summary>
        public void Press()
        {
            foreach (var animation in animations)
                animation.Stop();
            animations.Clear();

            animations.Add(glow.Blur(3, 300));
            animations.Add(normal.Fade(0.8f, 200));
            animations.Add(glow.Fade(1, 200));
            animations.Add(glyph.Scale(1.2f, 1.2f, 12, 12, 200));
            animations.Add(label.Offset(2,0,200));
            animations.Add(glyphGlow.Scale(1.2f, 1.2f, 12, 12, 200));
            animations.Add(labelGlow.Offset(2, 0, 200));

            foreach (var animation in animations)
                animation.Start();
        }

        /// <summary>
        /// Run when the user released bound Controller Button
        /// </summary>
        public void Release()
        {
            foreach (var animation in animations)
                animation.Stop();
            animations.Clear();

            animations.Add(glow.Blur(0, 300));
            animations.Add(normal.Fade(0.6f, 200));
            animations.Add(glow.Fade(0, 200));
            animations.Add(glyph.Scale(1, 1, 12, 12, 200));
            animations.Add(label.Offset(0, 0, 200));
            animations.Add(glyphGlow.Scale(1, 1, 12, 12, 200));
            animations.Add(labelGlow.Offset(0, 0, 200));

            foreach (var animation in animations)
                animation.Start();
        }
        
        /// <summary>
        /// Fade in hint
        /// </summary>
        public void Show()
        {
            foreach (var animation in animations)
                animation.Stop();
            animations.Clear();

            animations.Add((this).Fade(1, 200));

            foreach (var animation in animations)
                animation.Start();

        }

        /// <summary>
        /// Fade out hint
        /// </summary>
        public void Hide()
        {
            foreach (var animation in animations)
                animation.Stop();
            animations.Clear();

            animations.Add((this).Fade(0, 200));

            foreach (var animation in animations)
                animation.Start();
        }

        public void Dispose()
        {
            //Nothing to dispose
        }

        // Simulate button being tapped when the UserControl is tapped
        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.HandleKeyPress(key);
        }
    }
}
