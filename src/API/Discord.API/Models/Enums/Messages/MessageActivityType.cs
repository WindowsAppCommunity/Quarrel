// Quarrel © 2022

namespace Discord.API.Models.Enums.Messages
{
    /// <summary>
    /// The type of action for an activity message.
    /// </summary>
    public enum MessageActivityType
    {
        /// <summary>
        /// Join a game.
        /// </summary>
        Join = 1,

        /// <summary>
        /// Spectate a game.
        /// </summary>
        Spectate = 2,

        /// <summary>
        /// Unsure.
        /// </summary>
        Listen = 3,

        /// <summary>
        /// Join a group request.
        /// </summary>
        JoinRequest = 5
    }
}
