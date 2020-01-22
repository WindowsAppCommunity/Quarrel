using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Guild;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Guilds
{
    public class BindableMutualGuild
    {
        public IGuildsService GuildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        public BindableMutualGuild(MutualGuild mg)
        {
            MutualGuild = mg;
        }

        public MutualGuild MutualGuild { get; set; }

        public BindableGuild BindableGuild => GuildsService.Guilds.TryGetValue(MutualGuild.Id, out var value) ? value : null;
    }

    public sealed partial class MutualGuildTemplate : UserControl
    {
        public MutualGuildTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if (e.NewValue is MutualGuild guild)
                    DataContext = new BindableMutualGuild(guild);
                else
                    this.Bindings.Update();
            };
        }

        BindableMutualGuild ViewModel => DataContext as BindableMutualGuild;
    }
}
