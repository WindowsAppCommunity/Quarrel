using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SimpleClasses
{
    public enum MessageTypes { Default, RecipientAdded, RecipientRemoved, Call, ChannelNameChanged, ChannelIconChanged, PinnedMessage, GuildMemberJoined, Advert }
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
        public Message Message
        {
            get => _message;
            set { if (Equals(_message, value)) return; _message = value; OnPropertyChanged("Message"); }
        }

        private bool _iscontinuation;
        public bool IsContinuation
        {
            get => _iscontinuation;
            set { if (_iscontinuation == value) return; _iscontinuation = value; OnPropertyChanged("IsContinuation"); }
        }

        private MessageTypes _msgtype;
        public MessageTypes MessageType
        {
            get => _msgtype;
            set { if (_msgtype == value) return; _msgtype = value; OnPropertyChanged("MessageType"); }
        }


        private string _header;
        public string Header
        {
            get => _header;
            set { if (_header == value) return; _header = value; OnPropertyChanged("Header"); }
        }

        private bool _lastRead;
        public bool LastRead
        {
            get => _lastRead;
            set { if (_lastRead == value) return; _lastRead = value; OnPropertyChanged("LastRead"); }
        }

        private bool _pending;
        public bool Pending
        {
            get => _pending;
            set { if (_pending == value) return; _pending = value; OnPropertyChanged("Pending"); }
        }

        private bool _blocked;
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
        /* This is a really ugly class, but it's necessary to have the values update correctly */

        //id = channel id
        //name = the displayed name of the channel
        //imageurl = the URL of the image displayed for DMs
        //notificationcount = the amount of pending notifications in this channel
        //isunread = is the unread messages indicator visible?
        //ismuted = is the channel muted?
        //isdm = if it is the DM control

        private string _id;
        public string Id
        {
            get { return _id; }
            set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
        }

        private string _lastmessageId;
        public string TempLastMessageId
        {
            get { return _lastmessageId; }
            set { if (_lastmessageId == value) return; _lastmessageId = value; OnPropertyChanged("TempLastMessageId"); }
        }

        private string _imageurl;
        public string ImageURL
        {
            get { return _imageurl; }
            set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
        }

        private int _notificationcount;
        public int NotificationCount
        {
            get { return _notificationcount; }
            set { if (_notificationcount == value) return; _notificationcount = value; OnPropertyChanged("NotificationCount"); }
        }

        private bool _isunread;
        public bool IsUnread
        {
            get { return _isunread; }
            set { if (_isunread == value) return; _isunread = value; OnPropertyChanged("IsUnread"); }
        }

        private bool _ismuted;
        public bool IsMuted
        {
            get { return _ismuted; }
            set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
        }

        private bool _isdm;
        public bool IsDM
        {
            get { return _isdm; }
            set { if (_isdm == value) return; _isdm = value; OnPropertyChanged("IsDM"); }
        }

        private bool _isvalid;
        public bool IsValid
        {
            get { return _isvalid; }
            set { if (_isvalid == value) return; _isvalid = value; OnPropertyChanged("IsValid"); }
        }

        private bool _isselected;
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
            sg.TempLastMessageId = TempLastMessageId;
            return sg;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
    public class SimpleChannel : INotifyPropertyChanged
    {
        /* This is a really ugly class, but it's necessary to have the values update correctly */

        //id = channel id
        //name = the displayed name of the channel
        //userstatus = the displayed status (online, idle, etc...) as an int
        //subtitle = the text shown below, such as the member count for group DMs or the "Playing __" for DMs
        //imageurl = the URL of the image displayed for DMs
        //type = the type of Channel: 0=text, 1=DM, 2=Voice, 3=GroupDM
        //notificationcount = the amount of pending notifications in this channel
        //isunread = is the unread messages indicator visible?
        //istyping = is someone typing in that channel?
        //ismuted = is the channel muted?

        private string _id;
        public string Id
        {
            get { return _id; }
            set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
        }

        private string _userid;
        public string UserId
        {
            get { return _userid; }
            set { if (_userid == value) return; _userid = value; OnPropertyChanged("UserId"); }
        }

        private string _lastmessageid;
        public string LastMessageId
        {
            get { return _lastmessageid; }
            set { if (_lastmessageid == value) return; _lastmessageid = value; OnPropertyChanged("LastMessageId"); }
        }

        private string _parentid;
        public string ParentId
        {
            get { return _parentid; }
            set { if (_parentid == value) return; _parentid = value; OnPropertyChanged("ParentId"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
        }

        private Presence _userstatus;
        public Presence UserStatus
        {
            get { return _userstatus; }
            set { if (_userstatus == value) return; _userstatus = value; OnPropertyChanged("UserStatus"); }
        }

        private string _subtitle;
        public string Subtitle
        {
            get { return _subtitle; }
            set { if (_subtitle == value) return; _subtitle = value; OnPropertyChanged("Subtitle"); }
        }

        private Game _playing;
        public Game Playing
        {
            get { return _playing; }
            set { if (_playing != null && value != null && _playing.Name == value.Name) return; _playing = value; OnPropertyChanged("Playing"); }
        }

        private string _imageurl;
        public string ImageURL
        {
            get { return _imageurl; }
            set { if (_imageurl == value) return; _imageurl = value; OnPropertyChanged("ImageURL"); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { if (_icon == value) return; _icon = value; OnPropertyChanged("Icon"); }
        }

        private bool _isselected;
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
        public int Position
        {
            get { return _position; }
            set { if (_position == value) return; _position = value; OnPropertyChanged("Position"); }
        }

        private int _notificationcount;
        public int NotificationCount
        {
            get { return _notificationcount; }
            set { if (_notificationcount == value) return; _notificationcount = value; OnPropertyChanged("NotificationCount"); }
        }

        private bool _isunread;
        public bool IsUnread
        {
            get { return _isunread; }
            set { if (_isunread == value) return; _isunread = value; OnPropertyChanged("IsUnread"); }
        }

        private bool _istyping;
        public bool IsTyping
        {
            get { return _istyping; }
            set { if (_istyping == value) return; _istyping = value; OnPropertyChanged("IsTyping"); }
        }

        private bool _ismuted;
        public bool IsMuted
        {
            get { return _ismuted; }
            set { if (_ismuted == value) return; _ismuted = value; OnPropertyChanged("IsMuted"); }
        }

        private bool _nsfw;
        public bool Nsfw
        {
            get { return _nsfw; }
            set { if (_nsfw == value) return; _nsfw = value; OnPropertyChanged("Nsfw"); }
        }

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { if (_hidden == value) return; _hidden = value; OnPropertyChanged("Hidden"); }
        }

        private Dictionary<string, User> _members;
        public Dictionary<string, User> Members
        {
            get { return _members; }
            set { if (_members == value) return; _members = value; OnPropertyChanged("Members"); }
        }

        private List<VoiceState> _voiceMembers;
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
        public Member(SharedModels.GuildMember input)
        {
            Raw = input;
            //avatar = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + input.User.Id + "/" + input.User.Avatar + ".jpg")) };
        }


        public SharedModels.GuildMember _raw;
        public SharedModels.GuildMember Raw
        {
            get { return _raw; }
            set { if (_raw != null && _raw.Equals(value)) return; _raw = value; OnPropertyChanged("Raw"); }
        }

        public bool _istyping;
        public bool IsTyping
        {
            get { return _istyping; }
            set { if (_istyping == value) return; _istyping = value; OnPropertyChanged("IsTyping"); }
        }

        public string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { if (_displayname == value) return; _displayname = value; OnPropertyChanged("DisplayName"); }
        }

        public HoistRole _memberhoistrole;
        public HoistRole MemberHoistRole
        {
            get { return _memberhoistrole; }
            set { if (_memberhoistrole != null && _memberhoistrole.Equals(value)) return; _memberhoistrole = value; OnPropertyChanged("MemberHoistRole"); }
        }

        public SharedModels.Presence _status;
        public SharedModels.Presence status
        {
            get { return _status; }
            set { if (_status == value) return; _status = value; OnPropertyChanged("status"); }
        }

        public SharedModels.VoiceState _voicestate;
        public SharedModels.VoiceState voicestate
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
        public string Id { get; set; }
        public int _position;
        public int Position
        {
            get { return _position; }
            set { if (_position.Equals(value)) return; _position = value; OnPropertyChanged("Position"); }
        }

        public string _name;
        public string Name
        {
            get { return _name; }
            set { if (_name != null && _name.Equals(value)) return; _name = value; OnPropertyChanged("Name"); }
        }

        public int _membercount;
        public int Membercount
        {
            get { return _membercount; }
            set { if (_membercount.Equals(value)) return; _membercount = value; OnPropertyChanged("Membercount"); }
        }

        public int _brush;
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
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
