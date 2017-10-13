using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Managers
{
    public class MessageManager
    {
        public static List<MessageContainer> ConvertMessage(List<Message> messages)
        {
            if (messages != null)
            {
                int adCheck = 5;
                List<MessageContainer> returnMessages = new List<MessageContainer>();
                foreach (var message in messages)
                {
                    returnMessages.Add(new MessageContainer(message, GetMessageType(message.Type), false, null)); //TODO: isConinuation
                    adCheck--;
                    if (adCheck == 0 && App.ShowAds)
                    {
                        returnMessages.Add(new MessageContainer(null, MessageTypes.Advert, false, null));
                        adCheck = 5;
                    }
                }
                returnMessages.Reverse();
                return returnMessages;
            }
            return null; //else
        }
        public static MessageTypes GetMessageType(int type)
        {
            switch (type)
            {
                case 0: return MessageTypes.Default;
                case 1: return MessageTypes.RecipientAdded;
                case 2: return MessageTypes.RecipientRemoved;
                case 3: return MessageTypes.Call;
                case 4: return MessageTypes.ChannelNameChanged; 
                case 5: return MessageTypes.ChannelIconChanged;
                case 6: return MessageTypes.PinnedMessage; 
                case 7: return MessageTypes.GuildMemberJoined;
                default: return MessageTypes.Default;
            }
        }
        public static MessageContainer MakeMessage(Message message) //TODO: IsContinuous
        {
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type), false, null);
            return msg;
        }

        public enum MessageTypes { Default, RecipientAdded, RecipientRemoved, Call, ChannelNameChanged, ChannelIconChanged, PinnedMessage, GuildMemberJoined, Advert}
        public class MessageContainer : INotifyPropertyChanged
        {
            public MessageContainer(Message? message, MessageTypes messageType, bool isContinuation, string header)
            {
                Message = message;
                MessageType = messageType;
                IsContinuation = isContinuation;
                Header = header;
            }

            private SharedModels.Message? _message;
            public SharedModels.Message? Message
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

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
