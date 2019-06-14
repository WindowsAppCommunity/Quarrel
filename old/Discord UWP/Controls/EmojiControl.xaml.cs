using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;
using Quarrel.LocalModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class EmojiControl : UserControl
    {
        /// <summary>
        /// Event for when an emoji is selected
        /// </summary>
        public event EventHandler<ISimpleEmoji> PickedEmoji; 
        
        public class ChangedDiversityArgs : EventArgs
        {
            public int skintone { get; set; }
        }
        /// <summary>
        /// Event for when skin-tone is changed
        /// </summary>
        public event EventHandler<ChangedDiversityArgs> ChangedDiversity;

        public class ISimpleEmoji : INotifyPropertyChanged
        {
            /// <summary>
            /// split string by skin-tone suffix
            /// </summary>
            public List<string> names { get; set; }
            /// <summary>
            /// True if the Emoji supports multiple skintones
            /// </summary>
            public bool? hasDiversity { get; set; }
            /// <summary>
            /// True if the emoji is from Discord instead of Unicode
            /// </summary>
            public bool CustomEmoji { get; set; }
            /// <summary>
            /// The category the emoji is from
            /// </summary>
            public virtual string category { get; set; }

            private string _surrogates;
            /// <summary>
            /// How the emoji is displayed in drafts
            /// </summary>
            public string surrogates
            {
                get => _surrogates;
                set { if (_surrogates == value) return; _surrogates = value; OnPropertyChanged("surrogates"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            /// <summary>
            /// Change the skin-tone on this emoji 
            /// </summary>
            public void Emoji_ChangedDiversity(object sender, ChangedDiversityArgs e)
            {
                // Change the suffix name to a skin-tone
                if (names.Count > 1)
                {
                    names[1] = "skin-tone-" + e.skintone.ToString();
                } else
                {
                    names.Add("skin-tone-" + e.skintone.ToString());
                }
                surrogates = surrogates.ChangeSkinTone(e.skintone);
            }
        }

        #region Emoji Default Categories
        public class Person : ISimpleEmoji
        { public override string category => App.GetString("/Controls/PEOPLE"); }

        public class Nature : ISimpleEmoji
        { public override string category => App.GetString("/Controls/NATURE"); }

        public class Food : ISimpleEmoji
        { public override string category => App.GetString("/Controls/FOOD"); }

        public class Activity : ISimpleEmoji
        { public override string category => App.GetString("/Controls/ACTIVITIES"); }

        public class Travel : ISimpleEmoji
        { public override string category => App.GetString("/Controls/TRAVEL"); }

        public class Object : ISimpleEmoji
        { public override string category => App.GetString("/Controls/OBJECTS"); }

        public class Symbol : ISimpleEmoji
        { public override string category => App.GetString("/Controls/SYMBOLS"); }

        public class Flag : ISimpleEmoji
        { public override string category => App.GetString("/Controls/FLAGS"); }

        #endregion

        public class GuildSide : ISimpleEmoji
        {
            public override string category { get; set; }

            /// <summary>
            /// True if the user can use this emoji in the current guild
            /// </summary>
            public bool IsEnabled { get; set; }

            /// <summary>
            /// Discord ID of the emoji
            /// </summary>
            public string id { get; set; }
        }

        /// <summary>
        /// Root container of all emojis
        /// </summary>
        public class RootObject
        {
            public List<Person> people { get; set; }
            public List<Nature> nature { get; set; }
            public List<Food> food { get; set; }
            public List<Activity> activity { get; set; }
            public List<Travel> travel { get; set; }
            public List<Object> objects { get; set; }
            public List<Symbol> symbols { get; set; }
            public List<Flag> flags { get; set; }
        }

        public EmojiControl()
        {
            this.InitializeComponent();
            LoadEmojis();
        }

        private IEnumerable<ISimpleEmoji> emojis;
        private async void LoadEmojis()
        {
            // Read unicode emoji list from json file
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/emojis.json"));
            string json = await FileIO.ReadTextAsync(file);
            RootObject root = JsonConvert.DeserializeObject<RootObject>(json);

            var guildEmojis = new List<GuildSide>();
            if (!App.CurrentGuildIsDM)
            {
                try
                {
                    foreach (var emoji in LocalState.Guilds[App.CurrentGuildId].Raw.Emojis)
                    {
                        //Does the user have the authorization to use the emoji?
                        if (emoji.Roles.Count() != 0 && !LocalState.Guilds[App.CurrentGuildId].members[LocalState.CurrentUser.Id]
                                .Roles.Intersect(emoji.Roles)
                                .Any()) return;
                        
                        // Determine file type of emoji
                        string extension = ".png";
                        if (emoji.Animated) extension = ".gif";

                        // Add emoji to list
                        guildEmojis.Add(new GuildSide()
                        {
                            category = LocalState.Guilds[App.CurrentGuildId].Raw.Name.ToUpper(),
                            hasDiversity = false,
                            names = new List<string>() { emoji.Name },
                            surrogates = "https://cdn.discordapp.com/emojis/" + emoji.Id + extension,
                            id = emoji.Id,
                            CustomEmoji = true
                        });
                    }
                }
                catch (Exception) { }
            }
                
            // Merge emoji lists
            emojis = guildEmojis.Concat<ISimpleEmoji> (root.people)
                                .Concat(root.nature)
                                .Concat(root.food)
                                .Concat(root.activity)
                                .Concat(root.travel)
                                .Concat(root.objects)
                                .Concat(root.symbols)
                                .Concat(root.flags);
            
            // Register changed diversity event for all applicable emojis
            foreach(var emoji in emojis)
            {
                if(emoji.hasDiversity == true)
                     ChangedDiversity += emoji.Emoji_ChangedDiversity;
            }

            // Group emojis by Category
            var grouped = emojis.GroupBy(x => x.category);

            // Display grouped collection
            EmojiCVS.Source = grouped;
        }

        /// <summary>
        /// Invoke emoji selected event
        /// </summary>
        private void EmojiView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            PickedEmoji?.Invoke(null, (e.ClickedItem as ISimpleEmoji));
        }

        /// <summary>
        /// Get Emoji name with skin-tone
        /// </summary>
        /// <returns>Emoji name</returns>
        private string GetEmojiName(ISimpleEmoji e)
        {
            string suffix = "";
            if (e.hasDiversity == true)
            {
                if (SkinTone1.IsChecked == true)
                    suffix = ":skin-tone-1:";
                else if (SkinTone2.IsChecked == true)
                    suffix = ":skin-tone-2:";
                else if (SkinTone3.IsChecked == true)
                    suffix = ":skin-tone-3:";
                else if (SkinTone4.IsChecked == true)
                    suffix = ":skin-tone-4:";
                else if (SkinTone5.IsChecked == true)
                    suffix = ":skin-tone-5:";
            }
            return "<:" + e.names[0] + ":" + suffix ;
        }

        /// <summary>
        /// Indicates if the Control has finished loading
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Called when a skin-tone is selected
        /// </summary>
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                // Clear previous skin-tone
                if (sender as ToggleButton != SkinTone1)
                    SkinTone1.IsChecked = false;

                if (sender as ToggleButton != SkinTone2)
                    SkinTone2.IsChecked = false;

                if (sender as ToggleButton != SkinTone3)
                    SkinTone3.IsChecked = false;

                if (sender as ToggleButton != SkinTone4)
                    SkinTone4.IsChecked = false;

                if (sender as ToggleButton != SkinTone5)
                    SkinTone5.IsChecked = false;

                if (sender as ToggleButton != SkinTone0)
                    SkinTone0.IsChecked = false;

                // Invoke event
                ChangedDiversity?.Invoke(null, new ChangedDiversityArgs() { skintone = Convert.ToInt16((sender as ToggleButton).Tag as string) });
            }
        }

        /// <summary>
        /// Control finished loading
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        /// <summary>
        /// Show emoji-name tooltip
        /// </summary>
        private void Emoji_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Searchbox.PlaceholderText = ":" + ToolTipService.GetToolTip(sender as UIElement) + ":";
        }

        /// <summary>
        /// Query the emoji list
        /// </summary>
        private async void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filter groupings
            IEnumerable grouped = null;
            
            // All emoji names are lower case
            string query = Searchbox.Text.ToLower();


            await Task.Run(() =>
            {
                // Create a new emoji list with only emojis from query
                var filtered = new List<ISimpleEmoji>();
                foreach (var emoji in emojis)
                {
                    if (emoji.names[0].Contains(query))
                    {
                        filtered.Add(emoji);
                    }
                }

                // Sort by accuracy then group new listing
                grouped = filtered.OrderBy(x => x.names[0].StartsWith(query)).GroupBy(x => x.category);
            });
           
            // Show new listing
            EmojiCVS.Source = grouped;
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            //Nothing to dispose
        }
    }

    /// <summary>
    /// Extensions for emojis
    /// </summary>
    static class EmojiSkinToneManager
    {

        public static string ChangeSkinTone(this string emoji, int skinTone)
        {
            // Remove old skintone
            emoji = emoji.Replace("🏻", "").Replace("🏼", "").Replace("🏽", "").Replace("🏾", "").Replace("🏿", "");

            // Add new skin-tone
            if (skinTone == 0) return emoji;
            else
                emoji += GetSkinTone(skinTone);

            // Return result
            return emoji;
        }

        /// <summary>
        /// Get a skintone modifier character from an integer
        /// </summary>
        private static string GetSkinTone(int skintone)
        {
            switch (skintone)
            {
                case 1: return "🏻";
                case 2: return "🏼";
                case 3: return "🏽";
                case 4: return "🏾";
                case 5: return "🏿";
            }
            return "";
        }
    }

    /// <summary>
    /// DataTemplate for Emojis
    /// </summary>
    public class EmojiDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PlainEmojiTemplate { get; set; }
        public DataTemplate GuildEmojiTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            if (item is EmojiControl.GuildSide)
                return GuildEmojiTemplate;
            else
                return PlainEmojiTemplate;
        }
    }
}
