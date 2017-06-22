using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Discord_UWP.CacheModels
{
    [XmlInclude(typeof(TempGuild))]
    [XmlInclude(typeof(TempDmCache))]
    [XmlInclude(typeof(TempUser))]
    public class TempCache
    {
        public TempCache(){}

        public TempCache(Cache input)
        {
            CurrentUser = new TempUser(input.CurrentUser);

            foreach (KeyValuePair<string, Guild> guild in input.Guilds)
            {
                Guilds.Add(new TempGuild(guild.Value));
            }

            foreach (KeyValuePair<string, DmCache> dm in input.DMs)
            {
                DMs.Add(new TempDmCache(dm.Value));
            }
        }

        public TempUser CurrentUser;
        public List<TempGuild> Guilds = new List<TempGuild>();
        public List<TempDmCache> DMs = new List<TempDmCache>();
    }

    class TempCurrentUser
    {
        public TempCurrentUser(SharedModels.User input)
        {
            Raw = input;
            //avatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.Id + "/" + input.Avatar + ".jpg"));
        }

        public TempCurrentUser(TempUser input)
        {
            Raw = input.Raw;

            //avatar = input.avatar;
        }

        public SharedModels.User Raw = new SharedModels.User();
    }

    public class TempUser
    {
        public TempUser(){}
        public TempUser(User input)
        {
            Raw = input.Raw;
            //avatar = input.avatar;
        }

        public SharedModels.User Raw = new SharedModels.User();

        //public ImageBrush avatar = new ImageBrush();
    }

    [XmlInclude(typeof(TempMessage))]
    public class TempDmCache
    {
        public TempDmCache() { }
        public TempDmCache(DmCache input)
        {
            Raw = input.Raw;
            foreach (KeyValuePair<string, Message> message in input.Messages)
            {
                Messages.Add(new TempMessage(message.Value));
            }
        }

        public SharedModels.DirectMessageChannel Raw = new SharedModels.DirectMessageChannel();
        public List<TempMessage> Messages = new List<TempMessage>();
    }
    
    public class TempMember
    {
        public TempMember()
        {

        }
        public TempMember(Member input)
        {
            User = input.Raw.User;
            Nick = input.Raw.Nick;
            Roles = input.Raw.Roles.ToList();
            JoinedAt = input.Raw.JoinedAt;
            Deaf = input.Raw.Deaf;
            Mute = input.Raw.Mute;
            //avatar = input.avatar;
        }

        public SharedModels.User User;
        public string Nick;
        public List<string> Roles;
        public DateTime JoinedAt;
        public bool Deaf;
        public bool Mute;
        
        //public ImageBrush avatar = new ImageBrush();
    }
    
    [XmlInclude(typeof(TempGuildChannel))]
    [XmlInclude(typeof(TempMember))]
    public class TempGuild
    {
        public TempGuild(){}
        public TempGuild(Guild input)
        {
            Id = input.RawGuild.Id;
            Name = input.RawGuild.Name;
            OwnerId = input.RawGuild.OwnerId;
            Icon = input.RawGuild.Icon;
            Splash = input.RawGuild.Splash;
            Region = input.RawGuild.Region;
            if (input.RawGuild.Roles != null)
            {
                Roles = input.RawGuild.Roles.ToList();
            }

            foreach (KeyValuePair<string, GuildChannel> channel in input.Channels)
            {
                Channels.Add(new TempGuildChannel(channel.Value));
            }

            foreach (KeyValuePair<string, Member> user in input.Members)
            {
                Members.Add(new TempMember(user.Value));
            }
        }

        public string Id;
        public string Name;
        public string OwnerId;
        public string Icon;
        public string Splash;
        public string Region;
        public List<SharedModels.Role> Roles;

        public List<TempGuildChannel> Channels = new List<TempGuildChannel>();
        public List<TempMember> Members = new List<TempMember>();
        //ImageBrush icon = new ImageBrush();
    }

    [XmlInclude(typeof(TempMessage))]
    [XmlInclude(typeof(TempMember))]
    public class TempGuildChannel
    {
        public TempGuildChannel(){}
        public TempGuildChannel(GuildChannel input)
        {
            Id = input.Raw.Id;
            GuildId = input.Raw.GuildId;
            Name = input.Raw.Name;
            Type = input.Raw.Type;
            Position = input.Raw.Position;
            IsPrivate = input.Raw.Private;
            Topic = input.Raw.Topic;
            LastMessageId = input.Raw.LastMessageId;
            Overwrites = input.Raw.PermissionOverwrites.ToList();

            foreach (KeyValuePair<string, Message> message in input.Messages)
            {
                Messages.Add(new TempMessage(message.Value));
            }

            foreach (KeyValuePair<string, Message> message in input.PinnedMessages)
            {

                Pinnedmessages.Add(new TempMessage(message.Value));
            }

            foreach (KeyValuePair<string, Member> member in input.Members)
            {
                Members.Add(new TempMember(member.Value));
            }
        }

        public string Id;
        public string GuildId;
        public string Name;
        public string Type;
        public int Position;
        public bool IsPrivate;
        public string Topic;
        public string LastMessageId;
        public List<SharedModels.Overwrite> Overwrites;

        public List<TempMessage> Messages = new List<TempMessage>();
        public List<TempMessage> Pinnedmessages = new List<TempMessage>();
        public List<TempMember> Members = new List<TempMember>();
    }

    [XmlInclude(typeof(TempUser))]
    public class TempMessage
    {
        public TempMessage(){}
        public TempMessage(Message input)
        {
            Id = input.Raw.Id;
            ChannelId = input.Raw.ChannelId;
            Author = input.Raw.User;
            Content = input.Raw.Content;
            Timestamp = input.Raw.Timestamp;
            EditedTimestamp = input.Raw.EditedTimestamp;
            Tts = input.Raw.TTS;
            MentionEveryone = input.Raw.MentionEveryone;
            Mentions = input.Raw.Mentions.ToList();
            Nonce = input.Raw.Nonce;
            Pinned = input.Raw.Pinned;

            User = new TempUser(input.User);
        }

        public string Id;
        public string ChannelId;
        public SharedModels.User Author;
        public string Content;
        public DateTime Timestamp;
        public DateTime? EditedTimestamp;
        public bool Tts;
        public bool MentionEveryone;
        public List<SharedModels.User> Mentions = new List<SharedModels.User>();
        public long? Nonce;
        public bool Pinned;

        public TempUser User;
    }
}
