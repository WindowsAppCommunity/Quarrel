// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns a bool if the FriendStatus is Incoming pending.
    /// </summary>
    public sealed class RelationToAcceptFriendConverter
    {
        /// <summary>
        /// Converts the friend status to a bool indicating if incoming pending.
        /// </summary>
        /// <param name="value">The friend status.</param>
        /// <returns>Whether or not to show accept friend button.</returns>
        public static bool Convert(int value)
        {
            return value == 3;
        }
    }
}
