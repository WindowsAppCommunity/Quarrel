using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.CacheModels
{
    public class GuildChannel : INotifyPropertyChanged
    {
        public GuildChannel(SharedModels.GuildChannel input)
        {
            Raw = input;
            if (Storage.Cache.Guilds[input.GuildId].RawGuild.Roles != null)
            {
                foreach (SharedModels.Role role in Storage.Cache.Guilds[input.GuildId].RawGuild.Roles)
                {
                    if (Storage.Cache.Guilds[input.GuildId].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                    {
                        if (Storage.Cache.Guilds[input.GuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[input.GuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                        {
                            chnPerms.GetPermissions(role, Storage.Cache.Guilds[input.GuildId].RawGuild.Roles);
                        }
                        else
                        {
                            chnPerms.GetPermissions(0);
                        }
                    }
                }
                chnPerms.AddOverwrites(input.PermissionOverwrites, input.GuildId);
            }
        }

        public GuildChannel(TempGuildChannel input)
        {
            var raw = new SharedModels.GuildChannel();
            raw.Id = input.Id;
            raw.GuildId = input.GuildId;
            raw.Name = input.Name;
            raw.Type = input.Type;
            raw.Position = input.Position;
            raw.Private = input.IsPrivate;
            raw.Topic = input.Topic;
            raw.LastMessageId = input.LastMessageId;
            raw.PermissionOverwrites = input.Overwrites;
            Raw = raw;
            chnPerms = input.chnPerms;

            foreach (TempMessage message in input.Messages)
            {
                Messages.Add(message.Id, new Message(message));
            }

            foreach (TempMessage message in input.Pinnedmessages)
            {
                PinnedMessages.Add(message.Id, new Message(message));
            }

            foreach (TempMember member in input.Members)
            {
                Members.Add(member.User.Id, new Member(member));
            }
        }

        public void ChangeChannelName(string name)
        {
            //
            var RawAlt = Raw;
            RawAlt.Name = name;
            Raw = RawAlt;
            OnPropertyChanged("Raw");
        }

        public SharedModels.GuildChannel Raw;

        public Dictionary<string, Message> Messages = new Dictionary<string, Message>();
        public Dictionary<string, Message> PinnedMessages = new Dictionary<string, Message>();
        public Dictionary<string, Member> Members = new Dictionary<string, Member>();
        public Common.Permissions chnPerms = new Common.Permissions();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
