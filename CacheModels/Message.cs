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
    public class Message
    {
        public Message(SharedModels.Message input)
        {
            Raw = input;
            User = new User(Raw.User);
        }

        public Message(TempMessage input)
        {
            Raw.Id = input.Id;
            Raw.ChannelId = input.ChannelId;
            Raw.User = input.Author;
            Raw.Content = input.Content;
            Raw.Timestamp = input.Timestamp;
            Raw.EditedTimestamp = input.EditedTimestamp;
            Raw.TTS = input.Tts;
            Raw.MentionEveryone = input.MentionEveryone;
            Raw.Mentions = input.Mentions.AsEnumerable();
            Raw.Nonce = input.Nonce;
            Raw.Pinned = input.Pinned;

            User = new User(input.User);
        }

        public SharedModels.Message Raw = new SharedModels.Message();
        public User User;
    }
}
