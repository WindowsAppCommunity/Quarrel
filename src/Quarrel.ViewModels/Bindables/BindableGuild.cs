// Adam Dernis © 2022

using Discord;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Services.Discord;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableDiscordItemBase<IGuild>
    {
        public BindableGuild(IDiscordService discordService, IGuild model) : base(discordService, model)
        {
        }

        public override async void UpdateFromServiceAsync()
        {
            var guild = await DiscordService.DiscordClient.GetGuildAsync(Model.Id);
        }
    }
}
