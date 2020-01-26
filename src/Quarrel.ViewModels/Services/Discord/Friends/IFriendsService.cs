using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    public interface IFriendsService
    {
        ConcurrentDictionary<string, BindableFriend> Friends { get; }
        ConcurrentDictionary<string, BindableGuildMember> DMUsers { get; }
    }
}
