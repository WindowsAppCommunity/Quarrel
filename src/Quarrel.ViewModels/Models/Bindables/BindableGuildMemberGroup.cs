using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableGuildMemberGroup : BindableModelBase<Group>, IGuildMemberListItem
    {
        private IGuildsService GuildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        public BindableGuildMemberGroup([NotNull] Group model) : base(model)
        {
        }

        public int Count => Model.Count;

        public Role Role => GuildsService.CurrentGuild.Model.Roles.FirstOrDefault(x => x.Id == Model.Id);

        public string Name
        {
            get
            {
                switch (Model.Id)
                {
                    case "online":
                        return "Online";
                    case "offline":
                        return "Offline";
                    default:
                        return Role.Name;
                }
            }
        }
    }
}
