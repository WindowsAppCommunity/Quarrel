// Quarrel © 2022

using Discord.API.Models.Json.Emojis;

namespace Quarrel.Client.Models.Emojis
{
    public class Emoji
    {
        internal Emoji(JsonEmoji jsonEmoji)
        {
            Name = jsonEmoji.Name;
        }

        /// <summary>
        /// Gets the name of the emoji.
        /// </summary>
        /// <remarks>
        /// If the emoji is not a discord emoji, this is just the emoji.
        /// </remarks>
        public string Name { get; }

        internal static Emoji Create(JsonEmoji jsonEmoji)
        {
            return jsonEmoji.Id.HasValue ? new DiscordEmoji(jsonEmoji) : new Emoji(jsonEmoji);
        }
    }
}
