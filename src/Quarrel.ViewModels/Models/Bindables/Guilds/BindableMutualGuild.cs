// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Discord.Guilds;

namespace Quarrel.ViewModels.Models.Bindables.Guilds
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="MutualGuild"/> model.
    /// </summary>
    public class BindableMutualGuild : BindableModelBase<MutualGuild>
    {
        private IGuildsService _guildsService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMutualGuild"/> class.
        /// </summary>
        /// <param name="mg">The base <see cref="MutualGuild"/> object.</param>
        public BindableMutualGuild(MutualGuild mg) : base(mg)
        {
        }

        /// <summary>
        /// Gets the full guild data.
        /// </summary>
        public BindableGuild BindableGuild => GuildsService.GetGuild(Model.Id);

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());
    }
}
