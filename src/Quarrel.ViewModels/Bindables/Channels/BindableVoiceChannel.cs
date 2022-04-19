﻿// Quarrel © 2022

using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Users;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="VoiceChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableVoiceChannel : BindableGuildChannel
    {
        internal BindableVoiceChannel(VoiceChannel channel, GuildMember selfMember, BindableCategoryChannel? parent = null) :
            base(channel, selfMember, parent)
        {
        }
        
        /// <inheritdoc/>
        public override bool IsTextChannel => false;

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.Connect;
    }
}