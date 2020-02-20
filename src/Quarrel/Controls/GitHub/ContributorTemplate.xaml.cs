// Copyright (c) Quarrel. All rights reserved.

using GitHubAPI.Models;
using UICompositionAnimationsLegacy.Helpers.PointerEvents;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.GitHub
{
    /// <summary>
    /// Template shown for Contributors in CreditPage.
    /// </summary>
    public sealed partial class ContributorTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContributorTemplate"/> class.
        /// </summary>
        public ContributorTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
            this.ManageControlPointerStates(TogglePointerVisualStates);
        }

        /// <summary>
        /// Gets the contributor being shown by control.
        /// </summary>
        public Contributor ViewModel => DataContext as Contributor;

        /// <summary>
        /// Executes the necessary animations when the pointer goes over/out of the control.
        /// </summary>
        private void TogglePointerVisualStates(PointerDeviceType pointer, bool on)
        {
            if (pointer == PointerDeviceType.Mouse)
            {
                (on ? ShowHover : HideHover).Begin();
            }
        }
    }
}
