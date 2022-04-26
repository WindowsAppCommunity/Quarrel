// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Discord.API.Models.Json.Messages.Embeds;
using Discord.API.Models.Json.Reactions;
using Discord.API.Models.Json.Users;
using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal class JsonMessage
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public MessageType Type { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("webhook_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? WebhookId { get; set; }

        [JsonPropertyName("author")]
        public JsonUser? Author { get; set; }

        [JsonPropertyName("call")]
        public JsonCall? Call { get; set; }

        [JsonPropertyName("member")]
        public JsonGuildMember? Member { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonPropertyName("edited_timestamp")]
        public DateTimeOffset? EditedTimestamp { get; set; }

        [JsonPropertyName("tts")]
        public bool? IsTextToSpeech { get; set; }

        [JsonPropertyName("mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonPropertyName("mentions")]
        public JsonUser[]? UserMentions { get; set; }

        [JsonPropertyName("mention_roles"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[]? RoleMentions { get; set; }

        [JsonPropertyName("mention_channels")]
        public JsonChannelMention[]? ChannelMentions { get; set; }

        [JsonPropertyName("attachments")]
        public JsonAttachment[]? Attachments { get; set; }

        [JsonPropertyName("embeds")]
        public JsonEmbed[]? Embeds { get; set; }

        [JsonPropertyName("pinned")]
        public bool? Pinned { get; set; }

        [JsonPropertyName("reactions")]
        public JsonReaction[]? Reactions { get; set; }

        [JsonPropertyName("activity")]
        public JsonMessageActivity? Activity { get; set; }

        [JsonPropertyName("message_reference")]
        public JsonMessageReference? Reference { get; set; }

        [JsonPropertyName("referenced_message")]
        public JsonMessage? ReferencedMessage { get; set; }
    }
}
