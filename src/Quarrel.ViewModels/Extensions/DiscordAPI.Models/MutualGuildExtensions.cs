// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Guilds;

namespace DiscordAPI.Models
{
    /// <summary>
    /// Extensions for the <see cref="MutualGuild"/> class.
    /// </summary>
    internal static class MutualGuildExtensions
    {
        /// <summary>
        /// Gets the BindableGuild for the <see cref="MutualGuild"/>.
        /// </summary>
        /// <param name="mg">The <see cref="MutualGuild"/>.</param>
        /// <returns>The <see cref="BindableGuild"/> for the <see cref="MutualGuild"/>.</returns>
        public static BindableGuild Guild(this MutualGuild mg) => SimpleIoc.Default.GetInstance<IGuildsService>().AllGuilds.TryGetValue(mg.Id, out var value) ? value : null;

        /// <summary>
        /// Gets the guild's name of the <see cref="MutualGuild"/>.
        /// </summary>
        /// <param name="mg">The <see cref="MutualGuild"/>.</param>
        /// <returns>The name of the Guild for <paramref name="mg"/>.</returns>
        public static string GetName(this MutualGuild mg) => Guild(mg)?.Model?.Name;

        /// <summary>
        /// Gets the icon url of the <see cref="MutualGuild"/>.
        /// </summary>
        /// <param name="mg">The <see cref="MutualGuild"/>.</param>
        /// <returns>The icon url of the Guild for <paramref name="mg"/>.</returns>
        public static string GetIconUrl(this MutualGuild mg)
        {
            return Guild(mg)?.IconUrl;
        }
    }
}