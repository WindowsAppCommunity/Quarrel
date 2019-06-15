using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;

namespace DiscordAPI.API.Invite
{
    public interface IInviteService
    {
        [Get("/invites/{inviteCode}?with_counts=true")]
        Task<DiscordAPI.Models.Invite> GetInvite([AliasAs("inviteCode")] string inviteCode);

        [Delete("/invites/{inviteCode}")]
        Task<DiscordAPI.Models.Invite> DeleteInvite([AliasAs("inviteCode")] string inviteCode);

        [Post("/invites/{inviteCode}")]
        Task<DiscordAPI.Models.Invite> AcceptInvite([AliasAs("inviteCode")] string inviteCode);
    }
}
