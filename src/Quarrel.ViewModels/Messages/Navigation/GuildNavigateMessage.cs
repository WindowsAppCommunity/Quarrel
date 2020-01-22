using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Navigation
{
    public sealed class GuildNavigateMessage
    {
        public GuildNavigateMessage(BindableGuild guild)
        {
            Guild = guild;
        }

        public BindableGuild Guild { get; }
    }
}
