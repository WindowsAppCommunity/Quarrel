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

namespace Quarrel.Controls.Shell
{
    public sealed partial class QuarrelCommandBar : CommandBar
    {
        public QuarrelCommandBar()
        {
            this.InitializeComponent();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
            {
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, new OpenDownCommandBarVisualStateManager());
            }
        }

        public event EventHandler HamburgerClicked;
        private void ToggleSplitView(object sender, RoutedEventArgs e)
        {
            HamburgerClicked(this, null);
        }

        public event EventHandler MemberListButtonClicked;
        private void ToggleMemberPane(object sender, RoutedEventArgs e)
        {
            MemberListButtonClicked(this, null);
        }
    }

    public class OpenDownCommandBarVisualStateManager : VisualStateManager
    {
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            //replace OpenUp state change with OpenDown one and continue as normal
            if (!string.IsNullOrWhiteSpace(stateName) && stateName.EndsWith("OpenUp"))
            {
                stateName = stateName.Substring(0, stateName.Length - 6) + "OpenDown";
            }
            return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
        }
    }
}
