using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.Linq;
using Quarrel.Models.Bindables;
using UICompositionAnimations.Helpers;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
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

                    // Show members
                    foreach (var member in m.Members)
                    {
                        BindableUser bUser = new BindableUser(member);
                        bUser.GuildId = m.GuildId;
                        Source.AddElement(bUser);
                    }
                });
            });

            Messenger.Default.Register<BindableUserRequestMessage>(this, m => m.ReportResult(Source.Elements.FirstOrDefault(x => x.Model.User.Id == m.UserId)));

            Messenger.Default.Register<CurrentMemberListRequestMessage>(this, m => m.ReportResult(Source.Elements.ToList()));

            Source = new GroupedObservableCollection<Role, BindableUser>(x => x.TopHoistRole);
        }

        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        public GroupedObservableCollection<Role, BindableUser> Source { get; set; }
    }
}
