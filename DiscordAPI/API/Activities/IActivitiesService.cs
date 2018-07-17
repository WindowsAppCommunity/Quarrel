using Discord_UWP.SharedModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.Activities
{
    public interface IActivitesService
    {
        [Get("/v6/activities")]
        Task<IEnumerable<ActivityData>> GetActivites();

        [Get("/v6/users/@me/feed/settings?include_autosubscribed_games=true")]
        Task<IEnumerable<ActivityData>> GetFeedSettings();
    }
}
