using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;

namespace Quarrel.Services.Guild
{
    public class GuildsService : IGuildsService
    {
        public Dictionary<string, BindableChannel> CurrentChannels { get; private set; } = new Dictionary<string, BindableChannel>();

        private ICacheService CacheService;

        public void RegisterChannel(BindableChannel channel, string channelId)
        {
            CurrentChannels.Add(channelId, channel);
        }

        public BindableChannel RemoveChannel(string channelId)
        {
            BindableChannel removed;
            CurrentChannels.Remove(channelId, out removed);
            return removed;
        }

        public BindableChannel GetChannel(string channelId)
        {
            return CurrentChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }
    }
}
