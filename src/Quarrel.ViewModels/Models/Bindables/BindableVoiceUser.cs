// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Voice;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="VoiceState"/> model.
    /// </summary>
    public class BindableVoiceUser : BindableModelBase<VoiceState>, IEquatable<BindableVoiceUser>, IComparable<BindableVoiceUser>
    {
        private bool _speaking;

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
        public BindableGuildMember GuildMember => GuildsService.GetGuildMember(Model.UserId, Model.GuildId);

        private ICurrentUserService UserService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        private IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

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
