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
        Dictionary<string, BindableGuildMember> Users { get; }
        Dictionary<string, BindableGuildMember> DMUsers { get; }

        BindableUser CurrentUser { get; }

        /// <summary>
        /// Imporant, do not bind to
        /// </summary>
        BindableGuildMember CurrentGuildMember { get; }
    }
}
