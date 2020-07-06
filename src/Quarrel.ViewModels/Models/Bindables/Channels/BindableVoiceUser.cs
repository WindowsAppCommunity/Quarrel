// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;

namespace Quarrel.ViewModels.Models.Bindables.Channels
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="VoiceState"/> model.
    /// </summary>
    public class BindableVoiceUser : BindableModelBase<VoiceState>, IEquatable<BindableVoiceUser>, IComparable<BindableVoiceUser>
    {
        private bool _speaking;
        private ICurrentUserService _currentUsersService = null;
        private IGuildsService _guildsService = null;
        private IDispatcherHelper _dispatcherHelper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableVoiceUser"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="VoiceState"/> object.</param>
        public BindableVoiceUser([NotNull] VoiceState model) : base(model)
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is deafend.
        /// </summary>
        public bool ShowDeaf => Model.SelfDeaf || Model.ServerDeaf;

        /// <summary>
        /// Gets a value indicating whether or not the user is deafend by the server.
        /// </summary>
        public bool ServerDeaf => ShowDeaf && Model.ServerDeaf;

        /// <summary>
        /// Gets a value indicating whether or not the user is deafend locally.
        /// </summary>
        public bool LocalDeaf => ShowDeaf && !Model.ServerDeaf;

        /// <summary>
        /// Gets a value indicating whether or not the user is muted.
        /// </summary>
        public bool ShowMute => Model.SelfMute || Model.ServerMute;

        /// <summary>
        /// Gets a value indicating whether or not the user is muted by the server.
        /// </summary>
        public bool ServerMute => ShowMute && Model.ServerMute;

        /// <summary>
        /// Gets a value indicating whether or not user is muted locally.
        /// </summary>
        public bool LocalMute => ShowMute && !Model.ServerMute;

        /// <summary>
        /// Gets or sets a value indicating whether or not the user is speaking.
        /// </summary>
        public bool Speaking
        {
            get => _speaking;
            set => Set(ref _speaking, value);
        }

        /// <summary>
        /// Gets the guild member for the voice user.
        /// </summary>
        public BindableGuildMember GuildMember => Model.Member != null ? new BindableGuildMember(Model.Member, Model.GuildId) : GuildsService.GetGuildMember(Model.UserId, Model.GuildId);

        private ICurrentUserService CurrentUsersService => _currentUsersService ?? (_currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>());

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        private IDispatcherHelper DispatcherHelper => _dispatcherHelper ?? (_dispatcherHelper = SimpleIoc.Default.GetInstance<IDispatcherHelper>());

        /// <summary>
        /// Raises property changed on all properties.
        /// </summary>
        public void UpateProperties()
        {
            RaisePropertyChanged(nameof(ShowDeaf));
            RaisePropertyChanged(nameof(ServerDeaf));
            RaisePropertyChanged(nameof(LocalDeaf));
            RaisePropertyChanged(nameof(ShowMute));
            RaisePropertyChanged(nameof(ServerMute));
            RaisePropertyChanged(nameof(LocalMute));
        }

        /// <inheritdoc/>
        public bool Equals(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int CompareTo(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }
    }
}
