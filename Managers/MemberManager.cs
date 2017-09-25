using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Discord_UWP.Managers
{
    class MemberManager
    {
    }

    public class Member
    {
        public SharedModels.GuildMember Raw;

        public SharedModels.Role HighRole = new SharedModels.Role();
        public bool IsTyping { get; set; }
        public DisplayedRole MemberDisplayedRole { get; set; }
        public SharedModels.Presence status { get; set; }
        public SharedModels.VoiceState voicestate { get; set; }
    }

    public class DisplayedRole
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Membercount { get; set; }
        public SolidColorBrush Brush { get; set; }

        public DisplayedRole(string id, int position, string name, int membercount, SolidColorBrush brush)
        { Id = id; Position = position; Name = name; Membercount = membercount; Brush = brush; }
    }
}
