using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Discord_UWP.SharedModels;
using Discord_UWP.LocalModels;

namespace Discord_UWP.Managers
{
    public class MessageManager
    {
        public static async Task<List<MessageContainer>> ConvertMessage(List<Message> messages)
        {
            if (messages != null)
            {
                Message? prev = null;
                List<MessageContainer> returnMessages = new List<MessageContainer>();
                messages.Reverse();
                foreach (var message in messages)
                {
                    foreach (var user in message.Mentions)
                    {
                        if (!App.CurrentGuildIsDM && !LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(user.Id))
                        {
                            LocalState.Guilds[App.CurrentGuildId].members.Add(user.Id, await RESTCalls.GetGuildMember(App.CurrentGuildId, user.Id));
                        }
                    }

                    returnMessages.Add(new MessageContainer(message, GetMessageType(message.Type), ShouldContinuate(message, prev), null));
                    prev = message;
                }
                return returnMessages;
            }
            return null; //else
        }
        public static bool ShouldContinuate(Message current, Message? previous)
        {
            //If the previous message exists, is a normal message, and was published than 2 minutes ago, the current one should continuate it
            if (previous.HasValue && previous.Value.Type == 0 && current.Timestamp.Subtract(previous.Value.Timestamp).Minutes < 2 && previous.Value.User.Id == current.User.Id)
                return true;
            else
                return false;
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
        public static MessageContainer MakeMessage(Message message, bool isContinuation = false) //TODO: IsContinuous
        {
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type), isContinuation, null, false);
            return msg;
        }
        public static MessageContainer MakeMessage(string chnId, Discord_UWP.API.Channel.Models.MessageUpsert upsert)
        {
            Message message = new Message() { ChannelId = chnId, Content = upsert.Content, User = LocalState.CurrentUser, TTS = upsert.TTS, Timestamp = DateTime.Now };
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type), false, null, true);
            return msg;
        }

        public enum MessageTypes { Default, RecipientAdded, RecipientRemoved, Call, ChannelNameChanged, ChannelIconChanged, PinnedMessage, GuildMemberJoined, Advert }
        public class MessageContainer : INotifyPropertyChanged
        {
            public MessageContainer(Message? message, MessageTypes messageType, bool isContinuation, string header, bool pending = false)
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
                Blocked = messageType != MessageTypes.Advert && LocalState.Blocked.ContainsKey(message.Value.Id);
            }

            private Message? _message;
            public Message? Message
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
    }
}
