// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns true if the user has no relation status.
    /// </summary>
    public sealed class RelationToAddFriendConverter
    {
        /// <summary>
        /// Gets whether or not the relation status is 0.
        /// </summary>
        /// <param name="value">The relation status.</param>
        /// <returns>Whether or not the relation status is 0</returns>
        public static bool Convert(int value)
        {
            return value == 0;
        }
    }
}
