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
        private Dictionary<string, BindableChannel> currentChannels = new Dictionary<string, BindableChannel>();
        public ReadOnlyDictionary<string, BindableChannel> CurrentChannels { get; }
        private ICacheService CacheService;

        public void RegisterChannel(BindableChannel channel, string channelId)
        {
            currentChannels.Add(channelId, channel);
        }

        public BindableChannel GetChannel(string channelId)
        {
            return currentChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;

        }
    }
}
