using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_UWP.SharedModels;

namespace Discord_UWP.API.Invite
{
    public interface IInviteService
    {
        [Get("/invites/{inviteCode}")]
        Task<SharedModels.Invite> GetInvite([AliasAs("inviteCode")] string inviteCode);

        [Delete("/invites/{inviteCode}")]
        Task<SharedModels.Invite> DeleteInvite([AliasAs("inviteCode")] string inviteCode);

        [Post("/invites/{inviteCode}")]
        Task<SharedModels.Invite> AcceptInvite([AliasAs("inviteCode")] string inviteCode);
    }
}
