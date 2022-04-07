// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        private readonly IMessenger _messenger;

        public Shell()
        {
            this.InitializeComponent();
            _messenger = App.Current.Services.GetRequiredService<IMessenger>();

            _messenger.Register<NavigateToGuildMessage<BindableGuild>>(this, (_,_) => OpenLeft());
            _messenger.Register<NavigateToGuildMessage<Guild>>(this, (_,_) => OpenLeft());
        }

        private void OpenLeft()
        {
            Drawer.OpenLeft();
        }

        private void ToggleLeft(object sender, RoutedEventArgs e)
        {
            Drawer.ToggleLeft();
        }

        private void ToggleRight(object sender, RoutedEventArgs e)
        {
            Drawer.ToggleRight();
        }
    }
}
