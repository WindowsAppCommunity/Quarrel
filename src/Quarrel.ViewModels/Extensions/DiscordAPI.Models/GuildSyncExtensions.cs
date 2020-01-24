using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;

namespace DiscordAPI.Models
{
    internal static class GuildSyncExtentions
    {
        public static void Cache(this GuildSync sync)
        {
            #region Presense

            foreach(var presence in sync.Presences)
            {
                Messenger.Default.Send(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
            }

            #endregion
        }
    }
}
