// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns true if the user has a friend relation status.
    /// </summary>
    public sealed class RelationToRemoveFriendConverter
    {
        /// <summary>
        /// Gets whether or not the user is a friend.
        /// </summary>
        /// <param name="value">The user's relation status.</param>
        /// <returns>Whether or not the user is a friend.</returns>
        public static bool Convert(int value)
        {
            return value == 1;
        }
    }
}
