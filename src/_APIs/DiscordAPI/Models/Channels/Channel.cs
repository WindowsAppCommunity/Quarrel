// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Channels
{
    /// <summary>
    /// The model for any channel.
    /// </summary>
    public abstract class Channel
    {
        /// <summary>
        /// Gets or sets the id of the channel.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        public virtual string Name
        {
            get { return RawName; }
            set { RawName = value; }
        }

        /// <summary>
        /// Gets or sets the type of channel.
        /// </summary>
        /// <remarks>
        /// 0 = Text,
        /// 1 = DM,
        /// 2 = Voice,
        /// 3 = Group DM,
        /// 4 = Category.
        /// </remarks>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the id of the last message in the channel.
        /// </summary>
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }

        /// <summary>
        /// Gets or sets the raw name if the channel.
        /// </summary>
        [JsonProperty("name")]
        internal string RawName { get; set; }

        /// <summary>
        /// Sets the <see cref="LastMessageId"/> of the channel.
        /// </summary>
        /// <param name="id">The new message id.</param>
        public void UpdateLMID(string id)
        {
            LastMessageId = id;
        }
    }
}
