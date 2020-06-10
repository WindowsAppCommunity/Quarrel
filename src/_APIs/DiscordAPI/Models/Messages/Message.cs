// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Messages.Embeds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordAPI.Models.Messages
{
    /// <summary>
    /// A model for a message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the id of the message.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the id of message's channel.
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the Activity in the message.
        /// </summary>
        [JsonProperty("activity")]
        public Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets the author of the message.
        /// </summary>
        [JsonProperty("author")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the call referenced in the message.
        /// </summary>
        [JsonProperty("call")]
        public Call Call { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the message.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the timestamp the message was edited.
        /// </summary>
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message was to be read Text To Speech.
        /// </summary>
        [JsonProperty("tts")]
        public bool TTS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not message mentions @everyone.
        /// </summary>
        [JsonProperty("mention_everyone")]
        public bool MentionEveryone { get; set; }

        /// <summary>
        /// Gets or sets a list of the users mentioned in the message.
        /// </summary>
        [JsonProperty("mentions")]
        public IEnumerable<User> Mentions { get; set; }

        /// <summary>
        /// Gets or sets a list of the ids of the roles mentioned in the message.
        /// </summary>
        [JsonProperty("mention_roles")]
        public IEnumerable<string> MentionRoles { get; set; }

        /// <summary>
        /// Gets or sets a list of ChannelMentions.
        /// </summary>
        [JsonProperty("mention_channels")]
        public IEnumerable<ChannelMention> MentionChannels { get; set; }

        /// <summary>
        /// Gets or sets a list of the attachments in the message.
        /// </summary>
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets a list of the embeds in the message.
        /// </summary>
        [JsonProperty("embeds")]
        public IEnumerable<Embed> Embeds { get; set; }

        /// <summary>
        /// Gets or sets a list of the reactions to the message.
        /// </summary>
        [JsonProperty("reactions")]
        public IEnumerable<Reaction> Reactions { get; set; }

        /// <summary>
        /// Gets or sets the nonce of the message.
        /// </summary>
        [JsonProperty("nonce")]
        public long? Nonce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is pinned in the channel.
        /// </summary>
        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        /// <summary>
        /// Gets or sets the type of message.
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the webhook id for the message.
        /// </summary>
        [JsonProperty("webhook_id")]
        public string WebHookid { get; set; }

        /// <summary>
        /// Gets or sets the id of guild the message is in.
        /// </summary>
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
