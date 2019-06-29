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
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services;
using DiscordAPI.Models;
using Quarrel.Converters.Base;

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
                    var guildMemberList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableUser>>(Constants.Cache.Keys.GuildMemberList, m.GuildId);

                    // Show members
                    //TODO: much better way of doing this
                    foreach (var member in guildMemberList)
                    {
                        bool added = false;
                        for (int i = 0; i < Source.Count; i++)
                        {
                            if (Source[i].Key.Equals(member.TopHoistRole))
                            {
                                added = true;
                                Source[i] = new Grouping<Role, BindableUser>(member.TopHoistRole, Source[i].Concat(new []{member}));
                            }
                        }

                        if (!added)
                        {
                            Source.Add(new Grouping<Role, BindableUser>(member.TopHoistRole, new [] {member}));
                        }
                    }

                });
            });

          //  Messenger.Default.Register<BindableUserRequestMessage>(this, m => m.ReportResult(Source.FirstOrDefault(x => x.Model.User.Id == m.UserId)));

        }


        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public ObservableCollection<IGrouping<Role, BindableUser>> Source { get; set; } = new ObservableCollection<IGrouping<Role, BindableUser>>();
    }
}
