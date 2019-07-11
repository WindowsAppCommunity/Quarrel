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
        public HashedCollection<string, BindableUser> Users { get; } = new HashedCollection<string, BindableUser>(new List<KeyValuePair<string, BindableUser>>());

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
                    Users.Add(member.User.Id, bUser);
                }

            });


        }
    }
}
