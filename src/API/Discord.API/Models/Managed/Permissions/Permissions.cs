// Adam Dernis © 2022

using Discord.API.Models.Enums.Permissions;

namespace Discord.API.Models
{
    public struct Permissions
    {
        private Permission _permission;

        public Permissions(Permission permission)
        {
            _permission = permission;
        }

        public bool CreateInstantInvite
        {
            get => GetPerm(Permission.CreateInstantInvite);
            set => SetPerm(Permission.CreateInstantInvite, value);
        }
        
        public bool KickMembers
        {
            get => GetPerm(Permission.KickMembers);
            set => SetPerm(Permission.KickMembers, value);
        }

        public bool BanMembers
        {
            get => GetPerm(_permission & Permission.BanMembers);
            set => SetPerm(Permission.BanMembers, value);
        }

        public bool Administrator
        {
            get => GetPerm(Permission.Administrator);
            set => SetPerm(Permission.Administrator, value);
        }

        public bool ManageChannels
        {
            get => GetPerm(Permission.ManageChannels);
            set => SetPerm(Permission.ManageChannels, value);
        }

        public bool ManangeGuild
        {
            get => GetPerm(Permission.ManangeGuild);
            set => SetPerm(Permission.ManangeGuild, value);
        }

        public bool AddReactions
        {
            get => GetPerm(Permission.AddReactions);
            set => SetPerm(Permission.AddReactions, value);
        }

        public bool ViewAuditLog
        {
            get => GetPerm(Permission.ViewAuditLog);
            set => SetPerm(Permission.ViewAuditLog, value);
        }

        public bool PrioritySpeaker
        {
            get => GetPerm(Permission.PrioritySpeaker);
            set => SetPerm(Permission.PrioritySpeaker, value);
        }

        public bool Stream
        {
            get => GetPerm(Permission.Stream);
            set => SetPerm(Permission.Stream, value);
        }

        public bool ReadMessages
        {
            get => GetPerm(Permission.ReadMessages);
            set => SetPerm(Permission.ReadMessages, value);
        }

        public bool SendMessages
        {
            get => GetPerm(Permission.SendMessages);
            set => SetPerm(Permission.SendMessages, value);
        }

        public bool SendTtsMessages
        {
            get => GetPerm(Permission.SendTtsMessages);
            set => SetPerm(Permission.SendTtsMessages, value);
        }

        public bool ManageMessages
        {
            get => GetPerm(Permission.ManageMessages);
            set => SetPerm(Permission.ManageMessages, value);
        }

        public bool EmbedLinks
        {
            get => GetPerm(Permission.EmbedLinks);
            set => SetPerm(Permission.EmbedLinks, value);
        }

        public bool AttachFiles
        {
            get => GetPerm(Permission.AttachFiles);
            set => SetPerm(Permission.AttachFiles, value);
        }

        public bool ReadMessageHistory
        {
            get => GetPerm(Permission.ReadMessageHistory);
            set => SetPerm(Permission.ReadMessageHistory, value);
        }

        public bool MentionEveryone
        {
            get => GetPerm(Permission.MentionEveryone);
            set => SetPerm(Permission.MentionEveryone, value);
        }

        public bool UseExternalEmojis
        {
            get => GetPerm(Permission.UseExternalEmojis);
            set => SetPerm(Permission.UseExternalEmojis, value);
        }

        public bool Connect
        {
            get => GetPerm(Permission.Connect);
            set => SetPerm(Permission.Connect, value);
        }

        public bool Speak
        {
            get => GetPerm(Permission.Speak);
            set => SetPerm(Permission.Speak, value);
        }

        public bool MuteMembers
        {
            get => GetPerm(Permission.MuteMembers);
            set => SetPerm(Permission.MuteMembers, value);
        }

        public bool DeafenMembers
        {
            get => GetPerm(Permission.DeafenMembers);
            set => SetPerm(Permission.DeafenMembers, value);
        }

        public bool MoveMembers
        {
            get => GetPerm(Permission.MoveMembers);
            set => SetPerm(Permission.MoveMembers, value);
        }

        public bool UseVad
        {
            get => GetPerm(Permission.UseVad);
            set => SetPerm(Permission.UseVad, value);
        }

        public bool ChangeNickname
        {
            get => GetPerm(Permission.ChangeNickname);
            set => SetPerm(Permission.ChangeNickname, value);
        }

        public bool ManageNicknames
        {
            get => GetPerm(Permission.ManageNicknames);
            set => SetPerm(Permission.ManageNicknames, value);
        }

        public bool ManageRoles
        {
            get => GetPerm(Permission.ManageRoles);
            set => SetPerm(Permission.ManageRoles, value);
        }

        public bool ManageWebhooks
        {
            get => GetPerm(Permission.ManageWebhooks);
            set => SetPerm(Permission.ManageWebhooks, value);
        }

        public bool ManageEmojis
        {
            get => GetPerm(Permission.ManageEmojis);
            set => SetPerm(Permission.ManageEmojis, value);
        }

        public bool UseApplicationCommands
        {
            get => GetPerm(Permission.UseApplicationCommands);
            set => SetPerm(Permission.UseApplicationCommands, value);
        }

        public bool RequestToSpeak
        {
            get => GetPerm(Permission.RequestToSpeak);
            set => SetPerm(Permission.RequestToSpeak, value);
        }

        public bool ManageThreads
        {
            get => GetPerm(Permission.ManageThreads);
            set => SetPerm(Permission.ManageThreads, value);
        }

        public bool CreatePublicThreads
        {
            get => GetPerm(Permission.CreatePublicThreads);
            set => SetPerm(Permission.CreatePublicThreads, value);
        }

        public bool CreatePrivateThreads
        {
            get => GetPerm(Permission.CreatePrivateThreads);
            set => SetPerm(Permission.CreatePrivateThreads, value);
        }

        public bool UseExternalStickers
        {
            get => GetPerm(Permission.UseExternalStickers);
            set => SetPerm(Permission.UseExternalStickers, value);
        }

        public bool SendMessagesInThreads
        {
            get => GetPerm(Permission.SendMessagesInThreads);
            set => SetPerm(Permission.SendMessagesInThreads, value);
        }

        public bool StartEmbeddedActivities
        {
            get => GetPerm(Permission.StartEmbeddedActivities);
            set => SetPerm(Permission.StartEmbeddedActivities, value);
        }

        public bool ModerateMembers
        {
            get => GetPerm(Permission.ModerateMembers);
            set => SetPerm(Permission.ModerateMembers, value);
        }

        private void AddAllows(Permission set)
        {
            _permission |= set;
        }

        private void AddDenies(Permission set)
        {
            _permission &= ~set;
        }

        private bool GetPerm(Permission perm)
        {
            return ((_permission & perm) == perm || (_permission & Permission.Administrator) == Permission.Administrator);
        }

        private void SetPerm(Permission perm, bool value)
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

        public static implicit operator Permission(Permissions perms) => perms._permission;

        public static implicit operator Permissions(Permission perms) => new (perms);

        public static Permissions operator +(Permissions item1, Permissions item2)
        {
            return item1._permission | item2._permission;
        }

        public static Permissions operator -(Permissions item1, Permissions item2)
        {
            return item1._permission & ~item2._permission;
        }
    }
}
