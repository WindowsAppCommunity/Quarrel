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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class EmojiControl : UserControl
    {
        
        public event EventHandler<ISimpleEmoji> PickedEmoji; 
        
        public class ChangedDiversityArgs : EventArgs
        {
            public int skintone { get; set; }
        }
        public event EventHandler<ChangedDiversityArgs> ChangedDiversity;
        public class ISimpleEmoji : INotifyPropertyChanged
        {
            public List<string> names { get; set; }
            public bool? hasDiversity { get; set; }
            public virtual string category { get; set; }
            public int position { get; set; }

            private string _surrogates;
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

            public void Emoji_ChangedDiversity(object sender, ChangedDiversityArgs e)
            {
                surrogates = surrogates.ChangeSkinTone(e.skintone);
            }
        }

        public class Person : ISimpleEmoji
        { public override string category => "PEOPLE"; }

        public class Nature : ISimpleEmoji
        { public override string category => "NATURE"; }

        public class Food : ISimpleEmoji
        { public override string category => "FOOD"; }

        public class Activity : ISimpleEmoji
        { public override string category => "ACTIVITIES"; }

        public class Travel : ISimpleEmoji
        { public override string category => "TRAVEL"; }

        public class Object : ISimpleEmoji
        { public override string category => "OBJECTS"; }

        public class Symbol : ISimpleEmoji
        { public override string category => "SYMBOLS"; }

        public class Flag : ISimpleEmoji
        { public override string category => "FLAGS"; }

        public class GuildSide : ISimpleEmoji
        {
            public override string category { get; set; }
            public bool IsEnabled { get; set; }
            public string id { get; set; }
        }
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
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/emojis.json"));
            string json = await FileIO.ReadTextAsync(file);
            RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
            var guildEmojis = new List<GuildSide>();
            try
            {
                foreach (var emoji in Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Emojis)
                {
                    //Does the user have the authorization to use the emoji?
                    if (emoji.Roles.Count() != 0 && !App.GuildMembers[App.CurrentUserId]
                            .Raw.Roles.Intersect(emoji.Roles)
                            .Any()) return;
                    guildEmojis.Add(new GuildSide()
                    {
                        category = Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Name.ToUpper(),
                        hasDiversity = false,
                        names = new List<string>() { emoji.Name },
                        surrogates = "https://cdn.discordapp.com/emojis/" + emoji.Id + ".png",
                        id = emoji.Id
                    });
                }
            }
            catch (Exception) { }
            emojis = guildEmojis.Concat<ISimpleEmoji> (root.people)
                                .Concat(root.nature)
                                .Concat(root.food)
                                .Concat(root.activity)
                                .Concat(root.travel)
                                .Concat(root.objects)
                                .Concat(root.symbols)
                                .Concat(root.flags);
            foreach(var emoji in emojis)
            {
                if(emoji.hasDiversity == true)
                     ChangedDiversity += emoji.Emoji_ChangedDiversity;
            }
            var grouped = emojis.GroupBy(x => x.category);
            EmojiCVS.Source = grouped;
        }



        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (dragTransform.X <= 0 &&  dragTransform.X >= -48)
                dragTransform.X += e.Delta.Translation.X;
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if(dragTransform.X < -24)
                Snap.Begin();
            else
                SnapBack.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(dragTransform.X != -48)
                Snap.Begin();
            else
                SnapBack.Begin();
        }

        private void EmojiView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            PickedEmoji?.Invoke(null, (e.ClickedItem as ISimpleEmoji));
        }
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
            return ":" + e.names[0] + ":" + suffix;
        }
        private bool loaded = false;
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
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
                ChangedDiversity?.Invoke(null, new ChangedDiversityArgs() { skintone = Convert.ToInt16((sender as ToggleButton).Tag as string) });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private void Emoji_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Searchbox.PlaceholderText = ":" + ToolTipService.GetToolTip(sender as UIElement) + ":";
        }

        private async void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable grouped = null;
            string query = Searchbox.Text;
            await Task.Run(() =>
            {
                var filtered = new List<ISimpleEmoji>();
                foreach (var emoji in emojis)
                {
                    if (emoji.names[0].Contains(query))
                    {
                        filtered.Add(emoji);
                    }
                }
                grouped = filtered.OrderBy(x => x.names[0].StartsWith(query)).GroupBy(x => x.category);
            });
           
            EmojiCVS.Source = grouped;
        }
    }

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
