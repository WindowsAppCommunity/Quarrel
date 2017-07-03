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
