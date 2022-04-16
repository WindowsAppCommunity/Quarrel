// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Gateways.Models.Handshake;
using Quarrel.Client.Models.Users;

namespace Quarrel.Client
{
    /// <inheritdoc/>
    public partial class QuarrelClient
    {
        private void OnReady(Ready ready)
        {
            Guard.IsNotNull(ready, nameof(ready));

            AddSelfUser(ready.User);

            foreach (var guild in ready.Guilds)
            {
                // All child members are handled here
                AddGuild(guild);
            }

            foreach (var readState in ready.ReadStates)
            {
                AddReadState(readState);
            }

            foreach (var presence in ready.Presences)
            {
                AddPresence(presence);
            }

            foreach (var relationship in ready.Relationships)
            {
                AddRelationship(relationship);
            }

            UpdateSettings(ready.Settings);

            Guard.IsNotNull<SelfUser>(_selfUser, nameof(Client.QuarrelClient._selfUser));

            LoggedIn?.Invoke(this, _selfUser);
        }
    }
}
