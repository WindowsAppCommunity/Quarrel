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
        public HashedCollection<string, BindableGuildMember> Users { get; } = new HashedCollection<string, BindableGuildMember>(new List<KeyValuePair<string, BindableGuildMember>>());

        public CurrentUsersService()
        {
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, m =>
            {

                // Show members
                Users.Clear();
                foreach (var member in m.Members)
                {
                    BindableGuildMember bGuildMember = new BindableGuildMember(member);
                    bGuildMember.GuildId = m.GuildId;
                    Users.Add(member.User.Id, bGuildMember);
                }

            });



        }
    }
}
