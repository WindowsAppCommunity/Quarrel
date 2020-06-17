// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns true if the user is not or current user.
    /// </summary>
    public sealed class RelationToSendMessageConverter
    {
        /// <summary>
        /// Gets whether or not the user is the current user.
        /// </summary>
        /// <param name="value">The user's relation status.</param>
        /// <returns>Whether or not the user is the current user.</returns>
        public static bool Convert(int value)
        {
            return value != -1;
        }
    }
}
