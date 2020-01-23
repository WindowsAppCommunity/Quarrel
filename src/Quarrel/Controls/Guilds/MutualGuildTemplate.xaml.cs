using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Guilds
{

    /// <summary>
    /// Template to show a Guild mutual to a user
    /// </summary>
    public sealed partial class MutualGuildTemplate : UserControl
    {
        public MutualGuildTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Mutual Guild Data in template
        /// </summary>
        BindableMutualGuild ViewModel => DataContext as BindableMutualGuild;
    }
}
