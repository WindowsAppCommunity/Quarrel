// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Models.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Rest;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Messages.Gateway;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Quarrel.Services.Settings;
using Quarrel.Messages.Services.Settings;
using Quarrel.Navigation;
using Quarrel.Services.Users;
using Quarrel.ViewModels.Services.DispatcherHelper;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableModelBase<Guild>, IGuild
    {
        private IDiscordService _DiscordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISettingsService _SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
        private ICurrentUsersService CurrentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private ISubFrameNavigationService SubFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
        private IDispatcherHelper DispatcherHelper = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        public BindableGuild([NotNull] Guild model) : base(model)
        {
            _Channels = new ObservableCollection<BindableChannel>();

            MessengerInstance.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                    RaisePropertyChanged(nameof(ShowUnread));
                });
            });

            MessengerInstance.Register<GatewayMessageAckMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                    RaisePropertyChanged(nameof(ShowUnread));
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
        private Permissions permissions = null;
        public Permissions Permissions
        {
            get
            {
                if (permissions != null)
                    return permissions;

                if (Model.Id == "DM" || Model.OwnerId == CurrentUsersService.CurrentUser.Model.Id)
                    return new Permissions(int.MaxValue);

                // Role Id == Model.Id for @everyone
                Permissions perms = new Permissions(Model.Roles.FirstOrDefault(x => x.Id == Model.Id).Permissions);

                BindableGuildMember member = new BindableGuildMember(Model.Members.FirstOrDefault(x => x.User.Id == CurrentUsersService.CurrentUser.Model.Id));
                if (member == null) return perms;

                member.GuildId = Model.Id;
                foreach (var role in member.Roles)
                {
                    perms.AddAllows((GuildPermission)role.Permissions);
                }
                permissions = perms;
                return perms;
            }
        }

        public bool IsDM => Model.Id == "DM";

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

        public string IconUrl => $"https://cdn.discordapp.com/icons/{Model.Id}/{Model.Icon}.png?size=128";

        public bool HasIcon => !String.IsNullOrEmpty(Model.Icon);

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
        
        public bool IsOwner { get => CurrentUsersService.CurrentUser.Model.Id == Model.OwnerId; }

        private RelayCommand addChanneleCommand;
        public RelayCommand AddChannelCommand =>
            addChanneleCommand ?? (addChanneleCommand = new RelayCommand(() =>
            {
                SubFrameNavigationService.NavigateTo("AddChannelPage", Model);

            }));

        private RelayCommand markAsRead;
        public RelayCommand MarkAsRead => markAsRead = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.AckGuild(Model.Id);
        });
    }
}
