// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Models.Emojis
{
    /// <summary>
    /// Bindable ViewModel for Guild Emojis.
    /// </summary>
    public class GuildEmoji : Emoji
    {
        private readonly string _id;
        private IGuildsService _guildsService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildEmoji"/> class.
        /// </summary>
        /// <param name="emoji">The base model emoji.</param>
        public GuildEmoji(DiscordAPI.Models.Emoji emoji)
        {
            _id = emoji.Id;
            Category = GuildsService.CurrentGuild.Model.Name; // TODO: External
            Names = new List<string>() { emoji.Name };
            Preview = emoji.DisplayUrl;
        }

        /// <inheritdoc/>
        public override string Id { get => _id; }

        /// <summary>
        /// Gets or sets the guild name.
        /// </summary>
        public override string Category { get; set; }

        /// <inheritdoc/>
        public override bool CustomEmoji => true;

        /// <inheritdoc/>
        public override string Surrogate => string.Format(":{0}:", Names[0]);

        /// <summary>
        /// Gets a value indicating whether or not the user has permissions to use emoji or can't (nitro).
        /// </summary>
        public bool IsEnabled { get; private set; }

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());
    }
}
