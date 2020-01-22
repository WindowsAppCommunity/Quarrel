using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Channels
{
    /// <summary>
    /// Chevron to indicate collapsed status of Category in ChannelTemplate
    /// </summary>
    public sealed partial class CategoryChveron : UserControl
    {
        public CategoryChveron()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// true when collapsed
        /// </summary>
        /// <remarks> Rotates FontIcon -90 when collapsed </remarks>
        public bool IsCollapsed
        {
            get => _IsCollapsed;
            set
            {
                if (value)
                    Chevron.Rotate(-90, 7, 7, 400, 0, EasingType.Circle).Start();
                else
                    Chevron.Rotate(0, 7, 7, 400, 0, EasingType.Circle).Start();

                _IsCollapsed = value;
            }
        }
        private bool _IsCollapsed;
    }
}
