using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel
    {
        private RelayCommand<BindableGuild> navigateGuild;
        private BindableGuild _currentGuild;
        private BindableGuildMember _currentGuildMember;

        /// <summary>
        /// Gets a command that sends Messenger Request to change Guild.
        /// </summary>
        public RelayCommand<BindableGuild> NavigateGuild => navigateGuild = navigateGuild ?? new RelayCommand<BindableGuild>((guild) => { MessengerInstance.Send(new GuildNavigateMessage(guild)); });

        /// <summary>
        /// Gets or sets the currently selected guild.
        /// </summary>
        public BindableGuild CurrentGuild
        {
            get => _currentGuild;
            set => Set(ref _currentGuild, value);
        }

        /// <summary>
        /// Gets the current user's BindableGuildMember in the current guild.
        /// </summary>
        public BindableGuildMember CurrentGuildMember
        {
            get => _currentGuildMember;
            private set => Set(ref _currentGuildMember, value);
        }

        /// <summary>
        /// Gets all Guilds the current member is in.
        /// </summary>
        [NotNull]
        public ObservableRangeCollection<BindableGuild> BindableGuilds { get; private set; } =
            new ObservableRangeCollection<BindableGuild>();

        private void RegisterGuildsMessages()
        {
            MessengerInstance.Register<GuildNavigateMessage>(this, m =>
            {
                if (CurrentGuild != m.Guild)
                {
                    BindableChannel channel =
                        m.Guild.Channels.FirstOrDefault(x => x.IsTextChannel && x.Permissions.ReadMessages);
                    _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        CurrentChannel = channel;
                        CurrentGuild = m.Guild;
                        BindableMessages.Clear();

                        if (m.Guild.IsDM)
                        {
                            CurrentBindableMembers.Clear();
                        }

                        if (!m.Guild.IsDM)
                        {
                            CurrentGuildMember = _guildsService.GetGuildMember(_currentUserService.CurrentUser.Model.Id, m.Guild.Model.Id);
                        }
                        else
                        {
                            CurrentGuildMember = new BindableGuildMember(
                            new DiscordAPI.Models.GuildMember()
                            {
                                User = _currentUserService.CurrentUser.Model,
                            },
                            "DM",
                            _currentUserService.CurrentUser.Presence);
                        }
                    });

                    if (channel != null)
                    {
                        MessengerInstance.Send(new ChannelNavigateMessage(channel, m.Guild));
                    }
                }
            });

            // Handles string message used for App Events
            MessengerInstance.Register<string>(this, m =>
            {
                if (m == "GuildsReady")
                {
                    _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        // Show guilds
                        BindableGuilds.Clear();
                        BindableGuilds.AddRange(_guildsService.AllGuilds.Values.OrderBy(x => x.Position));
                    });
                }
            });

        }
    }
}
