// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Models.Interfaces;
using Windows.UI.Notifications;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Rest;
using GalaSoft.MvvmLight.Ioc;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableModelBase<Guild>, IGuild
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();

        public BindableGuild([NotNull] Guild model) : base(model)
        {
            _Channels = new List<BindableChannel>();
        }

        private List<BindableChannel> _Channels;

        public List<BindableChannel> Channels
        {
            get  => _Channels;
            set => Set(ref _Channels, value);
        }

        // Order
        // Add allows for @everyone role
        // Add allows for each role
        public Permissions Permissions
        {
            get
            {
                // TODO: Calculate once and store
                Permissions perms = new Permissions(Model.Roles.FirstOrDefault().Permissions);

                BindableGuildMember member = new BindableGuildMember(Model.Members.FirstOrDefault(x => x.User.Id == discordService.CurrentUser.Id));
                member.GuildId = Model.Id;
                foreach (var role in member.Roles)
                {
                    perms.AddAllows((GuildPermission)role.Permissions);
                }
                return perms;
            }
        }

        public bool IsDM
        {
            get => Model.Id == "DM";
        }

        #region Settings

        private int _Position;

        public int Position
        {
            get => _Position;
            set => Set(ref _Position, value);
        }


        private bool _Muted;

        public bool Muted
        {
            get => _Muted;
            set => Set(ref _Muted, value);
        }

        #endregion

        #region Icon

        public string DisplayText
        {
            get
            {
                if (IsDM) { return ""; }
                else
                {
                    return String.Concat(Model.Name.Split(' ').Select(s => StringInfo.GetNextTextElement(s, 0)).ToArray());
                }
            }
        }

        public string IconUrl
        {
            get { return "https://cdn.discordapp.com/icons/" + Model.Id + "/" + Model.Icon + ".png"; }
        }

        public Uri IconUri { get { return new Uri(IconUrl); } }
            
        public bool HasIcon { get { return !String.IsNullOrEmpty(Model.Icon); } }

        #endregion

        #region ReadState

        public bool IsUnread
        {
            get => Channels.Any(x => x.IsUnread);
        }

        public bool ShowUnread
        {
            get => Channels.Any(x => x.ShowUnread) && !Muted && NotificationCount == 0;
        }

        public int NotificationCount
        {
            get
            {
                int total = 0;
                foreach(var channel in Channels)
                {
                    total += channel.ReadState != null ? channel.ReadState.MentionCount : 0;
                }
                return total;
            }
        }

        #endregion
    }
}
