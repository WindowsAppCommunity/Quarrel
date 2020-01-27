using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.DerivedColor
{
    public interface IColorService
    {
        Task<int> GetUserColor(User user);

        int GetStatusColor(string status);
    }
}
