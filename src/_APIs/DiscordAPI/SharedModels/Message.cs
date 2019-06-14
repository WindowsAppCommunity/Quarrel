using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Message
    {
        public Message MergePending(Message gatewayData)
        {
            Id = gatewayData.Id;
            Attachments = gatewayData.Attachments;
            Embeds = gatewayData.Embeds;
            Mentions = gatewayData.Mentions;
            MentionEveryone = gatewayData.MentionEveryone;
            MentionRoles = gatewayData.MentionRoles;
            if (gatewayData.User.Id != null)
            {
                User = gatewayData.User;
            }
            if (gatewayData.Timestamp.Ticks > 100000)
            {
                Timestamp = gatewayData.Timestamp;
            }
            else
            {
                Timestamp.AddTicks(gatewayData.Timestamp.Ticks);
            }
            return this;
        }

        public void SetUser(User user)
        {
            User = user;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("activity")]
        public Activity Activity { get; set; }
        [JsonProperty("author")]
        public User User { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("call")]
        public Call Call { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }
        [JsonProperty("tts")]
        public bool TTS { get; set; }
        [JsonProperty("mention_everyone")]
        public bool MentionEveryone { get; set; }
        [JsonProperty("mentions")]
        public IEnumerable<User> Mentions { get; set; }
        [JsonProperty("mention_roles")]
        public IEnumerable<string> MentionRoles { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
        [JsonProperty("embeds")]
        public IEnumerable<Embed> Embeds { get; set; }
        [JsonProperty("reactions")]
        public IEnumerable<Reactions> Reactions { get; set; }
        [JsonProperty("nonce")]
        public long? Nonce { get; set; }
        [JsonProperty("pinned")]
        public bool Pinned { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("hits")]
        public bool Hit { get; set; }
        [JsonProperty("webhook_id")]
        public string WebHookid { get; set; }
    }
    public class Activity
    {
        [JsonProperty("party_id")]
        public string PartyId { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }
    public class MessageAck
    {
        [JsonProperty("message_id")]
        public string Id { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }

    public class Call
    {
        [JsonProperty("participants")]
        public IEnumerable<string> Participants { get; set; }
        [JsonProperty("ended_timestamp")]
        public string EndedTimestamp { get; set; }
    }
}
