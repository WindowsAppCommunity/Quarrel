using GitHubAPI.Models;
using Windows.UI.Xaml.Controls;
using UICompositionAnimationsLegacy.Helpers.PointerEvents;
using Windows.Devices.Input;

namespace Quarrel.Controls.GitHub
{
    public sealed partial class ContributorTemplate : UserControl
    {
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
        /// Executes the necessary animations when the pointer goes over/out of the control
        /// </summary>
        private void TogglePointerVisualStates(PointerDeviceType pointer, bool on)
        {
            if (pointer == PointerDeviceType.Mouse) (on ? ShowHover : HideHover).Begin();
        }

        public Contributor ViewModel => DataContext as Contributor;
    }
}
