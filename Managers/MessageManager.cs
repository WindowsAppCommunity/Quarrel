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
    class MessageManager
    {
        public static List<MessageContainer> ConvertMessage(List<Message> messages)
        {
            if (messages != null)
            {
                int adCheck = 5;
                List<MessageContainer> returnMessages = new List<MessageContainer>();
                foreach (var message in messages)
                {
                    returnMessages.Add(new MessageContainer(message, false, false, null)); //TODO: isConinuation
                    adCheck--;
                    if (adCheck == 0)
                    {
                        returnMessages.Add(new MessageContainer(null, true, false, null));
                        adCheck = 5;
                    }
                }
                returnMessages.Reverse();
                return returnMessages;
            }
            return null; //else
        }

        public static MessageContainer MakeMessage(Message message) //TODO: IsContinuous
        {
            MessageContainer msg = new MessageContainer(message, false, false, null);
            return msg;
        }

        public class MessageContainer : INotifyPropertyChanged
        {
            public MessageContainer(Message? message, bool isAd, bool isContinuation, string header)
            {
                Message = message;
                IsAdvert = isAd;
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

            private bool _isadvert;
            public bool IsAdvert
            {
                get => _isadvert;
                set { if (_isadvert == value) return; _isadvert = value; OnPropertyChanged("IsAdvert"); }
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
