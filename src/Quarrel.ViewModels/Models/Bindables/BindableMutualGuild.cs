using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Discord.Guilds;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// Mutual Guild that can be bound to the UI
    /// </summary>
    public class BindableMutualGuild : BindableModelBase<MutualGuild>
    {
        public IGuildsService GuildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        public BindableMutualGuild(MutualGuild mg) : base(mg)
        {}

        /// <summary>
        /// Guild represented in full
        /// </summary>
        public BindableGuild BindableGuild => GuildsService.AllGuilds.TryGetValue(Model.Id, out var value) ? value : null;
    }
}
