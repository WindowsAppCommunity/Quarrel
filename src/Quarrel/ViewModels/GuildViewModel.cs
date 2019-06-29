using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Quarrel.Helpers;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.Services;
using Quarrel.Services.Cache;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using UICompositionAnimations.Helpers;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using Quarrel.Messages.Navigation;

namespace Quarrel.ViewModels
{
    public class GuildViewModel : ViewModelBase
    {
        public GuildViewModel()
        {
            Messenger.Default.Register<GatewayReadyMessage>(this, async _ =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    // Load guild list
                    var guildList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableGuild>>(Constants.Cache.Keys.GuildList);

                    // Show guilds
                    foreach (var guild in guildList)
                    {
                        Source.Add(guild);
                    }
                });
            });

            ViewSource = new CollectionViewSource() { Source = this.Source };
        }

        public CollectionViewSource ViewSource { get; }

        public ObservableCollection<BindableGuild> Source { get; private set; } = new ObservableCollection<BindableGuild>();

        private RelayCommand<BindableGuild> navigateGuildCommand;

        public RelayCommand<BindableGuild> NavigateGuildCommand => navigateGuildCommand ?? (navigateGuildCommand = new RelayCommand<BindableGuild>((guild) =>
        {
            Messenger.Default.Send(new GuildNavigateMessage(guild.Model.Id));
        }));
    }
}
