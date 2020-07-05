// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables.Users
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Group"/> model.
    /// </summary>
    public class BindableGuildMemberGroup : BindableModelBase<Group>, IGuildMemberListItem
    {
        private IGuildsService _guildsService = null;
        private string _guildId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableGuildMemberGroup"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="Group"/> object.</param>
        public BindableGuildMemberGroup([NotNull] Group model, string guildId) : base(model)
        {
            _guildId = guildId;
        }

        /// <summary>
        /// Gets the number of members in the group.
        /// </summary>
        public int Count => Model.Count;

        /// <summary>
        /// Gets the role of the group.
        /// </summary>
        public Role Role
        {
            get
            {
                BindableGuild guild = GuildsService.GetGuild(_guildId);
                if (guild != null)
                {
                    return guild.Model.Roles?.FirstOrDefault(x => x.Id == Model.Id);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name
        {
            get
            {
                switch (Model.Id)
                {
                    case "online":
                        return "Online";
                    case "offline":
                        return "Offline";
                    default:
                        return Role?.Name;
                }
            }
        }

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());
    }
}
