using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using GuildChannel = DiscordAPI.SharedModels.GuildChannel;

// TODO: Replace with proper MVVM
namespace Quarrel.SimpleClasses
{
    /// <summary>
    /// Types of Messages
    /// </summary>
    public enum MessageTypes
    {
        Default,
        RecipientAdded,
        RecipientRemoved,
        Call,
        ChannelNameChanged,
        ChannelIconChanged,
        PinnedMessage,
        GuildMemberJoined,
        Advert
    }

    public class MessageContainer : INotifyPropertyChanged
    {
        public MessageContainer(Message message, MessageTypes messageType, bool isContinuation, string header, bool pending = false)
        {
            LastRead = false; // this is false, we decided if it should be marked as "last read" later on
            Message = message;
            MessageType = messageType;
            if (LastRead)
                IsContinuation = false; //If a message has a "New messages" indicator, don't show it as a continuation message
            else
                IsContinuation = isContinuation;
            Header = header;
            Pending = pending;
            Blocked = messageType != MessageTypes.Advert && LocalState.Blocked.ContainsKey(message.Id);
        }
        
        private Message _message;
        /// <summary>
        /// API Message data
        /// </summary>
        public Message Message
        {
            get => _message;
            set { if (Equals(_message, value)) return; _message = value; OnPropertyChanged("Message"); }
        }

        private bool _edit;
        /// <summary>
        /// Wheather or not the message is being edited
        /// </summary>
        public bool Edit
        {
            get => _edit;
            set { if (_edit == value) return; _edit = value; OnPropertyChanged("Edit"); }
        }

        private bool _iscontinuation;
        /// <summary>
        /// Wheather or not the message is an extension on the message before
        /// </summary>
        public bool IsContinuation
        {
            get => _iscontinuation;
            set { if (_iscontinuation == value) return; _iscontinuation = value; OnPropertyChanged("IsContinuation"); }
        }

        private MessageTypes _msgtype;
        /// <summary>
        /// Type of the message
        /// </summary>
        public MessageTypes MessageType
        {
            get => _msgtype;
            set { if (_msgtype == value) return; _msgtype = value; OnPropertyChanged("MessageType"); }
        }

        // Depricated
        private string _header;
        /// <summary>
        /// Header on message. For example "New Messages"
        /// </summary>
        public string Header
        {
            get => _header;
            set { if (_header == value) return; _header = value; OnPropertyChanged("Header"); }
        }

        private bool _lastRead;
        /// <summary>
        /// If the message before was the LastRead before the channel was opened
        /// </summary>
        public bool LastRead
        {
            get => _lastRead;
            set { if (_lastRead == value) return; _lastRead = value; OnPropertyChanged("LastRead"); }
        }
        
        // Depricated
        private bool _pending;
        /// <summary>
        /// True if the message has not yet finished sending
        /// </summary>
        public bool Pending
        {
            get => _pending;
            set { if (_pending == value) return; _pending = value; OnPropertyChanged("Pending"); }
        }

        //Depricated
        private bool _blocked;
        /// <summary>
        /// True if the user that sent the message is blocked by the current user
        /// </summary>
        public bool Blocked
        {
            get => _blocked;
            set { if (_blocked == value) return; _blocked = value; OnPropertyChanged("Blocked"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SimpleGuild : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// The ID of the Guild
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
        }

        private string _name;
        /// <summary>
        /// The Name of the Guild
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
        }

        private string _imageurl;
        /// <summary>
        /// URL for the Guild Icon
        /// </summary>
        public string ImageURL
        {
            get { return _imageurl; }
            set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
        }

        private int _notificationcount;
        /// <summary>
        /// Number of notification in the Guild
        /// </summary>
        public int NotificationCount
        {
            get { return _notificationcount; }
            set { if (_notificationcount == value) return; _notificationcount = value; OnPropertyChanged("NotificationCount"); }
        }

        private bool _isunread;
        /// <summary>
        /// True if an unmuted channel in the Guild has an unread message
        /// </summary>
        public bool IsUnread
        {
            get { return _isunread; }
            set { if (_isunread == value) return; _isunread = value; OnPropertyChanged("IsUnread"); }
        }

        private bool _ismuted;
        /// <summary>
        /// True if the Guild is muted
        /// </summary>
        public bool IsMuted
        {
            get { return _ismuted; }
            set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
        }

        private bool _isdm;
        /// <summary>
        /// True if the Guild item is just the DM item
        /// </summary>
        public bool IsDM
        {
            get { return _isdm; }
            set { if (_isdm == value) return; _isdm = value; OnPropertyChanged("IsDM"); }
        }

        // Depricated
        private bool _isvalid;
        /// <summary>
        /// False if the Server is down
        /// </summary>
        public bool IsValid
        {
            get { return _isvalid; }
            set { if (_isvalid == value) return; _isvalid = value; OnPropertyChanged("IsValid"); }
        }

        private bool _isselected;
        /// <summary>
        /// True if the Guild Item is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isselected; }
            set { if (_isselected == value) return; _isselected = value; OnPropertyChanged("IsSelected"); }
        }

        public SimpleGuild Clone()
        {
            SimpleGuild sg = new SimpleGuild();
            sg.Id = Id;
            sg.ImageURL = ImageURL;
            sg.IsDM = IsDM;
            sg.IsMuted = IsMuted;
            sg.IsUnread = IsUnread;
            sg.Name = Name;
            sg.NotificationCount = NotificationCount;
            sg.IsValid = IsValid;
            sg.IsSelected = IsSelected;
            return sg;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class SimpleChannel : INotifyPropertyChanged
    {
        public void Update(GuildChannel guildChannel)
        {
            _id = guildChannel.Id;
            _name = guildChannel.Name;
            if (guildChannel.Type == 0)
            {
                //TODO: Topic subtitle
            }
            _imageurl = guildChannel.Icon;
            _type = guildChannel.Type;
            _istyping = LocalState.Typers.ContainsKey(guildChannel.Id);
            _ismuted = !App.CurrentGuildIsDM &&
                LocalState.GuildSettings.ContainsKey(guildChannel.GuildId)
                && LocalState.GuildSettings[guildChannel.GuildId].channelOverrides.ContainsKey(guildChannel.Id)
                && LocalState.GuildSettings[guildChannel.GuildId].channelOverrides[guildChannel.Id].Muted;
        }

        public void Update(DirectMessageChannel dmChannel)
        {
            _id = dmChannel.Id;
            _name = dmChannel.Name;
            if (dmChannel.Type == 3)
            {
                _subtitle = App.GetString("/Main/members").Replace("<count>", dmChannel.Users.Count().ToString());
            }
            _imageurl = dmChannel.Icon;
            _type = dmChannel.Type;
            _istyping = LocalState.Typers.ContainsKey(dmChannel.Id);
        }

        private string _id;
        /// <summary>
        /// The ID of the channel
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
        }

        private string _userid;
        /// <summary>
        /// The ID of the user if channel is a DM
        /// </summary>
        public string UserId
        {
            get { return _userid; }
            set { if (_userid == value) return; _userid = value; OnPropertyChanged("UserId"); }
        }

        private string _lastmessageid;
        /// <summary>
        /// The ID of the last message in this channel
        /// </summary>
        public string LastMessageId
        {
            get { return _lastmessageid; }
            set { if (_lastmessageid == value) return; _lastmessageid = value; OnPropertyChanged("LastMessageId"); }
        }

        private string _parentid;
        /// <summary>
        /// The ID of the parent Channel Group
        /// </summary>
        public string ParentId
        {
            get { return _parentid; }
            set { if (_parentid == value) return; _parentid = value; OnPropertyChanged("ParentId"); }
        }

        private string _name;
        /// <summary>
        /// The name of the channel
        /// </summary>
        public string Name
        {
            get { return String.IsNullOrEmpty(_name) ? "Unnamed" : _name ; }
            set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
        }

        private Presence _userstatus;
        /// <summary>
        /// Presence of the User if the channel is a DM
        /// </summary>
        public Presence UserStatus
        {
            get { return _userstatus; }
            set { if (_userstatus == value) return; _userstatus = value; OnPropertyChanged("UserStatus"); }
        }

        private string _subtitle;
        /// <summary>
        /// Subtitle for channel (for example: Member Count in Group DM)
        /// </summary>
        public string Subtitle
        {
            get { return _subtitle; }
            set { if (_subtitle == value) return; _subtitle = value; OnPropertyChanged("Subtitle"); }
        }

        private Game _playing;
        /// <summary>
        /// The Game the user is playing if the channel is a DM
        /// </summary>
        public Game Playing
        {
            get { return _playing; }
            set { if (_playing != null && value != null && _playing.Name == value.Name) return; _playing = value; OnPropertyChanged("Playing"); }
        }

        private string _imageurl;
        /// <summary>
        /// URL of user avatar if the channel is a DM
        /// </summary>
        public string ImageURL
        {
            get { return _imageurl; }
            set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
        }

        private string _icon;
        /// <summary>
        /// URL of group icon if the channel is a Group DM
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { if (_icon == value) return; _icon = value; OnPropertyChanged("Icon"); }
        }

        private bool _havepermissions;
        /// <summary>
        /// True if the user has Read Permsissions on the channel
        /// </summary>
        public bool HavePermissions
        {
            get { return _havepermissions; }
            set { if (_havepermissions == value) return; _havepermissions = value; OnPropertyChanged("HavePermissions"); }
        }

        private bool _isselected;
        /// <summary>
        /// True if the channel is currently selected by the user
        /// </summary>
        public bool IsSelected
        {
            get { return _isselected; }
            set { if (_isselected == value) return; _isselected = value; OnPropertyChanged("IsSelected"); }
        }

        private int _type;
        /// <summary>
        /// 0: Text channel
        /// 1: Direct Message
        /// 2: Voice channel
        /// 3: Group DM
        /// 4: Guild category
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { if (_type == value) return; _type = value; OnPropertyChanged("Type"); }
        }

        private int _position;
        /// <summary>
        /// Postion in channel list
        /// </summary>
        public int Position
        {
            get { return _position; }
            set { if (_position == value) return; _position = value; OnPropertyChanged("Position"); }
        }

        private int _notificationcount;
        /// <summary>
        /// Number of notifcations for channel
        /// </summary>
        public int NotificationCount
        {
            get { return _notificationcount; }
            set { if (_notificationcount == value) return; _notificationcount = value; OnPropertyChanged("NotificationCount"); }
        }

        private bool _isunread;
        /// <summary>
        /// True if latest message in the channel is unread
        /// </summary>
        public bool IsUnread
        {
            get { return _isunread; }
            set { if (_isunread == value) return; _isunread = value; OnPropertyChanged("IsUnread"); }
        }

        private bool _istyping;
        /// <summary>
        /// True if someone is typing in the channel
        /// </summary>
        public bool IsTyping
        {
            get { return _istyping; }
            set { if (_istyping == value) return; _istyping = value; OnPropertyChanged("IsTyping"); }
        }

        private bool _ismuted;
        /// <summary>
        /// True if the channel is muted
        /// </summary>
        public bool IsMuted
        {
            get { return _ismuted; }
            set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
        }

        private bool _nsfw;
        /// <summary>
        /// True if the channel is marked NSFW
        /// </summary>
        public bool Nsfw
        {
            get { return _nsfw; }
            set { if (_nsfw == value) return; _nsfw = value; OnPropertyChanged("Nsfw"); }
        }

        private bool _hidden;
        /// <summary>
        /// True if the channel group is collapsed
        /// </summary>
        public bool Hidden
        {
            get { return _hidden; }
            set { if (_hidden == value) return; _hidden = value; OnPropertyChanged("Hidden"); }
        }

        private Dictionary<string, User> _members;
        /// <summary>
        /// List members if the channel is a DM
        /// </summary>
        public Dictionary<string, User> Members
        {
            get { return _members; }
            set { if (_members == value) return; _members = value; OnPropertyChanged("Members"); }
        }

        private List<VoiceState> _voiceMembers;
        /// <summary>
        /// List of Voice Status if the channel is a Voice Channel
        /// </summary>
        public List<VoiceState> VoiceMembers
        {
            get { return _voiceMembers; }
            set { if (_voiceMembers == value) return; _voiceMembers = value; OnPropertyChanged("VoiceMembers"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class Member : INotifyPropertyChanged
    {
        public Member(GuildMember input)
        {
            Raw = input;
            //avatar = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.User.Id + "/" + input.User.Avatar + ".jpg")) };
        }


        public GuildMember _raw;
        /// <summary>
        /// The API data of the Member
        /// </summary>
        public GuildMember Raw
        {
            get { return _raw; }
            set { if (_raw != null && _raw.Equals(value)) return; _raw = value; OnPropertyChanged("Raw"); }
        }

        public bool _istyping;
        /// <summary>
        /// True if that user is typing somewhere on the server
        /// </summary>
        public bool IsTyping
        {
            get { return _istyping; }
            set { if (_istyping == value) return; _istyping = value; OnPropertyChanged("IsTyping"); }
        }

        public string _displayname;
        /// <summary>
        /// The Name or Nickname of the Member
        /// </summary>
        public string DisplayName
        {
            get { return _displayname; }
            set { if (_displayname == value) return; _displayname = value; OnPropertyChanged("DisplayName"); }
        }

        public HoistRole _memberhoistrole;
        /// <summary>
        /// The highest role of the member
        /// </summary>
        public HoistRole MemberHoistRole
        {
            get { return _memberhoistrole; }
            set { if (_memberhoistrole != null && _memberhoistrole.Equals(value)) return; _memberhoistrole = value; OnPropertyChanged("MemberHoistRole"); }
        }

        public Presence _status;
        /// <summary>
        /// The Presense (online + game) of the member
        /// </summary>
        public Presence status
        {
            get { return _status; }
            set { if (_status == value) return; _status = value; OnPropertyChanged("status"); }
        }

        public VoiceState _voicestate;
        /// <summary>
        /// The VoiceState of the user
        /// </summary>
        public VoiceState voicestate
        {
            get { return _voicestate; }
            set { if (_voicestate.Equals(value)) return; _voicestate = value; OnPropertyChanged("voicestate"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class HoistRole : IComparable<HoistRole>, INotifyPropertyChanged
    {
        /// <summary>
        /// The ID of the role
        /// </summary>
        public string Id { get; set; }

        public int _position;
        /// <summary>
        /// The Postion of the role (smaller is higher)
        /// </summary>
        public int Position
        {
            get { return _position; }
            set { if (_position.Equals(value)) return; _position = value; OnPropertyChanged("Position"); }
        }

        public string _name;
        /// <summary>
        /// The name of the role
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { if (_name != null && _name.Equals(value)) return; _name = value; OnPropertyChanged("Name"); }
        }

        public int _membercount;
        /// <summary>
        /// The number of people that have the role
        /// </summary>
        public int Membercount
        {
            get { return _membercount; }
            set { if (_membercount.Equals(value)) return; _membercount = value; OnPropertyChanged("Membercount"); }
        }

        public int _brush;
        /// <summary>
        /// Int color of the role
        /// </summary>
        public int Brush
        {
            get { return _brush; }
            set { if (_brush.Equals(value)) return; _brush = value; OnPropertyChanged("Brush"); }
        }

        public HoistRole(string id, int position, string name, int membercount, int brush)
        { Id = id; Position = position; Name = name; Membercount = membercount; Brush = brush; }

        public int CompareTo(HoistRole obj)
        {
            if (obj != null && obj.Id == Id)
                return 0;
            else
                return 1;
        }
        public override bool Equals(object obj)
        {

            if (obj != null && obj.GetType() == typeof(HoistRole) && ((HoistRole)obj).Id == Id)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
            //TODO Why is this called from another thread???
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
