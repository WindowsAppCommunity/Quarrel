// Adam Dernis © 2022

using Discord.API.Models.Guilds.Interfaces;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Services.Discord;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableUnqiueItemBase<IGuild>
    {
        public BindableGuild(IDiscordService discordService, IGuild model) : base(discordService, model)
        {
        }

        public override async void UpdateFromServiceAsync()
        {
        }
    }
}
