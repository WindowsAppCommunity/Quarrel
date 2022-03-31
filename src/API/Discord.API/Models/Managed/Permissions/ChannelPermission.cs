// Adam Dernis © 2022

using System;

namespace Discord.API.Models.Permissions
{
    [Flags]
    public enum ChannelPermission : ulong
    {
        None = 0,
        CreateInstantInvite = 1 << 0,
        KickMembers = (ulong)1 << 1,
        BanMembers = (ulong)1 << 2,
        Administrator = (ulong)1 << 3,
        ManageChannels = (ulong)1 << 4,
        ManangeGuild = (ulong)1 << 5,
        AddReactions = (ulong)1 << 6,
        ViewAuditLog = (ulong)1 << 7,
        PrioritySpeaker = (ulong)1 << 8,
        Stream = (ulong)1 << 9,
        ReadMessages = (ulong)1 << 10,
        SendMessages = (ulong)1 << 11,
        SendTtsMessages = (ulong)1 << 12,
        ManageMessages = (ulong)1 << 13,
        EmbedLinks = (ulong)1 << 14,
        AttachFiles = (ulong)1 << 15,
        ReadMessageHistory = (ulong)1 << 16,
        MentionEveryone = (ulong)1 << 17,
        UseExternalEmojis = (ulong)1 << 18,
        Connect = (ulong)1 << 19,
        Speak = (ulong)1 << 20,
        MuteMembers = (ulong)1 << 21,
        DeafenMembers = (ulong)1 << 22,
        MoveMembers = (ulong)1 << 23,
        UseVad = (ulong)1 << 24,
        ChangeNickname = (ulong)1 << 25,
        ManageNicknames = (ulong)1 << 26,
        ManageRoles = (ulong)1 << 27,
        ManageWebhooks = (ulong)1 << 28,
        ManageEmojis = (ulong)1 << 29,
        UseApplicationCommands = (ulong)1 << 30,
        RequestToSpeak = (ulong)1 << 31,
        ManageThreads = (ulong)1 << 34,
        CreatePublicThreads = (ulong)1 << 35,
        CreatePrivateThreads = (ulong)1 << 36,
        UseExternalStickers = (ulong)1 << 37,
        SendMessagesInThreads = (ulong)1 << 38,
        StartEmbeddedActivities = (ulong)1 << 39,
        ModerateMembers = (ulong)1 << 40,
    }
}
