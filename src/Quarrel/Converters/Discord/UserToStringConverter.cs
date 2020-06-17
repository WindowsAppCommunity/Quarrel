// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns the username#disc for a uer.
    /// </summary>
    public sealed class UserToStringConverter
    {
        /// <summary>
        /// Converts a user to a username#discriminator format.
        /// </summary>
        /// <param name="value">The user model.</param>
        /// <returns>The user string.</returns>
        public static string Convert(User value)
        {
            return string.Format("{0}#{1}", value.Username, value.Discriminator);
        }
    }
}
