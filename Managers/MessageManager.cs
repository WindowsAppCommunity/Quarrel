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
                int adCheck = 5;
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

                    returnMessages.Add(new MessageContainer(message, GetMessageType(message.Type, message.Content), ShouldContinuate(message, prev), null));
                    adCheck--;
                    if (adCheck == 0 && App.ShowAds && !Storage.Settings.VideoAd)
                    {
                        returnMessages.Add(new MessageContainer(null, MessageTypes.Advert, false, null));
                        adCheck = 5;
                    }
                    prev = message;
                }
                return returnMessages;
            }
            return null; //else
        }
        private static bool ShouldContinuate(Message current, Message? previous)
        {
            //If the previous message exists, is a normal message, and was published than 2 minutes ago, the current one should continuate it
            if (previous.HasValue && previous.Value.Type == 7 && current.Timestamp.Subtract(previous.Value.Timestamp).Minutes < 2)
                return true;
            else
                return false;
        }
        public static MessageTypes GetMessageType(int type, string content)
        {
            if (content.StartsWith("[[[DUWP_E2E]"))
            {
                try
                {
                    if (content.StartsWith("[[[DUWP_E2E]-Encrypted]{"))
                    {
                        return MessageTypes.EncryptedMessage;
                    }
                    else if (content.StartsWith("[[[DUWP_E2E]-HandshakeRequest]{"))
                    {
                        return MessageTypes.StartEncryption;
                    }
                    else if (content.StartsWith("[[[DUWP_E2E]-HandshakeResponse"))
                    {
                        content = content.Remove(0, 32);
                        content = content.Remove(content.Length - 2);
                        if (!Storage.EncryptionKeys.ContainsKey(App.CurrentChannelId))
                        EncryptionManager.HandleHandshakeResponse(content);
                        return MessageTypes.AcceptEncryption;
                    }
                    else if (content == "[[[DUWP_E2E]-StopEncryption]]")
                    {
                        return MessageTypes.StopEncryption;
                    }
                }
                catch
                {
                    return MessageTypes.FailedEncryption;
                } 
            }
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
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type,message.Content), false, null, false);
            return msg;
        }
        public static MessageContainer MakeMessage(string chnId, Discord_UWP.API.Channel.Models.MessageUpsert upsert)
        {
            Message message = new Message() { ChannelId = chnId, Content = upsert.Content, User = LocalState.CurrentUser, TTS = upsert.TTS, Timestamp = DateTime.Now };
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type, message.Content), false, null, true);
            return msg;
        }

        public enum MessageTypes { Default, RecipientAdded, RecipientRemoved, Call, ChannelNameChanged, ChannelIconChanged, PinnedMessage, GuildMemberJoined, Advert, StartEncryption, AcceptEncryption, StopEncryption, FailedEncryption, EncryptedMessage }
        public class MessageContainer : INotifyPropertyChanged
        {
            public MessageContainer(Message? message, MessageTypes messageType, bool isContinuation, string header, bool pending = false)
            {
                Message = message;
                MessageType = messageType;
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
