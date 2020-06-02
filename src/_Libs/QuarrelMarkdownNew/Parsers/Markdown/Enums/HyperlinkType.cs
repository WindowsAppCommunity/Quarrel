// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Parsers.Markdown
{
    /// <summary>
    /// Specifies the type of Hyperlink that is used.
    /// </summary>
    public enum HyperlinkType
    {
        /// <summary>
        /// A hyperlink surrounded by angle brackets (e.g. "http://www.reddit.com").
        /// </summary>
        BracketedUrl,

        /// <summary>
        /// A fully qualified hyperlink (e.g. "http://www.reddit.com").
        /// </summary>
        FullUrl,

        /// <summary>
        /// A URL without a scheme (e.g. "www.reddit.com").
        /// </summary>
        PartialUrl,

        /// <summary>
        /// An email address (e.g. "test@reddit.com").
        /// </summary>
        Email,

        /// <summary>
        /// A discord user mention link (e.g. "@UserId").
        /// </summary>
        DiscordUserMention,

        /// <summary>
        /// A discord nick mention link (e.g. "@!UserId").
        /// </summary>
        DiscordNickMention,

        /// <summary>
        /// A discord role mention link (e.g. "@&amp;RoleId").
        /// </summary>
        DiscordRoleMention,

        /// <summary>
        /// A discord channel mention link (e.g. "#general").
        /// </summary>
        DiscordChannelMention,

        /// <summary>
        /// A color tag for the audit log (e.g. "@$Quarrel-color")
        /// </summary>
        QuarrelColor,
    }
}