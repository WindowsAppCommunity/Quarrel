// Copyright (c) Quarrel. All rights reserved.

using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Invite
{
    /// <summary>
    /// A service for invite REST calls.
    /// </summary>
    public interface IInviteService
    {
        /// <summary>
        /// Get an invite by code.
        /// </summary>
        /// <param name="inviteCode">The invite's code.</param>
        /// <returns>The invite.</returns>
        [Get("/v6/invites/{inviteCode}?with_counts=true")]
        Task<Models.Invite> GetInvite([AliasAs("inviteCode")] string inviteCode);

        /// <summary>
        /// Delete an invite by code.
        /// </summary>
        /// <param name="inviteCode">The invite's code.</param>
        /// <returns>The deleted invite.</returns>
        [Delete("/v6/invites/{inviteCode}")]
        Task<Models.Invite> DeleteInvite([AliasAs("inviteCode")] string inviteCode);

        /// <summary>
        /// Accept an invite by code.
        /// </summary>
        /// <param name="inviteCode">The invite's code.</param>
        /// <returns>The invite.</returns>
        [Post("/v6/invites/{inviteCode}")]
        Task<Models.Invite> AcceptInvite([AliasAs("inviteCode")] string inviteCode);
    }
}
