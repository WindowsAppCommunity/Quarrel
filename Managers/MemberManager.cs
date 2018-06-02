using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System.ComponentModel;

namespace Discord_UWP.Managers
{
    class MemberManager
    {
        private static List<HoistRole> TempRoleCache = new List<HoistRole>(); //This is as a temporary cache of roles to improve performance and not call Storage for every member
        public static HoistRole GetRole(string roleid, string guildid)
        {
            var cachedRole = TempRoleCache.FirstOrDefault(x => x.Id == roleid);
            if (cachedRole != null) return cachedRole;
            else
            {
                HoistRole role;
                if (roleid == null || !LocalState.Guilds[guildid].roles[roleid].Hoist)
                {

                    role = new HoistRole(null, 0, App.GetString("/Main/Everyone"), 0, (SolidColorBrush)App.Current.Resources["Foreground"]);
                    TempRoleCache.Add(role);
                }
                else
                {
                    var storageRole = LocalState.Guilds[guildid].roles[roleid];
                    role = new HoistRole(roleid, storageRole.Position, storageRole.Name.ToUpper(), storageRole.MemberCount, Common.IntToColor(storageRole.Color));
                    TempRoleCache.Add(role);
                }
                return role;
            }
        }
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
            set { if (_status.Equals(value)) return; _status = value; OnPropertyChanged("status"); }
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

        public SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get { return _brush; }
            set { if (_brush!=null && _brush.Equals(value)) return; _brush = value; OnPropertyChanged("Brush"); }
        }

        public HoistRole(string id, int position, string name, int membercount, SolidColorBrush brush)
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
