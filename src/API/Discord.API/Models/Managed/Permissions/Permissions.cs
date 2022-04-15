// Quarrel © 2022

using Discord.API.Models.Enums.Permissions;

namespace Discord.API.Models
{
    /// <summary>
    /// A struct containing permissions.
    /// </summary>
    public struct Permissions
    {
        private Permission _permission;

        internal Permissions(Permission permission)
        {
            _permission = permission;
        }

        /// <inheritdoc cref="Permission.CreateInstantInvite"/>
        public bool CreateInstantInvite
        {
            get => GetPerm(Permission.CreateInstantInvite);
            set => SetPerm(Permission.CreateInstantInvite, value);
        }
        
        /// <inheritdoc cref="Permission.KickMembers"/>
        public bool KickMembers
        {
            get => GetPerm(Permission.KickMembers);
            set => SetPerm(Permission.KickMembers, value);
        }
        
        /// <inheritdoc cref="Permission.BanMembers"/>
        public bool BanMembers
        {
            get => GetPerm(_permission & Permission.BanMembers);
            set => SetPerm(Permission.BanMembers, value);
        }
        
        /// <inheritdoc cref="Permission.Administrator"/>
        public bool Administrator
        {
            get => GetPerm(Permission.Administrator);
            set => SetPerm(Permission.Administrator, value);
        }
        
        /// <inheritdoc cref="Permission.ManageChannels"/>
        public bool ManageChannels
        {
            get => GetPerm(Permission.ManageChannels);
            set => SetPerm(Permission.ManageChannels, value);
        }
        
        /// <inheritdoc cref="Permission.ManangeGuild"/>
        public bool ManangeGuild
        {
            get => GetPerm(Permission.ManangeGuild);
            set => SetPerm(Permission.ManangeGuild, value);
        }
        
        /// <inheritdoc cref="Permission.AddReactions"/>
        public bool AddReactions
        {
            get => GetPerm(Permission.AddReactions);
            set => SetPerm(Permission.AddReactions, value);
        }
        
        /// <inheritdoc cref="Permission.ViewAuditLog"/>
        public bool ViewAuditLog
        {
            get => GetPerm(Permission.ViewAuditLog);
            set => SetPerm(Permission.ViewAuditLog, value);
        }
        
        /// <inheritdoc cref="Permission.PrioritySpeaker"/>
        public bool PrioritySpeaker
        {
            get => GetPerm(Permission.PrioritySpeaker);
            set => SetPerm(Permission.PrioritySpeaker, value);
        }
        
        /// <inheritdoc cref="Permission.Stream"/>
        public bool Stream
        {
            get => GetPerm(Permission.Stream);
            set => SetPerm(Permission.Stream, value);
        }
        
        /// <inheritdoc cref="Permission.ReadMessages"/>
        public bool ReadMessages
        {
            get => GetPerm(Permission.ReadMessages);
            set => SetPerm(Permission.ReadMessages, value);
        }
        
        /// <inheritdoc cref="Permission.SendMessages"/>
        public bool SendMessages
        {
            get => GetPerm(Permission.SendMessages);
            set => SetPerm(Permission.SendMessages, value);
        }
        
        /// <inheritdoc cref="Permission.SendTtsMessages"/>
        public bool SendTtsMessages
        {
            get => GetPerm(Permission.SendTtsMessages);
            set => SetPerm(Permission.SendTtsMessages, value);
        }
        
        /// <inheritdoc cref="Permission.ManageMessages"/>
        public bool ManageMessages
        {
            get => GetPerm(Permission.ManageMessages);
            set => SetPerm(Permission.ManageMessages, value);
        }
        
        /// <inheritdoc cref="Permission.EmbedLinks"/>
        public bool EmbedLinks
        {
            get => GetPerm(Permission.EmbedLinks);
            set => SetPerm(Permission.EmbedLinks, value);
        }
        
        /// <inheritdoc cref="Permission.AttachFiles"/>
        public bool AttachFiles
        {
            get => GetPerm(Permission.AttachFiles);
            set => SetPerm(Permission.AttachFiles, value);
        }
        
        /// <inheritdoc cref="Permission.ReadMessageHistory"/>
        public bool ReadMessageHistory
        {
            get => GetPerm(Permission.ReadMessageHistory);
            set => SetPerm(Permission.ReadMessageHistory, value);
        }
        
        /// <inheritdoc cref="Permission.MentionEveryone"/>
        public bool MentionEveryone
        {
            get => GetPerm(Permission.MentionEveryone);
            set => SetPerm(Permission.MentionEveryone, value);
        }
        
        /// <inheritdoc cref="Permission.UseExternalEmojis"/>
        public bool UseExternalEmojis
        {
            get => GetPerm(Permission.UseExternalEmojis);
            set => SetPerm(Permission.UseExternalEmojis, value);
        }
        
        /// <inheritdoc cref="Permission.Connect"/>
        public bool Connect
        {
            get => GetPerm(Permission.Connect);
            set => SetPerm(Permission.Connect, value);
        }
        
        /// <inheritdoc cref="Permission.Speak"/>
        public bool Speak
        {
            get => GetPerm(Permission.Speak);
            set => SetPerm(Permission.Speak, value);
        }
        
        /// <inheritdoc cref="Permission.MuteMembers"/>
        public bool MuteMembers
        {
            get => GetPerm(Permission.MuteMembers);
            set => SetPerm(Permission.MuteMembers, value);
        }
        
        /// <inheritdoc cref="Permission.DeafenMembers"/>
        public bool DeafenMembers
        {
            get => GetPerm(Permission.DeafenMembers);
            set => SetPerm(Permission.DeafenMembers, value);
        }
        
        /// <inheritdoc cref="Permission.MoveMembers"/>
        public bool MoveMembers
        {
            get => GetPerm(Permission.MoveMembers);
            set => SetPerm(Permission.MoveMembers, value);
        }
        
        /// <inheritdoc cref="Permission.UseVad"/>
        public bool UseVad
        {
            get => GetPerm(Permission.UseVad);
            set => SetPerm(Permission.UseVad, value);
        }
        
        /// <inheritdoc cref="Permission.ChangeNickname"/>
        public bool ChangeNickname
        {
            get => GetPerm(Permission.ChangeNickname);
            set => SetPerm(Permission.ChangeNickname, value);
        }
        
        /// <inheritdoc cref="Permission.ManageNicknames"/>
        public bool ManageNicknames
        {
            get => GetPerm(Permission.ManageNicknames);
            set => SetPerm(Permission.ManageNicknames, value);
        }
        
        /// <inheritdoc cref="Permission.ManageRoles"/>
        public bool ManageRoles
        {
            get => GetPerm(Permission.ManageRoles);
            set => SetPerm(Permission.ManageRoles, value);
        }
        
        /// <inheritdoc cref="Permission.ManageWebhooks"/>
        public bool ManageWebhooks
        {
            get => GetPerm(Permission.ManageWebhooks);
            set => SetPerm(Permission.ManageWebhooks, value);
        }
        
        /// <inheritdoc cref="Permission.ManageEmojis"/>
        public bool ManageEmojis
        {
            get => GetPerm(Permission.ManageEmojis);
            set => SetPerm(Permission.ManageEmojis, value);
        }
        
        /// <inheritdoc cref="Permission.UseApplicationCommands"/>
        public bool UseApplicationCommands
        {
            get => GetPerm(Permission.UseApplicationCommands);
            set => SetPerm(Permission.UseApplicationCommands, value);
        }
        
        /// <inheritdoc cref="Permission.RequestToSpeak"/>
        public bool RequestToSpeak
        {
            get => GetPerm(Permission.RequestToSpeak);
            set => SetPerm(Permission.RequestToSpeak, value);
        }
        
        /// <inheritdoc cref="Permission.ManageThreads"/>
        public bool ManageThreads
        {
            get => GetPerm(Permission.ManageThreads);
            set => SetPerm(Permission.ManageThreads, value);
        }
        
        /// <inheritdoc cref="Permission.CreatePublicThreads"/>
        public bool CreatePublicThreads
        {
            get => GetPerm(Permission.CreatePublicThreads);
            set => SetPerm(Permission.CreatePublicThreads, value);
        }
        
        /// <inheritdoc cref="Permission.CreatePrivateThreads"/>
        public bool CreatePrivateThreads
        {
            get => GetPerm(Permission.CreatePrivateThreads);
            set => SetPerm(Permission.CreatePrivateThreads, value);
        }
        
        /// <inheritdoc cref="Permission.UseExternalStickers"/>
        public bool UseExternalStickers
        {
            get => GetPerm(Permission.UseExternalStickers);
            set => SetPerm(Permission.UseExternalStickers, value);
        }
        
        /// <inheritdoc cref="Permission.SendMessagesInThreads"/>
        public bool SendMessagesInThreads
        {
            get => GetPerm(Permission.SendMessagesInThreads);
            set => SetPerm(Permission.SendMessagesInThreads, value);
        }
        
        /// <inheritdoc cref="Permission.StartEmbeddedActivities"/>
        public bool StartEmbeddedActivities
        {
            get => GetPerm(Permission.StartEmbeddedActivities);
            set => SetPerm(Permission.StartEmbeddedActivities, value);
        }
        
        /// <inheritdoc cref="Permission.ModerateMembers"/>
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

        /// <summary>
        /// Implictly converts a <see cref="Permissions"/> to <see cref="Permission"/>.
        /// </summary>
        public static implicit operator Permission(Permissions perms) => perms._permission;
        
        /// <summary>
        /// Implictly converts a <see cref="Permission"/> to <see cref="Permissions"/>.
        /// </summary>
        public static implicit operator Permissions(Permission perms) => new (perms);

        /// <summary>
        /// Adds the permissions from <paramref name="item2"/> to the permission set of <paramref name="item1"/>.
        /// </summary>
        public static Permissions operator +(Permissions item1, Permissions item2)
        {
            return item1._permission | item2._permission;
        }

        /// <summary>
        /// Removes any permissions from <paramref name="item2"/> from the permission set of <paramref name="item1"/>.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public static Permissions operator -(Permissions item1, Permissions item2)
        {
            return item1._permission & ~item2._permission;
        }
    }
}
