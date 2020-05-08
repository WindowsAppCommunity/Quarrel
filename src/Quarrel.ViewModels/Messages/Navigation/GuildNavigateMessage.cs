// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;

namespace Quarrel.ViewModels.Messages.Navigation
{
    /// <summary>
    /// A message to request switching guilds.
    /// </summary>
    public sealed class GuildNavigateMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildNavigateMessage"/> class.
        /// </summary>
        /// <param name="guild">The guild to navigate to.</param>
        public GuildNavigateMessage(BindableGuild guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets the guild to navigate to.
        /// </summary>
        public BindableGuild Guild { get; }
    }
}
