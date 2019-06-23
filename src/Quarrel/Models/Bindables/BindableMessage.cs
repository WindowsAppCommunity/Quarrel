// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Quarrel.Models.Bindables.Abstract;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Services;

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId) : base(model)
        {
            GuildId = guildId;
        }

        private string GuildId;

        public BindableGuildMember Author
        {
            get => ServicesManager.Cache.Runtime.TryGetValue<BindableGuildMember>(Quarrel.Helpers.Constants.Cache.Keys.GuildMember, GuildId + Model.User.Id);
        }

        #region Display

        public string AuthorName
        {
            get
            {
                return Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;
            }
        }

        public int AuthorColor
        {
            get
            {
                return Author != null && Author.TopRole != null ? Author.TopRole.Color : -1;
            }
        }

        #endregion
    }
}
