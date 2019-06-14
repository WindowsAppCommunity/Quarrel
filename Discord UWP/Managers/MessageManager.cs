using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DiscordAPI.API.Channel.Models;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;

namespace Quarrel.Managers
{
    public class MessageManager
    {
        public static async Task<List<MessageContainer>> ConvertMessage(List<Message> messages)
        {
            if (messages != null)
            {
                Message prev = null;
                List<MessageContainer> returnMessages = new List<MessageContainer>();
                messages.Reverse();
                foreach (var message in messages)
                {
                    /*foreach (var user in message.Mentions)
                    {
                        if (!App.CurrentGuildIsDM)
                        {
                            LocalState.Guilds[App.CurrentGuildId].members.TryAdd(user.Id, await RESTCalls.GetGuildMember(App.CurrentGuildId, user.Id));
                        }
                    }*/

                    returnMessages.Add(new MessageContainer(message, GetMessageType(message.Type), ShouldContinuate(message, prev), null));
                    prev = message;
                }
                return returnMessages;
            }
            return null; //else
        }
        public static bool ShouldContinuate(Message current, Message previous)
        {
            //If the previous message exists, is a normal message, and was published than 2 minutes ago, the current one should continuate it
            if (previous != null && previous.Type == 0 && current.Timestamp.Subtract(previous.Timestamp).Minutes < 2 && previous.User.Id == current.User.Id)
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
        public static MessageContainer MakeMessage(Message message, bool isContinuation = false, bool lastread = false)
        {
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type), isContinuation, null, false);
            if (lastread) msg.LastRead = true;
            return msg;
        }
        public static MessageContainer MakeMessage(string chnId, MessageUpsert upsert)
        {
            Message message = new Message() { ChannelId = chnId, Content = upsert.Content, User = LocalState.CurrentUser, TTS = upsert.TTS, Timestamp = DateTime.Now };
            MessageContainer msg = new MessageContainer(message, GetMessageType(message.Type), false, null, true);
            return msg;
        }

        
     }
}
