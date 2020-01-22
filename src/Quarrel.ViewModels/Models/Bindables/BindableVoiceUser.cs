using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Voice;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Users;
using System;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableVoiceUser : BindableModelBase<VoiceState>, IEquatable<BindableVoiceUser>, IComparable<BindableVoiceUser>
    {
        #region Constructors

        public BindableVoiceUser([NotNull] VoiceState model) : base(model)
        {
            #region Messenger

            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, e =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (e.VoiceState.UserId == Model.UserId)
                    {
                        if (e.VoiceState.SelfDeaf != Model.SelfDeaf)
                        {
                            Model.SelfDeaf = e.VoiceState.SelfDeaf;
                            UpateProperties();
                        }

                        if (e.VoiceState.SelfMute != Model.SelfMute)
                        {
                            Model.SelfMute = e.VoiceState.SelfMute;
                            UpateProperties();
                        }

                        if (e.VoiceState.ServerDeaf != Model.ServerDeaf)
                        {
                            Model.ServerDeaf = e.VoiceState.ServerDeaf;
                            UpateProperties();
                        }

                        if (e.VoiceState.ServerMute != Model.ServerMute)
                        {
                            Model.ServerMute = e.VoiceState.ServerMute;
                            UpateProperties();
                        }
                    }
                });
            });
            MessengerInstance.Register<SpeakMessage>(this, e =>
            {
                if (e.EventData.UserId == Model.UserId)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() => { Speaking = e.EventData.Speaking > 0; });
                }
            });

            #endregion
        }

        #endregion

        #region Properties

        #region Services

        public ICurrentUsersService UserService => SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        public IDispatcherHelper DispatcherHelper => SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        #endregion

        #region Display

        public bool ShowDeaf => Model.SelfDeaf || Model.ServerDeaf;
        public bool ServerDeaf => ShowDeaf && Model.ServerDeaf;
        public bool LocalDeaf => ShowDeaf && !Model.ServerDeaf;

        public bool ShowMute => Model.SelfMute || Model.ServerMute;
        public bool ServerMute => ShowMute && Model.ServerMute;
        public bool LocalMute => ShowMute && !Model.ServerMute;

        private bool speaking;
        public bool Speaking
        {
            get => speaking;
            set => Set(ref speaking, value);
        }

        #endregion

        public BindableGuildMember GuildMember => UserService != null && UserService.Users.TryGetValue(Model.UserId, out BindableGuildMember member) ? member : null;

        #endregion

        #region Methods

        private void UpateProperties()
        {
            RaisePropertyChanged(nameof(ShowDeaf));
            RaisePropertyChanged(nameof(ServerDeaf));
            RaisePropertyChanged(nameof(LocalDeaf));
            RaisePropertyChanged(nameof(ShowMute));
            RaisePropertyChanged(nameof(ServerMute));
            RaisePropertyChanged(nameof(LocalMute));
        }

        #endregion

        #region Interfaces

        public bool Equals(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
