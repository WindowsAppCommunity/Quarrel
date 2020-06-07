// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Discord.Guilds;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Notifications settings page data.
    /// </summary>
    public class NotificationsSettingsPageViewModel : ViewModelBase
    {
        private BindableGuild _guild;
        private int _notifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to modify.</param>
        public NotificationsSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets or sets the guild being modified.
        /// </summary>
        public BindableGuild Guild
        {
            get => _guild;
            set => Set(ref _guild, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not notifications should be shown for all messages.
        /// </summary>
        public bool AllMessages
        {
            get => _notifications == 2;
            set
            {
                if (Set(ref _notifications, 2))
                {
                    RaisePropertyChanged(nameof(Mentions));
                    RaisePropertyChanged(nameof(None));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not notifications should be shown for mentions.
        /// </summary>
        public bool Mentions
        {
            get => _notifications == 2;
            set
            {
                if (Set(ref _notifications, 2))
                {
                    RaisePropertyChanged(nameof(AllMessages));
                    RaisePropertyChanged(nameof(None));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not notifications should not be shown.
        /// </summary>
        public bool None
        {
            get => _notifications == 2;
            set
            {
                if (Set(ref _notifications, 2))
                {
                    RaisePropertyChanged(nameof(AllMessages));
                    RaisePropertyChanged(nameof(Mentions));
                }
            }
        }

        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();
    }
}
