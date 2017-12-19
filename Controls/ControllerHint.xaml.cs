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
        public ControllerHint()
        {
            this.InitializeComponent();
        }
        AnimationSet anim;
        AnimationSet anim2;
        AnimationSet anim3;
        public void Press()
        {
            if(anim != null)
            {
                anim.Stop();
                anim2.Stop();
                anim3.Stop();
            }
            anim = glow.Blur(3, 300);
            anim2 = normal.Fade(0.8f, 300);
            anim3 = glow.Fade(1, 300);
            anim.Start();
            anim2.Start();
            anim3.Start();
        }
        public void Release()
        {
            if(anim != null)
            {
                anim.Stop();
                anim2.Stop();
                anim3.Stop();
            }
            anim = glow.Blur(0, 300);
            anim2 = normal.Fade(0.6f, 300);
            anim3 = glow.Fade(0, 300);
            anim.Start();
            anim2.Start();
            anim3.Start();
        }
    }
}
