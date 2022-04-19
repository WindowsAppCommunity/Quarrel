// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The view model for the guild list in the app.
    /// </summary>
    public partial class GuildsViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ConcurrentDictionary<ulong, BindableGuild> _guilds;

        private BindableGuild? _selectedGuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildsViewModel"/> class.
        /// </summary>
        public GuildsViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<IBindableGuildListItem>();
            _guilds = new ConcurrentDictionary<ulong, BindableGuild>();

            _messenger.Register<UserLoggedInMessage>(this, (_, _) => LoadGuilds());
            _messenger.Register<NavigateToGuildMessage<ulong>>(this, (_, m) => ForwardNavigate(m.Guild));
        }

        /// <summary>
        /// Gets or sets the selected guild.
        /// </summary>
        public BindableGuild? SelectedGuild
        {
            get => _selectedGuild;
            set
            {
                if (_selectedGuild is not null)
                    _selectedGuild.IsSelected = false;

                if (SetProperty(ref _selectedGuild, value) && value is not null)
                {
                    value.IsSelected = true;
                    _messenger.Send(new NavigateToGuildMessage<BindableGuild>(value));
                }
            }
        }

        /// <summary>
        /// Gets the guild folders.
        /// </summary>
        public ObservableCollection<IBindableGuildListItem> Source { get; private set; }

        /// <summary>
        /// Loads the guilds for the user.
        /// </summary>
        public void LoadGuilds()
        {
            var folders = _discordService.GetMyGuildFolders();
            _dispatcherService.RunOnUIThread(() =>
            {
                foreach (var folder in folders)
                {
                    if (folder.Folder.Id is null)
                    {
                        foreach (var child in folder.Children)
                        {
                            _guilds.TryAdd(child.Guild.Id, child);
                            Source.Add(child);
                        }
                    }
                    else
                    {
                        Source.Add(folder);
                    }
                }
                _messenger.Send(new GuildsLoadedMessage());
            });
        }

        private void ForwardNavigate(ulong guildId)
        {
            BindableGuild? guild = GetBindableGuild(guildId);
            if (guild is not null)
            {
                SelectedGuild = guild;
            }
        }

        private BindableGuild? GetBindableGuild(ulong guildId)
        {
            if (_guilds.TryGetValue(guildId, out var guild))
            {
                return guild;
            }

            return null;
        }
    }
}
