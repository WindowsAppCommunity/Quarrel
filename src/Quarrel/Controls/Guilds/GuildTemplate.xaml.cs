using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Guilds
{
    /// <summary>
    /// Template for Guild Item in Guild List
    /// </summary>
    public sealed partial class GuildTemplate : UserControl
    {
        public GuildTemplate()
        {
            this.InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Guild represented
        /// </summary>
        public BindableGuild ViewModel => DataContext as BindableGuild;
    }
}
