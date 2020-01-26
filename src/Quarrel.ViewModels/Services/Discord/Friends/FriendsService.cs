using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System.Collections.Concurrent;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    public class FriendsService : IFriendsService
    {
        private IDispatcherHelper DispatcherHelper;

        public FriendsService(IDispatcherHelper dispatcherHelper)
        {
            DispatcherHelper = dispatcherHelper;

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() => {
                    foreach (var presence in m.EventData.Presences)
                    {
                        DMUsers.TryAdd(presence.User.Id, new BindableGuildMember(new GuildMember() { User = presence.User }) { Presence = presence, GuildId = "DM" });
                    }
                });
            });
        }

        public ConcurrentDictionary<string, BindableFriend> Friends { get; } =
            new ConcurrentDictionary<string, BindableFriend>();
        public ConcurrentDictionary<string, BindableGuildMember> DMUsers { get; } =
            new ConcurrentDictionary<string, BindableGuildMember>();
    }
}
