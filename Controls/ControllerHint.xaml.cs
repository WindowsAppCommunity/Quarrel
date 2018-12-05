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

namespace Discord_UWP.Controls
{
    public sealed partial class ControllerHint : UserControl
    {
        public string ButtonGlyph
        {
            get { return glyph.Glyph; }
            set { glyph.Glyph = value; }
        }
        public string Label
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        private Windows.System.VirtualKey key;
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
        public void Show()
        {
            foreach (var animation in animations)
                animation.Stop();
            animations.Clear();

            animations.Add((this).Fade(1, 200));

            foreach (var animation in animations)
                animation.Start();

        }
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

        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.HandleKeyPress(key);
        }
    }
}
