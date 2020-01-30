using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle GuildList
    /// </summary>
    public sealed partial class GuildListControl : UserControl
    {
        public GuildListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
