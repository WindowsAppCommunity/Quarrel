using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscordAPI.API.User;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Services.Users;

namespace Quarrel.ViewModels
{
    public class MemberViewModel : ViewModelBase
    {

        public MemberViewModel()
        {
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, async m =>
            {
                var tempSource = new SortedGroupedObservableHashedCollection<string, Role, BindableUser>(x => x.TopHoistRole, x => -x.Key.Position, new List<KeyValuePair<string, HashedGrouping<string, Role, BindableUser>>>());

                // Show members
                foreach (var member in m.Members)
                {
                    BindableUser bUser = new BindableUser(member);
                    bUser.GuildId = m.GuildId;
                    tempSource.AddElement(member.User.Id, bUser);
                }

                await DispatcherHelper.RunAsync(() => { Source = tempSource; RaisePropertyChanged(nameof(Source)); });
            });


            Source = new SortedGroupedObservableHashedCollection<string, Role, BindableUser>(x => x.TopHoistRole, x => -x.Key.Position, new List<KeyValuePair<string, HashedGrouping<string, Role, BindableUser>>>());
        }

        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public SortedGroupedObservableHashedCollection<string, Role, BindableUser> Source { get; set; }
    }
}
