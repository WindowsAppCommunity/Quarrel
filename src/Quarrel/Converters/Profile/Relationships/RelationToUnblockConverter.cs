// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns true if the user has a blocked relation status.
    /// </summary>
    public sealed class RelationToUnblockConverter
    {
        /// <summary>
        /// Gets whether or not the user is blocked.
        /// </summary>
        /// <param name="value">The user's relation status.</param>
        /// <returns>Whether or not the user is blocked.</returns>
        public static bool Convert(int value)
        {
            return value == 2;
        }
    }
}
