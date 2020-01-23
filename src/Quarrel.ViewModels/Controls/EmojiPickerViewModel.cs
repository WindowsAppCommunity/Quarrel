using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Services.Guild;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Controls
{
    /// <summary>
    /// Sorts bindable data for the Emoji Picker
    /// </summary>
    public class EmojiPickerViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Create emoji lists bindings based on <paramref name="emojiList"/>
        /// </summary>
        /// <param name="emojiList">List of Emojis</param>
        public EmojiPickerViewModel(EmojiLists emojiList)
        {
            _Emojis = emojiList;

            // Reloads emojis when a guild is selected
            Messenger.Default.Register<GuildNavigateMessage>(this, _ =>
            {
                LoadEmojis();
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads full list of Emojis
        /// </summary>
        public void LoadEmojis()
        {
            // Just filter none
            FilterEmojis("");
        }

        /// <summary>
        /// Filters down Emojis to only the ones that contain <paramref name="query"/>
        /// </summary>
        /// <param name="query">Emoji filtering query</param>
        public void FilterEmojis(string query)
        {
            // All emoji names are lower case
            query = query.ToLower();

            // Combines lists
            var emojis = _Emojis.People
                .Concat<Emoji>(_Emojis.Nature)
                .Concat(_Emojis.Food)
                .Concat(_Emojis.Activity)
                .Concat(_Emojis.Travel)
                .Concat(_Emojis.Objects)
                .Concat(_Emojis.Symbols)
                .Concat(_Emojis.Flags).ToList();

            // Guild Emojis
            // TODO: External emojis
            if (!GuildsService.CurrentGuild.IsDM)
                foreach (var emoji in GuildsService.CurrentGuild.Model.Emojis)
                {
                    emojis.Add(new GuildEmoji(emoji));
                }

            // Resets list
            Emojis.Clear();


            // Adds emoji to list if it matches query
            foreach (var emoji in emojis)
            {
                if (string.IsNullOrEmpty(query) || emoji.Names.Any(x => x.Contains(query)))
                    Emojis.AddElement(emoji);
            }

            // TODO: Sort by accuracy
        }

        #endregion

        #region Properties

        #region Services

        public IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        #endregion

        private EmojiLists _Emojis;

        /// <summary>
        /// Grouping of Emojis by 
        /// </summary>
        public GroupedObservableCollection<string, Emoji> Emojis { get; set; } = new GroupedObservableCollection<string, Emoji>(x => x.Category);

        #endregion
    }

    /// <summary>
    /// Categorized Emoji lists
    /// </summary>
    public class EmojiLists
    {
        /// <summary>
        /// List of People emojis
        /// </summary>
        [JsonProperty("people")]
        public List<PersonEmoji> People { get; set; }

        /// <summary>
        /// List of Nature emojis
        /// </summary>
        [JsonProperty("nature")]
        public List<NatureEmoji> Nature { get; set; }

        /// <summary>
        /// List of Food emojis
        /// </summary>
        [JsonProperty("food")]
        public List<FoodEmoji> Food { get; set; }

        /// <summary>
        /// List of Activity emojis
        /// </summary>
        [JsonProperty("activity")]
        public List<ActivityEmoji> Activity { get; set; }

        /// <summary>
        /// List of Travel emojis
        /// </summary>
        [JsonProperty("travel")]
        public List<TravelEmoji> Travel { get; set; }

        /// <summary>
        /// List of Object emojis
        /// </summary>
        [JsonProperty("objects")]
        public List<ObjectEmoji> Objects { get; set; }

        /// <summary>
        /// List of Symbol emojis
        /// </summary>
        public List<SymbolEmoji> Symbols { get; set; }

        /// <summary>
        /// List of Flag emojis
        /// </summary>
        [JsonProperty("flags")]
        public List<FlagEmoji> Flags { get; set; }
    }

    /// <summary>
    /// Bindable ViewModel for Emoji DataContext
    /// </summary>
    public class Emoji : ViewModelBase, IEquatable<Emoji>, IComparable<Emoji>
    {
        /// <summary>
        /// Unique ID for Id (overriden by Guild ID)
        /// </summary>
        [JsonIgnore]
        public virtual string Id
        {
            get => Names?[0];
        }

        /// <summary>
        /// split string by skin-tone suffix
        /// </summary>
        [JsonProperty("names")]
        public List<string> Names
        {
            get => _Names;
            set => Set(ref _Names, value);
        }
        private List<string> _Names;

        /// <summary>
        /// True if the Emoji supports multiple skintones
        /// </summary>
        [JsonProperty("hasDiversity")]
        public bool? HasDiversity
        {
            get => _HasDiversity;
            set => Set(ref _HasDiversity, value);
        }
        private bool? _HasDiversity;

        /// <summary>
        /// True if the emoji is from Discord instead of Unicode
        /// </summary>
        [JsonIgnore]
        public virtual bool CustomEmoji => false;

        /// <summary>
        /// The category the emoji is from
        /// </summary>
        [JsonProperty("category")]
        public virtual string Category { get; }

        /// <summary>
        /// How the emoji is displayed in drafts
        /// </summary>
        public string Surrogates
        {
            get => _Surrogates;
            set => Set(ref _Surrogates, value);
        }
        private string _Surrogates;

        #region Interfaces

        #region IEquatable

        /// <summary>
        /// Checks if this is the same as other
        /// </summary>
        /// <param name="other">Emoji to compare with</param>
        /// <returns>True if this emoji is the same as other</returns>
        public bool Equals(Emoji other)
        {
            return Id == other.Id;
        }

        #endregion

        #region IComparable

        /// <remarks>
        /// Arbitrary comparsion for consistent unimportant order
        /// </remarks>
        public int CompareTo(Emoji other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion

        #endregion
    }

    #region Emoji Types

    // TODO: In desperate need of refactor

    public class GuildEmoji : Emoji
    {
        #region Services

        public IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        #endregion

        public GuildEmoji(DiscordAPI.Models.Emoji emoji)
        {
            _Id = emoji.Id;
            _Catergory = GuildsService.CurrentGuild.Model.Name;
            Names = new List<string>() { emoji.Name };
            Surrogates = emoji.DisplayUrl;
        }

        private string _Id;
        public override string Id { get => _Id; }

        private string _Catergory;
        public override string Category { get => _Catergory; }

        public override bool CustomEmoji => true;

        /// <summary>
        /// False if the user does have permissions to use emoji or can't (nitro)
        /// </summary>
        public bool IsEnabled { get; private set; }
    }

    public class PersonEmoji : Emoji
    { public override string Category => "Person"; }

    public class NatureEmoji : Emoji
    { public override string Category => "Nature"; }

    public class FoodEmoji : Emoji
    { public override string Category => "Food"; }

    public class ActivityEmoji : Emoji
    { public override string Category => "Activity"; }

    public class TravelEmoji : Emoji
    { public override string Category => "Travel"; }

    public class ObjectEmoji : Emoji
    { public override string Category => "Object"; }

    public class SymbolEmoji : Emoji
    { public override string Category => "Symbol"; }

    public class FlagEmoji : Emoji
    { public override string Category => "Flag"; }

    #endregion
}
