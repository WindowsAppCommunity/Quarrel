// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Emojis;

namespace Quarrel.Client.Models.Emojis
{
    public class DiscordEmoji : Emoji
    {
        internal DiscordEmoji(JsonEmoji jsonEmoji) :
            base(jsonEmoji)
        {
            Guard.IsNotNull(jsonEmoji.Id, nameof(jsonEmoji.Id));

            Id = jsonEmoji.Id.Value;
            RequreColons = jsonEmoji.RequireColons ?? false;
            IsAnimated = jsonEmoji.IsAnimated ?? false;
        }

        public ulong Id { get; }

        public bool RequreColons { get; }

        public bool IsAnimated { get; }

        public string DisplayUrl
            => $"https://cdn.discordapp.com/emojis/{Id}{(IsAnimated ? ".gif" : ".png")}";
    }
}
