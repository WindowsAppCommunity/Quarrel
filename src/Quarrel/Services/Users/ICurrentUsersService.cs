using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Users
{
    public interface ICurrentUsersService
    {
        GroupedObservableHashedCollection<string, Role, BindableUser> Users { get; set; }
    }
}
