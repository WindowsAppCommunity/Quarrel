using System;
using System.Collections.Generic;
using System.Linq;
using DiscordAPI.SharedModels;

namespace Quarrel.LocalModels
{
    public class PermissionDifference
    {
        /// <summary>
        /// Removed Permissions in the form of a string list
        /// </summary>
        public IEnumerable<string> RemovedPermissions { get; set; }

        /// <summary>
        /// Added permissions in the form of a string list
        /// </summary>
        public IEnumerable<string> AddedPermissions { get; set; }
    }

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
        ManageEmojis = 0x40000000
    }

    public class Permissions
    {
        private GuildPermission _perms;

        /// <summary>
        /// Intiailize permissions by int
        /// </summary>
        /// <param name="perms">int permissions</param>
        public Permissions(int perms)
        {
            _perms = (GuildPermission)perms;
        }

        /// <summary>
        /// Initialize permissions by enum
        /// </summary>
        /// <param name="perms">enum permissions</param>
        public Permissions(GuildPermission perms)
        {
            _perms = perms;
        }

        /// <summary>
        /// Initilize Permissions by guild, channel and user Ids
        /// </summary>
        /// <param name="guildId">GuildId</param>
        /// <param name="channelId">ChannelId (default none)</param>
        /// <param name="userId">UserId (default current user)</param>
        public Permissions(string guildId, string channelId = "", string userId = "")
        {
            // If no user is selected, current user
            if (userId == "") userId = LocalState.CurrentUser.Id;

            // Initialize as none
            _perms = 0;

            // Add allows for @everyone role
            AddAllows((GuildPermission)(LocalState.Guilds[guildId].roles.First(a => a.Value.Position == 0).Value.Permissions));

            // Add allows for each role
            if (LocalState.Guilds[guildId].members.ContainsKey(userId))
                foreach (string role in LocalState.Guilds[guildId].members[userId].Roles)
                    AddAllows((GuildPermission)LocalState.Guilds[guildId].roles[role].Permissions);

            if (channelId != "" && LocalState.Guilds[guildId].channels.ContainsKey(channelId))
            {
                //Order:
                //  Denies of @everyone
                //  Allows of @everyone
                //  All Role Denies
                //  All Role Allows
                //  Member denies
                //  Member allows

                GuildPermission roleDenies = 0;
                GuildPermission roleAllows = 0;
                GuildPermission memberDenies = 0;
                GuildPermission memberAllows = 0;

                foreach (Overwrite overwrite in LocalState.Guilds[guildId].channels[channelId].raw.PermissionOverwrites)
                    if (overwrite.Id == guildId)
                    {
                        AddDenies((GuildPermission)overwrite.Deny);
                        AddAllows((GuildPermission)overwrite.Allow);
                    }
                    else if (overwrite.Type == "role" && LocalState.Guilds[guildId].members[userId].Roles.Contains(overwrite.Id))
                    {
                        roleDenies |= (GuildPermission)overwrite.Deny;
                        roleAllows |= (GuildPermission)overwrite.Allow;
                    }
                    else if (overwrite.Type == "member" && overwrite.Id == userId)
                    {
                        memberDenies |= (GuildPermission)overwrite.Deny;
                        memberAllows |= (GuildPermission)overwrite.Allow;
                    }

                AddDenies(roleDenies);
                AddAllows(roleAllows);
                AddDenies(memberDenies);
                AddAllows(memberAllows);
            }

            // If owner, add admin
            if (LocalState.Guilds[guildId].Raw.OwnerId == userId)
            {
                SetPerm(GuildPermission.Administrator, true);
            }
        }

        /// <summary>
        /// Get permission by enum
        /// </summary>
        /// <param name="perm">permssion</param>
        /// <returns>Permission of Perm</returns>
        private bool GetPerm(GuildPermission perm)
        {
            return ((_perms & perm) == perm || (_perms & GuildPermission.Administrator) == GuildPermission.Administrator);
        }

        /// <summary>
        /// Set permission by enum
        /// </summary>
        /// <param name="perm">permission</param>
        /// <param name="value">status</param>
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

        /// <summary>
        /// Permissions Create Instant Invite status
        /// </summary>
        public bool CreateInstantInvite
        {
            get => GetPerm(GuildPermission.CreateInstantInvite);
            set => SetPerm(GuildPermission.CreateInstantInvite, value);
        }

        /// <summary>
        /// Permissions Kick Members status
        /// </summary>
        public bool KickMembers
        {
            get => GetPerm(GuildPermission.KickMembers);
            set => SetPerm(GuildPermission.KickMembers, value);
        }

        /// <summary>
        /// Permissions Ban Members status
        /// </summary>
        public bool BanMembers
        {
            get => GetPerm(GuildPermission.BanMembers);
            set => SetPerm(GuildPermission.BanMembers, value);
        }

        /// <summary>
        /// Permissions Admin status
        /// </summary>
        public bool Administrator
        {
            get => GetPerm(GuildPermission.Administrator);
            set => SetPerm(GuildPermission.Administrator, value);
        }

        /// <summary>
        /// Permissions Manage Channels status
        /// </summary>
        public bool ManageChannels
        {
            get => GetPerm(GuildPermission.ManageChannels);
            set => SetPerm(GuildPermission.ManageChannels, value);
        }

        /// <summary>
        /// Permissions Manage Guild status
        /// </summary>
        public bool ManangeGuild
        {
            get => GetPerm(GuildPermission.ManangeGuild);
            set => SetPerm(GuildPermission.ManangeGuild, value);
        }

        /// <summary>
        /// Permissions Add Reactions status
        /// </summary>
        public bool AddReactions
        {
            get => GetPerm(GuildPermission.AddReactions);
            set => SetPerm(GuildPermission.AddReactions, value);
        }

        /// <summary>
        /// Permissions View Audit Log status
        /// </summary>
        public bool ViewAuditLog
        {
            get => GetPerm(GuildPermission.ViewAuditLog);
            set => SetPerm(GuildPermission.ViewAuditLog, value);
        }

        /// <summary>
        /// Permissions Read Messages status 
        /// </summary>
        public bool ReadMessages
        {
            get => GetPerm(GuildPermission.ReadMessages);
            set => SetPerm(GuildPermission.ReadMessages, value);
        }

        /// <summary>
        /// Permissions Send Messages status
        /// </summary>
        public bool SendMessages
        {
            get => GetPerm(GuildPermission.SendMessages);
            set => SetPerm(GuildPermission.SendMessages, value);
        }

        /// <summary>
        /// Permissions Send TTS Messages status
        /// </summary>
        public bool SendTtsMessages
        {
            get => GetPerm(GuildPermission.SendTtsMessages);
            set => SetPerm(GuildPermission.SendTtsMessages, value);
        }

        /// <summary>
        /// Permissions Manage Messages status
        /// </summary>
        public bool ManageMessages
        {
            get => GetPerm(GuildPermission.ManageMessages);
            set => SetPerm(GuildPermission.ManageMessages, value);
        }

        /// <summary>
        /// Permissions EmbedLinks status
        /// </summary>
        public bool EmbedLinks
        {
            get => GetPerm(GuildPermission.EmbedLinks);
            set => SetPerm(GuildPermission.EmbedLinks, value);
        }

        /// <summary>
        /// Permissions Attach Files status
        /// </summary>
        public bool AttachFiles
        {
            get => GetPerm(GuildPermission.AttachFiles);
            set => SetPerm(GuildPermission.AttachFiles, value);
        }

        /// <summary>
        /// Permissions Read Message History status
        /// </summary>
        public bool ReadMessageHistory
        {
            get => GetPerm(GuildPermission.ReadMessageHistory);
            set => SetPerm(GuildPermission.ReadMessageHistory, value);
        }

        /// <summary>
        /// Permissions Mention @everyone status
        /// </summary>
        public bool MentionEveryone
        {
            get => GetPerm(GuildPermission.MentionEveryone);
            set => SetPerm(GuildPermission.MentionEveryone, value);
        }

        /// <summary>
        /// Permissions External Emojis status
        /// </summary>
        public bool UseExternalEmojis
        {
            get => GetPerm(GuildPermission.UseExternalEmojis);
            set => SetPerm(GuildPermission.UseExternalEmojis, value);
        }

        /// <summary>
        /// Permissions Connect status
        /// </summary>
        public bool Connect
        {
            get => GetPerm(GuildPermission.Connect);
            set => SetPerm(GuildPermission.Connect, value);
        }

        /// <summary>
        /// Permissions Speak status
        /// </summary>
        public bool Speak
        {
            get => GetPerm(GuildPermission.Speak);
            set => SetPerm(GuildPermission.Speak, value);
        }

        /// <summary>
        /// Permissions Mute Members status
        /// </summary>
        public bool MuteMembers
        {
            get => GetPerm(GuildPermission.MuteMembers);
            set => SetPerm(GuildPermission.MuteMembers, value);
        }

        /// <summary>
        /// Permissions Deafen Members status
        /// </summary>
        public bool DeafenMembers
        {
            get => GetPerm(GuildPermission.DeafenMembers);
            set => SetPerm(GuildPermission.DeafenMembers, value);
        }

        /// <summary>
        /// Permissions Move Member status
        /// </summary>
        public bool MoveMembers
        {
            get => GetPerm(GuildPermission.MoveMembers);
            set => SetPerm(GuildPermission.MoveMembers, value);
        }

        /// <summary>
        /// Permissions Vad status
        /// </summary>
        public bool UseVad
        {
            get => GetPerm(GuildPermission.UseVad);
            set => SetPerm(GuildPermission.UseVad, value);
        }

        /// <summary>
        /// Permissions Change Nickname status
        /// </summary>
        public bool ChangeNickname
        {
            get => GetPerm(GuildPermission.ChangeNickname);
            set => SetPerm(GuildPermission.ChangeNickname, value);
        }

        /// <summary>
        /// Permissions Manage Nicknames status
        /// </summary>
        public bool ManageNicknames
        {
            get => GetPerm(GuildPermission.ManageNicknames);
            set => SetPerm(GuildPermission.ManageNicknames, value);
        }

        /// <summary>
        /// Permissions Manage Roles status
        /// </summary>
        public bool ManageRoles
        {
            get => GetPerm(GuildPermission.ManageRoles);
            set => SetPerm(GuildPermission.ManageRoles, value);
        }

        /// <summary>
        /// Permissions Manage Webhooks status
        /// </summary>
        public bool ManageWebhooks
        {
            get => GetPerm(GuildPermission.ManageWebhooks);
            set => SetPerm(GuildPermission.ManageWebhooks, value);
        }

        /// <summary>
        /// Permissions Manage Emojis status
        /// </summary>
        public bool ManageEmojis
        {
            get => GetPerm(GuildPermission.ManageEmojis);
            set => SetPerm(GuildPermission.ManageEmojis, value);
        }

        /// <summary>
        /// Permissions Priority Speaker status
        /// </summary>
        public bool PrioritySpeaker
        {
            get => GetPerm(GuildPermission.PrioritySpeaker);
            set => SetPerm(GuildPermission.PrioritySpeaker, value);
        }

        /// <summary>
        /// Compare old set of permissions with current set
        /// </summary>
        /// <param name="oldPermissions">Old Permissions object</param>
        /// <returns>String difference of permssions</returns>
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
            if (Administrator) perms.Add(App.GetString("/Permissions/ADMINISTRATOR"));
            if (AddReactions) perms.Add(App.GetString("/Permissions/ADD_REACTIONS"));
            if (AttachFiles) perms.Add(App.GetString("/Permissions/ATTACH_FILES"));
            if (BanMembers) perms.Add(App.GetString("/Permissions/BAN_MEMBERS"));
            if (ChangeNickname) perms.Add(App.GetString("/Permissions/CHANGE_NICKNAME"));
            if (Connect) perms.Add(App.GetString("/Permissions/CONNECT"));
            if (CreateInstantInvite) perms.Add(App.GetString("/Permissions/CREATE_INSTANT_INVITE"));
            if (DeafenMembers) perms.Add(App.GetString("/Permissions/DEAFEN_MEMBERS"));
            if (EmbedLinks) perms.Add(App.GetString("/Permissions/EMBED_LINKS"));
            if (KickMembers) perms.Add(App.GetString("/Permissions/KICK_MEMBERS"));
            if (ManageChannels) perms.Add(App.GetString("/Permissions/MANAGE_CHANNELS"));
            if (ManageEmojis) perms.Add(App.GetString("/Permissions/MANAGE_EMOJIS"));
            if (ManageMessages) perms.Add(App.GetString("/Permissions/MANAGE_MESSAGES"));
            if (ManageNicknames) perms.Add(App.GetString("/Permissions/MANAGE_NICKNAMES"));
            if (ManageRoles) perms.Add(App.GetString("/Permissions/MANAGE_ROLES"));
            if (ManageWebhooks) perms.Add(App.GetString("/Permissions/MANAGE_WEBHOOKS"));
            if (ManangeGuild) perms.Add(App.GetString("/Permissions/MANAGE_GUILD"));
            if (MentionEveryone) perms.Add(App.GetString("/Permissions/MENTION_EVERYONE"));
            if (MoveMembers) perms.Add(App.GetString("/Permissions/MOVE_MEMBERS"));
            if (MuteMembers) perms.Add(App.GetString("/Permissions/MUTE_MEMBERS"));
            if (ReadMessageHistory) perms.Add(App.GetString("/Permissions/READ_MESSAGE_HISTORY"));
            if (ReadMessages) perms.Add(App.GetString("/Permissions/READ_MESSAGES"));
            if (SendMessages) perms.Add(App.GetString("/Permissions/SEND_MESSAGES"));
            if (SendTtsMessages) perms.Add(App.GetString("/Permissions/SEND_TTS_MESSAGES"));
            if (Speak) perms.Add(App.GetString("/Permissions/SPEAK"));
            if (UseExternalEmojis) perms.Add(App.GetString("/Permissions/USE_EXTERNAL_EMOJIS"));
            if (UseVad) perms.Add(App.GetString("/Permissions/USE_VAD"));
            if (ViewAuditLog) perms.Add(App.GetString("/Permissions/VIEW_AUDIT_LOGS"));
            if (PrioritySpeaker) perms.Add(App.GetString("/Permissions/PRIORITY_SPEAKER"));
            return perms;
        }

        /// <summary>
        /// Add permissions by flagged enum
        /// </summary>
        /// <param name="set"></param>
        private void AddAllows(GuildPermission set)
        {
            _perms |= set;
        }

        /// <summary>
        /// Remove permissions by flagged enum
        /// </summary>
        /// <param name="set"></param>
        private void AddDenies(GuildPermission set)
        {
            _perms &= ~set;
        }

        public int GetPermInt()
        {
            return (int)_perms;
        }

        /// <summary>
        /// Check if current user can change user's Nickname
        /// </summary>
        /// <param name="userId">User Id to change</param>
        /// <param name="guildId">Guild Id for Nickname</param>
        /// <returns>Current User's nickname changing permissions for user in a guild</returns>
        public static bool CanChangeNickname(string userId, string guildId)
        {
            Permissions cachedPerms = new Permissions(guildId);
            return (cachedPerms.ChangeNickname && userId == LocalState.CurrentUser.Id) || cachedPerms.ManageNicknames &&
                   (!LocalState.Guilds[guildId].members[userId].Roles.Any() ||
                    LocalState.Guilds[guildId]
                        .roles[LocalState.Guilds[guildId].members[LocalState.CurrentUser.Id].Roles.FirstOrDefault()]
                        .Position >= LocalState.Guilds[guildId]
                        .roles[LocalState.Guilds[guildId].members[userId].Roles.FirstOrDefault()].Position);
        }
    }
}
