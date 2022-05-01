// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.API.Models.Enums.Messages
{
    public enum MessageFlags
    {
        /// <summary>
        /// This message has been published to subscribed channels (via Channel Following)
        /// </summary>
        CROSSPOSTED = 0,

        /// <summary>
        /// This message originated from a message in another channel (via Channel Following)
        /// </summary>
        IS_CROSSPOST = 1,

        /// <summary>
        /// Do not include any embeds when serializing this message
        /// </summary>
        SUPPRESS_EMBEDS = 1 << 2,

        /// <summary>
        /// The source message for this crosspost has been deleted (via Channel Following)
        /// </summary>
        SOURCE_MESSAGE_DELETED = 1 << 3,

        /// <summary>
        /// This message came from the urgent message system
        /// </summary>
        URGENT = 1 << 4,

        /// <summary>
        /// This message has an associated thread, with the same id as the message
        /// </summary>
        HAS_THREAD = 1 << 5,

        /// <summary>
        /// This message is only visible to the user who invoked the Interaction
        /// </summary>
        EPHEMERAL = 1 << 6,

        /// <summary>
        /// This message is an Interaction Response and the bot is "thinking"
        /// </summary>
        LOADING = 1 << 7,

        /// <summary>
        /// This message failed to mention some roles and add their members to the thread
        /// </summary>
        FAILED_TO_MENTION_SOME_ROLES_IN_THREAD = 1 << 8,
    }

}
