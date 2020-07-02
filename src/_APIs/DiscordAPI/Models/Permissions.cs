using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAPI.Models
{

    [Flags]
    public enum GuildPermission
    {
        CreateInstantInvite = 0x1,
        KickMembers = 0x2,
        BanMembers = 0x4,
        Administrator = 0x8,
        ManageChannels = 0x10,
        ManangeGuild = 0x20,
        AddReactions = 0x40,
        ViewAuditLog = 0x80,
        PrioritySpeaker = 0x100,
        ReadMessages = 0x400,
        SendMessages = 0x800,
        SendTtsMessages = 0x1000,
        ManageMessages = 0x2000,
        EmbedLinks = 0x4000,
        AttachFiles = 0x8000,
        ReadMessageHistory = 0x10000,
        MentionEveryone = 0x20000,
        UseExternalEmojis = 0x40000,
        Connect = 0x100000,
        Speak = 0x200000,
        MuteMembers = 0x400000,
        DeafenMembers = 0x800000,
        MoveMembers = 0x1000000,
        UseVad = 0x2000000,
        ChangeNickname = 0x4000000,
        ManageNicknames = 0x8000000,
        ManageRoles = 0x10000000,
        ManageWebhooks = 0x20000000,
        ManageEmojis = 0x40000000,
    }

    public class Permissions
    {
        #region Constructors

        public Permissions(int perms)
        {
            _perms = (GuildPermission)perms;
        }

        public Permissions(GuildPermission perms)
        {
            _perms = perms;
        }

        #endregion

        #region Properties

        public bool CreateInstantInvite
        {
            get => GetPerm(GuildPermission.CreateInstantInvite);
            set => SetPerm(GuildPermission.CreateInstantInvite, value);
        }

        public bool KickMembers
        {
            get => GetPerm(GuildPermission.KickMembers);
            set => SetPerm(GuildPermission.KickMembers, value);
        }

        public bool BanMembers
        {
            get => GetPerm(GuildPermission.BanMembers);
            set => SetPerm(GuildPermission.BanMembers, value);
        }

        public bool Administrator
        {
            get => GetPerm(GuildPermission.Administrator);
            set => SetPerm(GuildPermission.Administrator, value);
        }

        public bool ManageChannels
        {
            get => GetPerm(GuildPermission.ManageChannels);
            set => SetPerm(GuildPermission.ManageChannels, value);
        }

        public bool ManangeGuild
        {
            get => GetPerm(GuildPermission.ManangeGuild);
            set => SetPerm(GuildPermission.ManangeGuild, value);
        }

        public bool AddReactions
        {
            get => GetPerm(GuildPermission.AddReactions);
            set => SetPerm(GuildPermission.AddReactions, value);
        }

        public bool ViewAuditLog
        {
            get => GetPerm(GuildPermission.ViewAuditLog);
            set => SetPerm(GuildPermission.ViewAuditLog, value);
        }

        public bool ReadMessages
        {
            get => GetPerm(GuildPermission.ReadMessages);
            set => SetPerm(GuildPermission.ReadMessages, value);
        }

        public bool SendMessages
        {
            get => GetPerm(GuildPermission.SendMessages);
            set => SetPerm(GuildPermission.SendMessages, value);
        }

        public bool SendTtsMessages
        {
            get => GetPerm(GuildPermission.SendTtsMessages);
            set => SetPerm(GuildPermission.SendTtsMessages, value);
        }

        public bool ManageMessages
        {
            get => GetPerm(GuildPermission.ManageMessages);
            set => SetPerm(GuildPermission.ManageMessages, value);
        }

        public bool EmbedLinks
        {
            get => GetPerm(GuildPermission.EmbedLinks);
            set => SetPerm(GuildPermission.EmbedLinks, value);
        }

        public bool AttachFiles
        {
            get => GetPerm(GuildPermission.AttachFiles);
            set => SetPerm(GuildPermission.AttachFiles, value);
        }

        public bool ReadMessageHistory
        {
            get => GetPerm(GuildPermission.ReadMessageHistory);
            set => SetPerm(GuildPermission.ReadMessageHistory, value);
        }

        public bool MentionEveryone
        {
            get => GetPerm(GuildPermission.MentionEveryone);
            set => SetPerm(GuildPermission.MentionEveryone, value);
        }

        public bool UseExternalEmojis
        {
            get => GetPerm(GuildPermission.UseExternalEmojis);
            set => SetPerm(GuildPermission.UseExternalEmojis, value);
        }

        public bool Connect
        {
            get => GetPerm(GuildPermission.Connect);
            set => SetPerm(GuildPermission.Connect, value);
        }

        public bool Speak
        {
            get => GetPerm(GuildPermission.Speak);
            set => SetPerm(GuildPermission.Speak, value);
        }

        public bool MuteMembers
        {
            get => GetPerm(GuildPermission.MuteMembers);
            set => SetPerm(GuildPermission.MuteMembers, value);
        }

        public bool DeafenMembers
        {
            get => GetPerm(GuildPermission.DeafenMembers);
            set => SetPerm(GuildPermission.DeafenMembers, value);
        }

        public bool MoveMembers
        {
            get => GetPerm(GuildPermission.MoveMembers);
            set => SetPerm(GuildPermission.MoveMembers, value);
        }

        public bool UseVad
        {
            get => GetPerm(GuildPermission.UseVad);
            set => SetPerm(GuildPermission.UseVad, value);
        }

        public bool ChangeNickname
        {
            get => GetPerm(GuildPermission.ChangeNickname);
            set => SetPerm(GuildPermission.ChangeNickname, value);
        }

        public bool ManageNicknames
        {
            get => GetPerm(GuildPermission.ManageNicknames);
            set => SetPerm(GuildPermission.ManageNicknames, value);
        }

        public bool ManageRoles
        {
            get => GetPerm(GuildPermission.ManageRoles);
            set => SetPerm(GuildPermission.ManageRoles, value);
        }

        public bool ManageWebhooks
        {
            get => GetPerm(GuildPermission.ManageWebhooks);
            set => SetPerm(GuildPermission.ManageWebhooks, value);
        }

        public bool ManageEmojis
        {
            get => GetPerm(GuildPermission.ManageEmojis);
            set => SetPerm(GuildPermission.ManageEmojis, value);
        }

        public bool PrioritySpeaker
        {
            get => GetPerm(GuildPermission.PrioritySpeaker);
            set => SetPerm(GuildPermission.PrioritySpeaker, value);
        }

        #endregion

        #region Methods

        public void AddAllows(GuildPermission set)
        {
            _perms |= set;
        }

        public void AddDenies(GuildPermission set)
        {
            _perms &= ~set;
        }

        public Permissions Clone()
        {
            return new Permissions(_perms);
        }

        #endregion

        #region Helper Methods

        public PermissionDifference GetDifference(Permissions oldPermissions)
        {
            List<string> oldperms = oldPermissions.GetPermissions();
            List<string> newperms = GetPermissions();
            IEnumerable<string> added = oldperms.Except(newperms);
            IEnumerable<string> removed = newperms.Except(oldperms);
            return new PermissionDifference
            {
                AddedPermissions = added,
                RemovedPermissions = removed
            };
        }

        public List<string> GetPermissions()
        {
            List<string> perms = new List<string>();
            if (Administrator) perms.Add("Administrator");
            if (AddReactions) perms.Add("ADD_REACTIONS");
            if (AttachFiles) perms.Add("ATTACH_FILES");
            if (BanMembers) perms.Add("BAN_MEMBERS");
            if (ChangeNickname) perms.Add("CHANGE_NICKNAME");
            if (Connect) perms.Add("CONNECT");
            if (CreateInstantInvite) perms.Add("CREATE_INSTANT_INVITE");
            if (DeafenMembers) perms.Add("DEAFEN_MEMBERS");
            if (EmbedLinks) perms.Add("EMBED_LINKS");
            if (KickMembers) perms.Add("KICK_MEMBERS");
            if (ManageChannels) perms.Add("MANAGE_CHANNELS");
            if (ManageEmojis) perms.Add("MANAGE_EMOJIS");
            if (ManageMessages) perms.Add("MANAGE_MESSAGES");
            if (ManageNicknames) perms.Add("MANAGE_NICKNAMES");
            if (ManageRoles) perms.Add("MANAGE_ROLES");
            if (ManageWebhooks) perms.Add("MANAGE_WEBHOOKS");
            if (ManangeGuild) perms.Add("MANAGE_GUILD");
            if (MentionEveryone) perms.Add("MENTION_EVERYONE");
            if (MoveMembers) perms.Add("MOVE_MEMBERS");
            if (MuteMembers) perms.Add("MUTE_MEMBERS");
            if (ReadMessageHistory) perms.Add("READ_MESSAGE_HISTORY");
            if (ReadMessages) perms.Add("READ_MESSAGES");
            if (SendMessages) perms.Add("SEND_MESSAGES");
            if (SendTtsMessages) perms.Add("SEND_TTS_MESSAGES");
            if (Speak) perms.Add("SPEAK");
            if (UseExternalEmojis) perms.Add("USE_EXTERNAL_EMOJIS");
            if (UseVad) perms.Add("USE_VAD");
            if (ViewAuditLog) perms.Add("VIEW_AUDIT_LOGS");
            if (PrioritySpeaker) perms.Add("PRIORITY_SPEAKER");
            return perms;
        }

        private bool GetPerm(GuildPermission perm)
        {
            return ((_perms & perm) == perm || (_perms & GuildPermission.Administrator) == GuildPermission.Administrator);
        }

        private void SetPerm(GuildPermission perm, bool value)
        {
            if (value)
            {
                AddAllows(perm);
            }
            else
            {
                AddDenies(perm);
            }
        }

        #endregion

        private GuildPermission _perms;
    }

    public class PermissionDifference
    {
        /// <summary>
        /// Added permissions in the form of a string list
        /// </summary>
        public IEnumerable<string> AddedPermissions { get; set; }

        /// <summary>
        /// Removed Permissions in the form of a string list
        /// </summary>
        public IEnumerable<string> RemovedPermissions { get; set; }
    }

}
