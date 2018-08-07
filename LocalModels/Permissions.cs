using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.LocalModels
{
    public class PermissionDifference
    {
        public List<string> RemovedPermissions { get; set; }
        public List<string> AddedPermissions { get; set; }
    }
    public class Permissions
    {
        public PermissionDifference GetDifference(Permissions oldPermissions)
        {
            var oldperms = oldPermissions.GetPermissions();
            var newperms = GetPermissions();
            var diffFilter = new HashSet<string>(oldperms, EqualityComparer<string>.Default);
            var removed = new List<string>();
            foreach (var item in oldperms.Distinct())
            {
                if (diffFilter.Contains(item))
                {
                    diffFilter.Remove(item);
                }
                else
                {
                    removed.Add(item);
                }
            }
            var added = diffFilter.ToList();
            return new PermissionDifference()
            {
                AddedPermissions = added,
                RemovedPermissions = removed
            };
        }
        public List<string> GetPermissions()
        {
            var perms = new List<string>();
            if (Administrator)        perms.Add(App.GetString("/Dialogs/AdministratorToggle.Header"));
            if (AddReactions)         perms.Add(App.GetString("/Dialogs/AddReactionsToggle.Header"));
            if (AttachFiles)          perms.Add(App.GetString("/Dialogs/AttachFilesToggle.Header"));
            if (BanMembers)           perms.Add(App.GetString("/Dialogs/BanMembersToggle.Header"));
            if (ChangeNickname)       perms.Add(App.GetString("/Dialogs/ChangeNickname.Header"));
            if (Connect)              perms.Add(App.GetString("/Dialogs/ConnectToggle.Header"));
            if (CreateInstantInvite)  perms.Add(App.GetString("/Dialogs/CreateInstantInviteToggle.Header"));
            if (DeafenMembers)        perms.Add(App.GetString("/Dialogs/DeafenMembersToggle.Header"));
            if (EmbedLinks)           perms.Add(App.GetString("/Dialogs/EmbedLinksToggle.Header"));
            if (KickMembers)          perms.Add(App.GetString("/Dialogs/KickMembersToggle.Header"));
            if (ManageChannels)       perms.Add(App.GetString("/Dialogs/ManageChannelsToggle.Header"));
            if (ManageEmojis)         perms.Add(App.GetString("/Dialogs/ManageEmojisToggle.Header"));
            if (ManageMessages)       perms.Add(App.GetString("/Dialogs/ManageMessagesToggle.Header"));
            if (ManageNicknames)      perms.Add(App.GetString("/Dialogs/ManageNicknamesToggle.Header"));
            if (ManageRoles)          perms.Add(App.GetString("/Dialogs/ManageRolesToggle.Header"));
            if (ManageWebhooks)       perms.Add(App.GetString("/Dialogs/ManageWebhooksToggle.Header"));
            if (ManangeGuild)         perms.Add(App.GetString("/Dialogs/ManageGuildToggle.Header"));
            if (MentionEveryone)      perms.Add(App.GetString("/Dialogs/MentionEveryoneToggle.Header"));
            if (MoveMembers)          perms.Add(App.GetString("/Dialogs/MoveMembersToggle.Header"));
            if (MuteMembers)          perms.Add(App.GetString("/Dialogs/MuteMembersToggle.Header"));
            if (ReadMessageHistory)   perms.Add(App.GetString("/Dialogs/ReadMessageHistoryToggle.Header"));
            if (ReadMessages)         perms.Add(App.GetString("/Dialogs/ReadMessagesToggle.Header"));
            if (SendMessages)         perms.Add(App.GetString("/Dialogs/SendMessagesToggle.Header"));
            if (SendTtsMessages)      perms.Add(App.GetString("/Dialogs/SendTTSMessagesToggle.Header"));
            if (Speak)                perms.Add(App.GetString("/Dialogs/SpeakToggle.Header"));
            if (UseExternalEmojis)    perms.Add(App.GetString("/Dialogs/UseExternalEmojisToggle.Header"));
            if (UseVad)               perms.Add(App.GetString("/Dialogs/UseVadToggle.Header"));
            if (ViewAuditLog)         perms.Add(App.GetString("/Dialogs/ViewAuditLogToggle.Header"));
            return perms;
        }

        public Permissions(int perms)
        {
            Perms = perms;
        }

        public Permissions(string guildId, string channelId = "", string userId = "")
        {
            if (userId == "")
            {
                userId = LocalState.CurrentUser.Id;
            }

            Perms = 0;
            if (LocalState.Guilds[guildId].members.ContainsKey(userId))
            {
                foreach (string role in LocalState.Guilds[guildId].members[userId].Roles)
                {
                    AddAllows(LocalState.Guilds[guildId].roles[role].Permissions);
                }

            }

            if (channelId != "" && LocalState.Guilds[guildId].channels.ContainsKey(channelId))
            {
                //Order:
                //  Denies of @everyone
                //  Allows of @everyone
                //  All Role Denies
                //  All Role Allows
                //  Member denies
                //  Member allows

                int roleDenies = 0;
                int roleAllows = 0;
                int memberDenies = 0;
                int memberAllows = 0;

                foreach (var overwrite in LocalState.Guilds[guildId].channels[channelId].raw.PermissionOverwrites)
                {
                    if (overwrite.Id == guildId)
                    {
                        AddDenies(overwrite.Deny);
                        AddDenies(overwrite.Allow);
                    } else if (overwrite.Type == "role" && LocalState.Guilds[guildId].members[userId].Roles.Contains(overwrite.Id))
                    {
                        roleDenies &= ~overwrite.Deny;
                        roleAllows &= ~overwrite.Allow;
                    } else if (overwrite.Type == "member" && overwrite.Id == userId)
                    {
                        memberDenies = overwrite.Deny;
                        memberAllows = overwrite.Allow;
                    }
                }
                AddDenies(roleDenies);
                AddAllows(roleAllows);
                AddDenies(memberDenies);
                AddAllows(memberAllows);
            }
        }
        
        private void AddAllows(int set)
        {
            Perms |= set;
        }

        private void AddDenies(int set)
        {
            Perms &= ~set;
        }

        public int GetPermInt()
        {
            return Perms;
        }

        public bool CreateInstantInvite
        {
            get { return Convert.ToBoolean(Perms & 0x1) || Administrator; }
            set { Perms = value ? Perms | 0x1 : Perms & ~0x1; }
        }
        public bool KickMembers
        {
            get { return Convert.ToBoolean(Perms & 0x2) || Administrator; }
            set { Perms = value ? Perms | 0x2 : Perms & ~0x2; }
        }
        public bool BanMembers
        {
            get { return Convert.ToBoolean(Perms & 0x4) || Administrator; }
            set { Perms = value ? Perms | 0x4 : Perms & ~0x4; }
        }
        public bool Administrator
        {
            get { return Convert.ToBoolean(Perms & 0x8); }
            set { Perms = value ? Perms | 0x8 : Perms & ~0x8; }
        }
        public bool ManageChannels
        {
            get { return Convert.ToBoolean(Perms & 0x10) || Administrator; }
            set { Perms = value ? Perms | 0x10 : Perms & ~0x10; }
        }
        public bool ManangeGuild
        {
            get { return Convert.ToBoolean(Perms & 0x20) || Administrator; }
            set { Perms = value ? Perms | 0x20 : Perms & ~0x20; }
        }
        public bool AddReactions
        {
            get { return Convert.ToBoolean(Perms & 0x40) || Administrator; }
            set { Perms = value ? Perms | 0x40 : Perms & ~0x40; }
        }
        public bool ViewAuditLog
        {
            get { return Convert.ToBoolean(Perms & 0x80) || Administrator; }
            set { Perms = value ? Perms | 0x80 : Perms & ~0x80; }
        }
        public bool ReadMessages
        {
            get { return Convert.ToBoolean(Perms & 0x400) || Administrator; }
            set { Perms = value ? Perms | 0x400 : Perms & ~0x400; }
        }
        public bool SendMessages
        {
            get { return Convert.ToBoolean(Perms & 0x800) || Administrator; }
            set { Perms = value ? Perms | 0x800 : Perms & ~0x800; }
        }
        public bool SendTtsMessages
        {
            get { return Convert.ToBoolean(Perms & 0x1000) || Administrator; }
            set { Perms = value ? Perms | 0x1000 : Perms & ~0x1000; }
        }
        public bool ManageMessages
        {
            get { return Convert.ToBoolean(Perms & 0x2000) || Administrator; }
            set { Perms = value ? Perms | 0x2000 : Perms & ~0x2000; }
        }
        public bool EmbedLinks
        {
            get { return Convert.ToBoolean(Perms & 0x4000); }
            set { Perms = value ? Perms | 0x4000 : Perms & ~0x4000; }
        }
        public bool AttachFiles
        {
            get { return Convert.ToBoolean(Perms & 0x8000) || Administrator; }
            set { Perms = value ? Perms | 0x8000 : Perms & ~0x8000; }
        }
        public bool ReadMessageHistory
        {
            get { return Convert.ToBoolean(Perms & 0x10000) || Administrator; }
            set { Perms = value ? Perms | 0x10000 : Perms & ~0x10000; }
        }
        public bool MentionEveryone
        {
            get { return Convert.ToBoolean(Perms & 0x20000) || Administrator; }
            set { Perms = value ? Perms | 0x20000 : Perms & ~0x20000; }
        }
        public bool UseExternalEmojis
        {
            get { return Convert.ToBoolean(Perms & 0x40000) || Administrator; }
            set { Perms = value ? Perms | 0x40000 : Perms & ~0x40000; }
        }
        public bool Connect
        {
            get { return Convert.ToBoolean(Perms & 0x100000) || Administrator; }
            set { Perms = value ? Perms | 0x100000 : Perms & ~0x100000; }
        }
        public bool Speak
        {
            get { return Convert.ToBoolean(Perms & 0x200000) || Administrator; }
            set { Perms = value ? Perms | 0x200000 : Perms & ~0x200000; }
        }
        public bool MuteMembers
        {
            get { return Convert.ToBoolean(Perms & 0x400000) || Administrator; }
            set { Perms = value ? Perms | 0x400000 : Perms & ~0x400000; }
        }
        public bool DeafenMembers
        {
            get { return Convert.ToBoolean(Perms & 0x800000) || Administrator; }
            set { Perms = value ? Perms | 0x800000 : Perms & ~0x800000; }
        }
        public bool MoveMembers
        {
            get { return Convert.ToBoolean(Perms & 0x1000000) || Administrator; }
            set { Perms = value ? Perms | 0x1000000 : Perms & ~0x1000000; }
        }
        public bool UseVad
        {
            get { return Convert.ToBoolean(Perms & 0x2000000) || Administrator; }
            set { Perms = value ? Perms | 0x2000000 : Perms & ~0x2000000; }
        }
        public bool ChangeNickname
        {
            get { return Convert.ToBoolean(Perms & 0x4000000) || Administrator; }
            set { Perms = value ? Perms | 0x4000000 : Perms & ~0x4000000; }
        }
        public bool ManageNicknames
        {
            get { return Convert.ToBoolean(Perms & 0x8000000) || Administrator; }
            set { Perms = value ? Perms | 0x8000000 : Perms & ~0x8000000; }
        }
        public bool ManageRoles
        {
            get { return Convert.ToBoolean(Perms & 0x10000000) || Administrator; }
            set { Perms = value ? Perms | 0x10000000 : Perms & ~0x10000000; }
        }
        public bool ManageWebhooks
        {
            get { return Convert.ToBoolean(Perms & 0x20000000) || Administrator; }
            set { Perms = value ? Perms | 0x20000000 : Perms & ~0x20000000; }
        }
        public bool ManageEmojis
        {
            get { return Convert.ToBoolean(Perms & 0x40000000) || Administrator; }
            set { Perms = value ? Perms | 0x40000000 : Perms & ~0x40000000; }
        }
        public bool PrioritySpeaking
        {
            get { return Convert.ToBoolean(Perms & 0x100) || Administrator; }
            set { Perms = value ? Perms | 0x100 : Perms & ~0x100; }
        }
        int Perms = 0;

        public static bool CanChangeNickname(string userId, string guildId)
        {
            if (((new Permissions(guildId)).ChangeNickname && userId == LocalState.CurrentUser.Id) ||
               ((new Permissions(guildId)).ManageNicknames && (LocalState.Guilds[guildId].roles[LocalState.Guilds[guildId].members[LocalState.CurrentUser.Id].Roles.FirstOrDefault()]).Position >= (LocalState.Guilds[guildId].roles[LocalState.Guilds[guildId].members[userId].Roles.FirstOrDefault()]).Position))
            {
                return true;
            }
            return false;
        }
    }
}
