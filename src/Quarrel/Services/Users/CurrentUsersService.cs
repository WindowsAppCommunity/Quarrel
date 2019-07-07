using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Users
{
    public class CurrentUsersService : ICurrentUsersService
    {
        public GroupedObservableHashedCollection<string, Role, BindableUser> Users { get; set; } =
            new GroupedObservableHashedCollection<string, Role, BindableUser>(x => x.TopHoistRole,
                new List<KeyValuePair<string, HashedGrouping<string, Role, BindableUser>>>());

        public CurrentUsersService()
        {
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, m =>
            {

                // Show members
                Users.Clear();
                foreach (var member in m.Members)
                {
                    BindableUser bUser = new BindableUser(member);
                    bUser.GuildId = m.GuildId;
                    Users.AddElement(member.User.Id, bUser);
                }

            });

            Messenger.Default.Register<BindableUserRequestMessage>(this, m => m.ReportResult(Users.ContainsKey(m.UserId) ? Users[m.UserId][m.UserId] : default));

            Messenger.Default.Register<CurrentMemberListRequestMessage>(this, m => m.ReportResult(Users.Elements.ToList()));

        }
    }
}
