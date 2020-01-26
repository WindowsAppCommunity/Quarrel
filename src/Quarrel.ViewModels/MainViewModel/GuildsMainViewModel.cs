using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel
    {
        #region Commands

        #region Navigation

        /// <summary>
        /// Sends Messenger Request to change Guild
        /// </summary>
        private RelayCommand<BindableGuild> navigateGuildCommand;
        public RelayCommand<BindableGuild> NavigateGuildCommand => navigateGuildCommand ??=
            new RelayCommand<BindableGuild>((guild) => { MessengerInstance.Send(new GuildNavigateMessage(guild)); });


        #endregion

        #endregion

        #region Methods

        private void RegisterGuildsMessages()
        {
            #region Navigation

            MessengerInstance.Register<GuildNavigateMessage>(this, m =>
            {
                if (CurrentGuild != m.Guild)
                {
                    BindableChannel channel =
                        m.Guild.Channels.FirstOrDefault(x => x.IsTextChannel && x.Permissions.ReadMessages);
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        CurrentChannel = channel;
                        CurrentGuild = m.Guild;
                        BindableMessages.Clear();
                        //BindableChannels = m.Guild.Channels;
                    });

                    if (m.Guild.IsDM)
                    {
                        CurrentBindableMembers.Clear();
                    }

                    if (channel != null)
                        MessengerInstance.Send(new ChannelNavigateMessage(channel, m.Guild));
                }
            });

            #endregion

            // Handles string message used for App Events
            MessengerInstance.Register<string>(this, m =>
            {
                if (m == "GuildsReady")
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        // Show guilds
                        BindableGuilds.AddRange(GuildsService.AllGuilds.Values.OrderBy(x => x.Position));
                    });
            });

        }

        #endregion

        #region Properties

        /// <summary>
        /// Bindable object representing the currently opened guild
        /// </summary>
        public BindableGuild CurrentGuild
        {
            get => _CurrentGuild;
            set => Set(ref _CurrentGuild, value);
        }
        private BindableGuild _CurrentGuild;

        /// <summary>
        /// TODO: Remove
        /// </summary>
        private string guildId => CurrentChannel?.GuildId ?? "DM";


        [NotNull]
        public ObservableRangeCollection<BindableGuild> BindableGuilds { get; private set; } =
            new ObservableRangeCollection<BindableGuild>();

        #endregion
    }
}
