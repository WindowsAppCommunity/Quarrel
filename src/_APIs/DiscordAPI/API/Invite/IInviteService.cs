using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Invite
{
    public interface IInviteService
    {
        [Get("/v6/invites/{inviteCode}?with_counts=true")]
        Task<DiscordAPI.Models.Invite> GetInvite([AliasAs("inviteCode")] string inviteCode);

        [Delete("/invites/{inviteCode}")]
        Task<DiscordAPI.Models.Invite> DeleteInvite([AliasAs("inviteCode")] string inviteCode);

        [Post("/invites/{inviteCode}")]
        Task<DiscordAPI.Models.Invite> AcceptInvite([AliasAs("inviteCode")] string inviteCode);
    }
}
