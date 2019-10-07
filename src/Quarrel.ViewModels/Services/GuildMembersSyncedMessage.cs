using System;
using System.Collections.Generic;
using System.Text;
using Quarrel.Models.Bindables;

namespace Quarrel.ViewModels.Services
{
    public class GuildMembersSyncedMessage
    {
        public List<BindableGuildMember> Members { get; }
        public GuildMembersSyncedMessage(List<BindableGuildMember> members)
        {
            Members = members;
        }
    }
}
