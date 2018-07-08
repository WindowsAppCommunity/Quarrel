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
        Task<ActivityData> GetGatewayConfig();
    }
}
