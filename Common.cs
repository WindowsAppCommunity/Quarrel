using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Discord_UWP.SharedModels;

namespace Discord_UWP
{
    public class Common
    {

        public static bool IsYesterday(DateTime dt)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            if (dt >= yesterday && dt < DateTime.Today)
                return true;
            return false;
        }

        // I added this function to make the date nicer to read and to make sure the formatting depends on the local one (not always the US one)
        public static string HumanizeDate(DateTime dt, DateTime? dtPrevious)
        {
            string result = "";
            if (dt.DayOfYear == DateTime.Now.DayOfYear && dt.Year == DateTime.Now.Year)
            {
                if (dtPrevious != null && dtPrevious.Value.DayOfYear == dt.DayOfYear && dtPrevious.Value.Year == dt.Year)
                { result = ""; }
                else
                { result = "Today "; }
            }
            else if (IsYesterday(dt))
            { result = "Yesterday "; }
            else
            {
                var localCulture = new CultureInfo(GlobalizationPreferences.Languages.First());
                result = dt.Date.ToString("d", localCulture) + " ";
            }

            result += "at " + dt.ToString("HH:mm");

            return result;
        }
        public static SolidColorBrush IntToColor(int color)
        {
            byte a = (byte)(255);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public class Permissions
        {
            public Permissions()
            {

            }

            public Permissions GetPermissions(SharedModels.Role input, IEnumerable<SharedModels.Role> roles)
            {
                ServerSidePerms = new PermissionsSave(Convert.ToInt64(input.Permissions));

                foreach (SharedModels.Role role in roles)
                {
                    if (role.Position <= input.Position)
                    {
                        EffectivePerms.AddMerge(Convert.ToInt64(role.Permissions));
                    }
                }
                return this;
            }

            public Permissions GetPermissions(long input)
            {
                ServerSidePerms = new PermissionsSave(Convert.ToInt64(input));
                return this;
            }

            public void AddOverwrites(IEnumerable<SharedModels.Overwrite> overwrites, string guild)
            {
                foreach (SharedModels.Overwrite overwrite in overwrites)
                {
                    switch (overwrite.Type)
                    {
                        case "role":
                            //Find a way to get guild
                            if (Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles != null && (Storage.Cache.Guilds[guild].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id) && Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(overwrite.Id)) || Storage.Cache.Guilds[guild].Roles.ContainsKey(overwrite.Id) && Storage.Cache.Guilds[(guild)].Roles[overwrite.Id].Name == "@everyone")
                            {
                                EffectivePerms.AddMerge(Convert.ToInt64(overwrite.Allow));
                                EffectivePerms.RemoveMerge(Convert.ToInt64(overwrite.Deny));
                            }
                            break;
                        case "member":
                            if (Storage.Cache.CurrentUser.Raw.Id == overwrite.Id)
                            {
                                EffectivePerms.AddMerge(Convert.ToInt64(overwrite.Allow));
                                EffectivePerms.RemoveMerge(Convert.ToInt64(overwrite.Deny));
                            }
                            break;
                    }
                }
            }

            public PermissionsSave EffectivePerms;
            public PermissionsSave ServerSidePerms;
        }

        public struct PermissionsSave
        {
            public PermissionsSave(long perms)
            {
                CreateInstantInvite = Convert.ToBoolean(perms & 0x1);
                KickMembers = Convert.ToBoolean(perms & 0x2);
                BanMembers = Convert.ToBoolean(perms & 0x4);
                Administrator = Convert.ToBoolean(perms & 0x8);
                ManageChannels = Convert.ToBoolean(perms & 0x10);
                ManangeGuild = Convert.ToBoolean(perms & 0x20);
                AddReactions = Convert.ToBoolean(perms & 0x40);
                ReadMessages = Convert.ToBoolean(perms & 0x400);
                SendMessages = Convert.ToBoolean(perms & 0x800);
                SendTtsMessages = Convert.ToBoolean(perms & 0x1000);
                ManageMessages = Convert.ToBoolean(perms & 0x2000);
                EmbedLinks = Convert.ToBoolean(perms & 0x4000);
                AttachFiles = Convert.ToBoolean(perms & 0x8000);
                ReadMessageHistory = Convert.ToBoolean(perms & 0x40000);
                MentionEveryone = Convert.ToBoolean(perms & 0x20000);
                UseExternalEmojis = Convert.ToBoolean(perms & 0x40000);
                Connect = Convert.ToBoolean(perms & 0x100000);
                Speak = Convert.ToBoolean(perms & 0x200000);
                MuteMembers = Convert.ToBoolean(perms & 0x400000);
                DeafenMembers = Convert.ToBoolean(perms & 0x800000);
                MoveMembers = Convert.ToBoolean(perms & 0x1000000);
                UseVad = Convert.ToBoolean(perms & 0x2000000);
                ChangeNickname = Convert.ToBoolean(perms & 0x4000000);
                ManageNicknames = Convert.ToBoolean(perms & 0x8000000);
                ManagerWebhooks = Convert.ToBoolean(perms & 0x20000000);
                ManageEmojis = Convert.ToBoolean(perms & 0x40000000);
                ManageRoles = Convert.ToBoolean(perms & 0x10000000);
            }

            public void AddMerge(long perms)
            {
                CreateInstantInvite = CreateInstantInvite ? true : Convert.ToBoolean(perms & 0x1);
                KickMembers = KickMembers ? true : Convert.ToBoolean(perms & 0x2);
                BanMembers = BanMembers ? true : Convert.ToBoolean(perms & 0x4);
                Administrator = Administrator ? true : Convert.ToBoolean(perms & 0x8);
                ManageChannels = ManageChannels ? true : Convert.ToBoolean(perms & 0x10);
                ManangeGuild = ManangeGuild ? true : Convert.ToBoolean(perms & 0x20);
                AddReactions = AddReactions ? true : Convert.ToBoolean(perms & 0x40);
                ReadMessages = ReadMessages ? true : Convert.ToBoolean(perms & 0x400);
                SendMessages = SendMessages ? true : Convert.ToBoolean(perms & 0x800);
                SendTtsMessages = SendTtsMessages ? true : Convert.ToBoolean(perms & 0x1000);
                ManageMessages = ManageMessages ? true : Convert.ToBoolean(perms & 0x2000);
                EmbedLinks = EmbedLinks ? true : Convert.ToBoolean(perms & 0x4000);
                AttachFiles = AttachFiles ? true : Convert.ToBoolean(perms & 0x8000);
                ReadMessageHistory = ReadMessageHistory ? true : Convert.ToBoolean(perms & 0x40000);
                MentionEveryone = MentionEveryone ? true : Convert.ToBoolean(perms & 0x20000);
                UseExternalEmojis = UseExternalEmojis ? true : Convert.ToBoolean(perms & 0x80000);
                Connect = Connect ? true : Convert.ToBoolean(perms & 0x100000);
                Speak = Speak ? true : Convert.ToBoolean(perms & 0x200000);
                MuteMembers = MuteMembers ? true : Convert.ToBoolean(perms & 0x400000);
                DeafenMembers = DeafenMembers ? true : Convert.ToBoolean(perms & 0x800000);
                MoveMembers = MoveMembers ? true : Convert.ToBoolean(perms & 0x1000000);
                UseVad = UseVad ? true : Convert.ToBoolean(perms & 0x2000000);
                ChangeNickname = ChangeNickname ? true : Convert.ToBoolean(perms & 0x4000000);
                ManageNicknames = ManageNicknames ? true : Convert.ToBoolean(perms & 0x8000000);
                ManagerWebhooks = ManagerWebhooks ? true : Convert.ToBoolean(perms & 0x20000000);
                ManageEmojis = ManageEmojis ? true : Convert.ToBoolean(perms & 0x40000000);
                ManageRoles = ManageRoles ? true : Convert.ToBoolean(perms & 0x10000000);
            }

            public void RemoveMerge(long perms)
            {
                CreateInstantInvite = Convert.ToBoolean(perms & 0x1) ? false : CreateInstantInvite;
                KickMembers = Convert.ToBoolean(perms & 0x2) ? false : KickMembers;
                BanMembers = Convert.ToBoolean(perms & 0x4) ? false : BanMembers;
                Administrator = Convert.ToBoolean(perms & 0x8) ? false : Administrator;
                ManageChannels = Convert.ToBoolean(perms & 0x10) ? false : ManageChannels;
                ManangeGuild = Convert.ToBoolean(perms & 0x20) ? false : ManangeGuild;
                AddReactions = Convert.ToBoolean(perms & 0x40) ? false : AddReactions;
                ReadMessages = Convert.ToBoolean(perms & 0x400) ? false : ReadMessages;
                SendMessages = Convert.ToBoolean(perms & 0x800) ? false : SendMessages;
                SendTtsMessages = Convert.ToBoolean(perms & 0x1000) ? false : SendTtsMessages;
                ManageMessages = Convert.ToBoolean(perms & 0x2000) ? false : ManageMessages;
                EmbedLinks = Convert.ToBoolean(perms & 0x4000) ? false : EmbedLinks;
                AttachFiles = Convert.ToBoolean(perms & 0x8000) ? false : AttachFiles;
                ReadMessageHistory = Convert.ToBoolean(perms & 0x40000) ? false : ReadMessageHistory;
                MentionEveryone = Convert.ToBoolean(perms & 0x20000) ? false : MentionEveryone;
                UseExternalEmojis = Convert.ToBoolean(perms & 0x80000) ? false : UseExternalEmojis;
                Connect = Convert.ToBoolean(perms & 0x100000) ? false : Connect;
                Speak = Convert.ToBoolean(perms & 0x200000) ? false : Speak;
                MuteMembers = Convert.ToBoolean(perms & 0x400000) ? false : MuteMembers;
                DeafenMembers = Convert.ToBoolean(perms & 0x800000) ? false : DeafenMembers;
                MoveMembers = Convert.ToBoolean(perms & 0x1000000) ? false : MoveMembers;
                UseVad = Convert.ToBoolean(perms & 0x2000000) ? false : UseVad;
                ChangeNickname = Convert.ToBoolean(perms & 0x4000000) ? false : ChangeNickname;
                ManageNicknames = Convert.ToBoolean(perms & 0x8000000) ? false : ManageNicknames;
                ManagerWebhooks = Convert.ToBoolean(perms & 0x20000000) ? false : ManagerWebhooks;
                ManageEmojis = Convert.ToBoolean(perms & 0x40000000) ? false : ManageEmojis;
                ManageRoles = Convert.ToBoolean(perms & 0x10000000) ? false : ManageRoles;
            }

            public bool CreateInstantInvite;
            public bool KickMembers;
            public bool BanMembers;
            public bool Administrator;
            public bool ManageChannels;
            public bool ManangeGuild;
            public bool AddReactions;
            public bool ReadMessages;
            public bool SendMessages;
            public bool SendTtsMessages;
            public bool ManageMessages;
            public bool EmbedLinks;
            public bool AttachFiles;
            public bool ReadMessageHistory;
            public bool MentionEveryone;
            public bool UseExternalEmojis;
            public bool Connect;
            public bool Speak;
            public bool MuteMembers;
            public bool DeafenMembers;
            public bool MoveMembers;
            public bool UseVad;
            public bool ChangeNickname;
            public bool ManageNicknames;
            public bool ManagerWebhooks;
            public bool ManageEmojis;
            public bool ManageRoles;
        }
    }
}
