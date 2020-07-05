// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Emojis;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quarrel.ViewModels.Services.DispatcherHelper;

namespace Quarrel.ViewModels.Controls
{
    /// <summary>
    /// Sorts bindable data for the Emoji Picker.
    /// </summary>
    public class EmojiPickerViewModel : ViewModelBase
    {
        private readonly IEnumerable<Emoji> _emojis;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiPickerViewModel"/> class based on <paramref name="emojiList"/>.
        /// </summary>
        /// <param name="emojiList">List of Emojis.</param>
        public EmojiPickerViewModel(IEnumerable<Emoji> emojiList)
        {
            _emojis = emojiList;

            // Reloads emojis when a guild is selected
            Messenger.Default.Register<GuildNavigateMessage>(this, _ =>
            {
                LoadEmojis();
            });
        }

        /// <summary>
        /// Gets grouping of Emojis by the current filter.
        /// </summary>
        public GroupedObservableCollection<string, Emoji> Emojis { get; private set; } = new GroupedObservableCollection<string, Emoji>(x => x.Category);

        private string _searchQuery;

        /// <summary>
        /// Gets or sets the search query text.
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    Set(ref _searchQuery, value);
                    FilterEmojis(_searchQuery);
                }
            }
        }

        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        private IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        /// <summary>
        /// Loads full list of Emojis.
        /// </summary>
        public void LoadEmojis()
        {
            // Just filter none
            FilterEmojis(string.Empty);
        }

        /// <summary>
        /// Filters down Emojis to only the ones that contain <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Emoji filtering query.</param>
        public void FilterEmojis(string query)
        {
            DispatcherHelper.CheckBeginInvokeOnUi(() =>
            {
                // Resets list
                Emojis.Clear();

                // All emoji names are lower case
                query = query.ToLower();

                // Guild Emojis
                // TODO: External emojis
                if (!GuildsService.CurrentGuild.IsDM)
                {
                    var emojis = GuildsService.CurrentGuild.Model.Emojis
                        .Select(x => new GuildEmoji(x));
                    foreach (var emoji in emojis)
                    {
                        if (string.IsNullOrEmpty(query) || emoji.Names.Any(x => x.ToLower().Contains(query)))
                        {
                            Emojis.AddElement(emoji);
                        }
                    }
                }

                // Adds emoji to list if it matches query
                foreach (var emoji in _emojis)
                {
                    if (string.IsNullOrEmpty(query) || emoji.Names.Any(x => x.ToLower().Contains(query)))
                    {
                        Emojis.AddElement(emoji);
                    }
                }
            });

            // TODO: Sort by accuracy
        }
    }
}
