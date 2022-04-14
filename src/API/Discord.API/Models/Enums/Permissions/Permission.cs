// Quarrel © 2022

using System;

namespace Discord.API.Models.Enums.Permissions
{
    /// <summary>
    /// A flagged set of permissions.
    /// </summary>
    [Flags]
    public enum Permission : ulong
    {
        /// <summary>
        /// No permissions specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Permission to create instant invites.
        /// </summary>
        CreateInstantInvite = (ulong)1 << 0,

        /// <summary>
        /// Permission to kick members.
        /// </summary>
        KickMembers = (ulong)1 << 1,

        /// <summary>
        /// Permission to ban members.
        /// </summary>
        BanMembers = (ulong)1 << 2,

        /// <summary>
        /// Administrative permission.
        /// </summary>
        Administrator = (ulong)1 << 3,

        /// <summary>
        /// Permission to manage channels.
        /// </summary>
        ManageChannels = (ulong)1 << 4,

        /// <summary>
        /// Permission to manage the guild.
        /// </summary>
        ManangeGuild = (ulong)1 << 5,

        /// <summary>
        /// Permission to add reactions to messages.
        /// </summary>
        AddReactions = (ulong)1 << 6,

        /// <summary>
        /// Permission to view the audit log.
        /// </summary>
        ViewAuditLog = (ulong)1 << 7,

        /// <summary>
        /// Permission to be a priority speaker.
        /// </summary>
        PrioritySpeaker = (ulong)1 << 8,

        /// <summary>
        /// Permission to stream your desktop in a voice channel.
        /// </summary>
        Stream = (ulong)1 << 9,

        /// <summary>
        /// Permission to read messages.
        /// </summary>
        ReadMessages = (ulong)1 << 10,

        /// <summary>
        /// Permission to send messages.
        /// </summary>
        SendMessages = (ulong)1 << 11,

        /// <summary>
        /// Permission to send messages with text to speech.
        /// </summary>
        SendTtsMessages = (ulong)1 << 12,

        /// <summary>
        /// Permission to delete other user's messages.
        /// </summary>
        ManageMessages = (ulong)1 << 13,

        /// <summary>
        /// Permission to send embedded links.
        /// </summary>
        EmbedLinks = (ulong)1 << 14,

        /// <summary>
        /// Permission to attach files to messages.
        /// </summary>
        AttachFiles = (ulong)1 << 15,

        /// <summary>
        /// Permission to read messages in history.
        /// </summary>
        ReadMessageHistory = (ulong)1 << 16,

        /// <summary>
        /// Permission to mention @everyone.
        /// </summary>
        MentionEveryone = (ulong)1 << 17,

        /// <summary>
        /// Permission to use emojis from other guilds.
        /// </summary>
        UseExternalEmojis = (ulong)1 << 18,

        /// <summary>
        /// Permission to connect to a voice channel.
        /// </summary>
        Connect = (ulong)1 << 19,

        /// <summary>
        /// Permission to speak in a voice channel.
        /// </summary>
        Speak = (ulong)1 << 20,

        /// <summary>
        /// Permission to mute other members in a voice channel.
        /// </summary>
        MuteMembers = (ulong)1 << 21,

        /// <summary>
        /// Permission to deafen other members in a voice channel.
        /// </summary>
        DeafenMembers = (ulong)1 << 22,

        /// <summary>
        /// Permission to move members between voice channels.
        /// </summary>
        MoveMembers = (ulong)1 << 23,

        /// <summary>
        /// Permission to have an open mic in voice channels.
        /// </summary>
        UseVad = (ulong)1 << 24,

        /// <summary>
        /// Permission to change your own nickname.
        /// </summary>
        ChangeNickname = (ulong)1 << 25,

        /// <summary>
        /// Permission to change other's nicknames.
        /// </summary>
        ManageNicknames = (ulong)1 << 26,

        /// <summary>
        /// Permission to manage roles.
        /// </summary>
        ManageRoles = (ulong)1 << 27,

        /// <summary>
        /// Permission to manage webhooks.
        /// </summary>
        ManageWebhooks = (ulong)1 << 28,

        /// <summary>
        /// Permission to manage emojis.
        /// </summary>
        ManageEmojis = (ulong)1 << 29,

        /// <summary>
        /// Permission to use application commands.
        /// </summary>
        UseApplicationCommands = (ulong)1 << 30,

        /// <summary>
        /// Permission to request to speak in a stage voice channel.
        /// </summary>
        RequestToSpeak = (ulong)1 << 31,

        /// <summary>
        /// Permission to manage other's threads.
        /// </summary>
        ManageThreads = (ulong)1 << 34,

        /// <summary>
        /// Permission to create public threads.
        /// </summary>
        CreatePublicThreads = (ulong)1 << 35,

        /// <summary>
        /// Permission to create private threads.
        /// </summary>
        CreatePrivateThreads = (ulong)1 << 36,

        /// <summary>
        /// Permission to use stickers from other guilds.
        /// </summary>
        UseExternalStickers = (ulong)1 << 37,

        /// <summary>
        /// Permission to send messages in threads.
        /// </summary>
        SendMessagesInThreads = (ulong)1 << 38,

        /// <summary>
        /// Permission to start embedded activities.
        /// </summary>
        StartEmbeddedActivities = (ulong)1 << 39,

        /// <summary>
        /// Permission to moderate members.
        /// </summary>
        ModerateMembers = (ulong)1 << 40,
    }
}
