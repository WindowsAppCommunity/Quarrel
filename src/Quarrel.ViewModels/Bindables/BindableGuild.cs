// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Quarrel.Models.Bindables.Abstract;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableUnqiueItemBase<Guild>
    {
        public BindableGuild(Guild model) : base(model)
        {
        }

        public override async void UpdateFromServiceAsync()
        {
        }
    }
}
