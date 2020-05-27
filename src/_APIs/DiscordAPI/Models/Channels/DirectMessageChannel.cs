// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordAPI.Models.Channels
{
    /// <summary>
    /// A model for a DirectMessage channel.
    /// </summary>
    public class DirectMessageChannel : Channel
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the channel is private.
        /// </summary>
        [JsonProperty("is_private")]
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets the owner's id of the channel.
        /// </summary>
        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the recipients of the channel.
        /// </summary>
        [JsonProperty("recipients")]
        public List<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the icon hash of the channel.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        public override string Name
        {
            get { return RawName == null ? NameFromUsers() : RawName; }
        }

        /// <summary>
        /// Gets the recipients list as a string.
        /// </summary>
        /// <returns>The recipient's users as a string.</returns>
        private string NameFromUsers()
        {
            string cache = string.Empty;
            bool first = true;
            foreach (var user in Users)
            {
                if (first)
                {
                    cache += user.Username;
                    first = false;
                }
                else
                {
                    cache += ", " + user.Username;
                }
            }

            return string.IsNullOrEmpty(cache) ? "Unnamed" : cache;
        }

        /// <summary>
        /// Gets the icon url for the channel.
        /// </summary>
        /// <param name="useDefault">A value indicating if the default icon should be used.</param>
        /// <param name="darkTheme">A value indicating if the dark theme icon should be used.</param>
        /// <returns>A url for the channel's icon.</returns>
        public string IconUrl(bool useDefault = true, bool darkTheme = false)
        {
            if (Icon != null)
            {
                return "https://cdn.discordapp.com/channel-icons/" + Id + "/" + Icon + ".png";
            }

            if (useDefault)
            {
                return "ms-appx:///Assets/DefaultAvatars/Group_" + (darkTheme ? "dark" : "light") + ".png";
            }

            return null;
        }

        /// <summary>
        /// Gets the icon uri for the channel.
        /// </summary>
        /// <param name="useDefault">A value indicating if the default icon should be used.</param>
        /// <param name="darkTheme">A value indicating if the dark theme icon should be used.</param>
        /// <returns>A uri for the channel's icon.</returns>
        public Uri IconUri(bool useDefault = true, bool darkTheme = false)
        {
            var url = IconUrl(useDefault, darkTheme);
            return url != null ? new Uri(url) : null;
        }
    }
}
