// Quarrel © 2022

namespace Discord.API.Models.Enums.Users
{
    /// <summary>
    /// The relationship with a user.
    /// </summary>
    public enum RelationshipType
    {
        /// <summary>
        /// No relationship to user.
        /// </summary>
        None,

        /// <summary>
        /// The user is a friend.
        /// </summary>
        Friend,

        /// <summary>
        /// The user is blocked.
        /// </summary>
        Blocked,

        /// <summary>
        /// The user has requested to be friends with the current user.
        /// </summary>
        IncomingFriendRequest,

        /// <summary>
        /// Current user has requested to be friends with the user.
        /// </summary>
        OutgoingFriendRequest,

        /// <summary>
        /// The users have no relationship but interact a lot.
        /// </summary>
        Implicit,
    }
}
