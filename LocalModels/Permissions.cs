using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.LocalModels
{
    public class Permissions
    {
        public Permissions(int perms)
        {
            Perms = perms;
        }
        
        public void AddAllows(int set)
        {
            Perms = Perms | set;
        }

        public void AddDenies(int set)
        {
            Perms = Perms & ~set;
        }

        public int GetPermInt()
        {
            return Perms;
        }

        public bool CreateInstantInvite
        {
            get { return Convert.ToBoolean(Perms & 0x1); }
            set { Perms = value ? Perms | 0x1 : Perms & ~0x1; }
        }
        public bool KickMembers
        {
            get { return Convert.ToBoolean(Perms & 0x2); }
            set { Perms = value ? Perms | 0x2 : Perms & ~0x2; }
        }
        public bool BanMembers
        {
            get { return Convert.ToBoolean(Perms & 0x4); }
            set { Perms = value ? Perms | 0x4 : Perms & ~0x4; }
        }
        public bool Administrator
        {
            get { return Convert.ToBoolean(Perms & 0x8); }
            set { Perms = value ? Perms | 0x8 : Perms & ~0x8; }
        }
        public bool ManageChannels
        {
            get { return Convert.ToBoolean(Perms & 0x10); }
            set { Perms = value ? Perms | 0x10 : Perms & ~0x10; }
        }
        public bool ManangeGuild
        {
            get { return Convert.ToBoolean(Perms & 0x20); }
            set { Perms = value ? Perms | 0x20 : Perms & ~0x20; }
        }
        public bool AddReactions
        {
            get { return Convert.ToBoolean(Perms & 0x40); }
            set { Perms = value ? Perms | 0x40 : Perms & ~0x40; }
        }
        public bool ViewAuditLog
        {
            get { return Convert.ToBoolean(Perms & 0x80); }
            set { Perms = value ? Perms | 0x80 : Perms & ~0x80; }
        }
        public bool ReadMessages
        {
            get { return Convert.ToBoolean(Perms & 0x400); }
            set { Perms = value ? Perms | 0x400 : Perms & ~0x400; }
        }
        public bool SendMessages
        {
            get { return Convert.ToBoolean(Perms & 0x800); }
            set { Perms = value ? Perms | 0x800 : Perms & ~0x800; }
        }
        public bool SendTtsMessages
        {
            get { return Convert.ToBoolean(Perms & 0x1000); }
            set { Perms = value ? Perms | 0x1000 : Perms & ~0x1000; }
        }
        public bool ManageMessages
        {
            get { return Convert.ToBoolean(Perms & 0x2000); }
            set { Perms = value ? Perms | 0x2000 : Perms & ~0x2000; }
        }
        public bool EmbedLinks
        {
            get { return Convert.ToBoolean(Perms & 0x4000); }
            set { Perms = value ? Perms | 0x4000 : Perms & ~0x4000; }
        }
        public bool AttachFiles
        {
            get { return Convert.ToBoolean(Perms & 0x8000); }
            set { Perms = value ? Perms | 0x8000 : Perms & ~0x8000; }
        }
        public bool ReadMessageHistory
        {
            get { return Convert.ToBoolean(Perms & 0x10000); }
            set { Perms = value ? Perms | 0x10000 : Perms & ~0x10000; }
        }
        public bool MentionEveryone
        {
            get { return Convert.ToBoolean(Perms & 0x20000); }
            set { Perms = value ? Perms | 0x20000 : Perms & ~0x20000; }
        }
        public bool UseExternalEmojis
        {
            get { return Convert.ToBoolean(Perms & 0x40000); }
            set { Perms = value ? Perms | 0x40000 : Perms & ~0x40000; }
        }
        public bool Connect
        {
            get { return Convert.ToBoolean(Perms & 0x100000); }
            set { Perms = value ? Perms | 0x100000 : Perms & ~0x100000; }
        }
        public bool Speak
        {
            get { return Convert.ToBoolean(Perms & 0x200000); }
            set { Perms = value ? Perms | 0x200000 : Perms & ~0x200000; }
        }
        public bool MuteMembers
        {
            get { return Convert.ToBoolean(Perms & 0x400000); }
            set { Perms = value ? Perms | 0x400000 : Perms & ~0x400000; }
        }
        public bool DeafenMembers
        {
            get { return Convert.ToBoolean(Perms & 0x800000); }
            set { Perms = value ? Perms | 0x800000 : Perms & ~0x800000; }
        }
        public bool MoveMembers
        {
            get { return Convert.ToBoolean(Perms & 0x1000000); }
            set { Perms = value ? Perms | 0x1000000 : Perms & ~0x1000000; }
        }
        public bool UseVad
        {
            get { return Convert.ToBoolean(Perms & 0x2000000); }
            set { Perms = value ? Perms | 0x2000000 : Perms & ~0x2000000; }
        }
        public bool ChangeNickname
        {
            get { return Convert.ToBoolean(Perms & 0x4000000); }
            set { Perms = value ? Perms | 0x4000000 : Perms & ~0x4000000; }
        }
        public bool ManageNicknames
        {
            get { return Convert.ToBoolean(Perms & 0x8000000); }
            set { Perms = value ? Perms | 0x8000000 : Perms & ~0x8000000; }
        }
        public bool ManageRoles
        {
            get { return Convert.ToBoolean(Perms & 0x10000000); }
            set { Perms = value ? Perms | 0x10000000 : Perms & ~0x10000000; }
        }
        public bool ManageWebhooks
        {
            get { return Convert.ToBoolean(Perms & 0x20000000); }
            set { Perms = value ? Perms | 0x20000000 : Perms & ~0x20000000; }
        }
        public bool ManageEmojis
        {
            get { return Convert.ToBoolean(Perms & 0x40000000); }
            set { Perms = value ? Perms | 0x40000000 : Perms & ~0x40000000; }
        }

        int Perms;
    }
}
