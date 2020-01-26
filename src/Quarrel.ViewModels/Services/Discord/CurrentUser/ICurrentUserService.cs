using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.CurrentUser
{
    public interface ICurrentUserService
    {
        BindableUser CurrentUser { get; }

        UserSettings CurrentUserSettings { get; }
    }
}
