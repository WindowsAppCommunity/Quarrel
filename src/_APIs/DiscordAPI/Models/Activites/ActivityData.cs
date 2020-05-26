// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models.Activites
{
    /// <summary>
    /// A model for a user's activity.
    /// </summary>
    public class ActivityData
    {
        /// <summary>
        /// Gets or sets the duraiton the activity occured.
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the activity application's id.
        /// </summary>
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the activity user's id.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets when the activity was updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
