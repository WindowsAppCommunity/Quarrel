// Quarrel © 2022

using Discord.API.Models.Json.Reactions;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Emojis;

namespace Quarrel.Client.Models.Messages
{
    /// <summary>
    /// A reaction managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class Reaction : DiscordItem
    {
        internal Reaction(JsonReaction jsonReaction, Message message, QuarrelClient context) :
            base(context)
        {
            Me = jsonReaction.Me;
            Emoji = Emoji.Create(jsonReaction.Emoji);
            Count = jsonReaction.Count;
            Message = message;
        }

        /// <summary>
        /// Gets the emoji on display.
        /// </summary>
        public Emoji Emoji { get; }

        /// <summary>
        /// Gets whether or not the current user has reacted.
        /// </summary>
        public bool Me { get; }

        /// <summary>
        /// Gets the number of people that reacted with this reaction.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the message the reaction belongs to.
        /// </summary>
        public Message Message { get; }
    }
}
