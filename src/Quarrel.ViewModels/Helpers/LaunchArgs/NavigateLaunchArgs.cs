// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Helpers.LaunchArgs.Models;
using Quarrel.Messages.Navigation;
using System;
using System.Text.RegularExpressions;

namespace Quarrel.Helpers.LaunchArgs
{
    /// <summary>
    /// A class for launch args that navigate to a channel or guild on opening.
    /// </summary>
    public class NavigateLaunchArgs : LaunchArgsBase
    {
        /// <summary>
        /// A regular expression to determine if the launch args are a valid navigate argument.
        /// </summary>
        /// <remarks>
        /// Group 1 is the guildId
        /// Group 2 is the channelId
        /// </remarks>
        private const string NavigateArgsRegex = @"^(?:(?:(?:guild)|g)/([0-9]*)/?)?(?:(?:(?:channel)|c)/([0-9]*))?$";

        private NavigateLaunchArgs(ulong? guildId, ulong? channelId)
        {
            GuildId = guildId;
            ChannelId = channelId;
        }

        /// <summary>
        /// The guild id to navigate to.
        /// </summary>
        public ulong? GuildId { get; }

        /// <summary>
        /// The channel id to navigate to.
        /// </summary>
        public ulong? ChannelId { get; }

        /// <summary>
        /// Parses a <see cref="NavigateLaunchArgs"/> from a launch uri.
        /// </summary>
        public static new NavigateLaunchArgs? Parse(string args)
        {
            Match match = Regex.Match(args, NavigateArgsRegex);
            if (match.Success)
            {
                ulong? nullableGuildId = null;
                ulong? nullableChannelId = null;
                bool hasGuild = ulong.TryParse(match.Groups[1].Value, out ulong guildId);
                bool hasChannel = ulong.TryParse(match.Groups[2].Value, out ulong channelId);
                if (hasGuild) nullableGuildId = guildId;
                if (hasChannel) nullableChannelId = channelId;
                return new NavigateLaunchArgs(nullableGuildId, nullableChannelId);
            }

            return null;
        }

        /// <inheritdoc/>
        public override void RunPostLoad(IServiceProvider serviceProvider)
        {
            var messenger = serviceProvider.GetRequiredService<IMessenger>();
            if (GuildId.HasValue)
            {
                messenger.Send(new SelectGuildMessage<ulong>(GuildId.Value));
            }
        }
    }
}
