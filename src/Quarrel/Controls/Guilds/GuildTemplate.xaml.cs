using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Guilds
{
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

        public BindableGuild ViewModel => DataContext as BindableGuild;
    }
}
