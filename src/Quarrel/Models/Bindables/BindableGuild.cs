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
using Quarrel.Messages.Gateway;
using UICompositionAnimations.Helpers;
using System.Collections.ObjectModel;
using Quarrel.Services.Settings;
using Quarrel.Messages.Services.Settings;
using Quarrel.Services.Users;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableModelBase<Guild>, IGuild
    {
        private IDiscordService _DiscordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISettingsService _SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
        private ICurrentUsersService CurrentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        public BindableGuild([NotNull] Guild model) : base(model)
        {
            _Channels = new ObservableCollection<BindableChannel>();

            MessengerInstance.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                });
            });

            MessengerInstance.Register<GatewayMessageAckMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                });
            });

            MessengerInstance.Register<SettingChangedMessage<bool>>(this, m => 
            {
                if (m.Key == SettingKeys.ServerMuteIcons)
                {
                    RaisePropertyChanged(nameof(ShowMute));
                }
            });
        }

        private ObservableCollection<BindableChannel> _Channels;

        public ObservableCollection<BindableChannel> Channels
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
                Permissions perms = new Permissions(Model.Roles.FirstOrDefault(x => x.Name == "@everyone").Permissions);

                BindableGuildMember member = CurrentUsersService.CurrentGuildMember;
                if (member == null) return perms;
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
            set
            {
                if (Set(ref _Muted, value))
                    RaisePropertyChanged(nameof(ShowMute));
            }
        }

        public bool ShowMute => Muted && _SettingsService.Roaming.GetValue<bool>(SettingKeys.ServerMuteIcons);

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
