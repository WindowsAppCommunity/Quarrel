namespace Quarrel.ViewModels.Services.Discord.Presence
{
    public interface IPresenceService
    {
        DiscordAPI.Models.Presence GetUserPrecense(string userId);
        void UpdateUserPrecense(string userId, DiscordAPI.Models.Presence presence);
    }
}
