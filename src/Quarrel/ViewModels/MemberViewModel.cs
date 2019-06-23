using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Quarrel.Models.Bindables;
using UICompositionAnimations.Helpers;
using Quarrel.Helpers;
using Quarrel.Messages.Gateway;
using Quarrel.Services;
using DiscordAPI.Models;

namespace Quarrel.ViewModels
{
    public class MemberViewModel : ViewModelBase
    {
        public MemberViewModel()
        {
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    Source.Clear();

                    // Load guild list
                    var guildMemberList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableGuildMember>>(Constants.Cache.Keys.GuildMemberList, m.GuildId);

                    // Show members
                    foreach (var member in guildMemberList)
                    {
                        Source.Add(member);
                    }
                });
            });

            Source = new GroupedObservableCollection<Role, BindableGuildMember>(x => x.TopHoistRole);
            ViewSource = new CollectionViewSource() { Source = this.Source, IsSourceGrouped = true };
        }

        public CollectionViewSource ViewSource { get; }

        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public GroupedObservableCollection<Role, BindableGuildMember> Source { get; }
    }
}
