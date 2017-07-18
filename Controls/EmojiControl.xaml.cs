using System;
using System.Collections;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class EmojiControl : UserControl
    {
        public event EventHandler<RoutedEventArgs> PickedEmoji; 
        public class ISimpleEmoji
        {
            public List<string> names { get; set; }
            public string surrogates { get; set; }
            public bool? hasDiversity { get; set; }
            public virtual string category { get; set; }
            public int position { get; set; }
        }

        public class Person : ISimpleEmoji
        { public override string category => "Person"; }

        public class Nature : ISimpleEmoji
        { public override string category => "Nature"; }

        public class Food : ISimpleEmoji
        { public override string category => "Food"; }

        public class Activity : ISimpleEmoji
        { public override string category => "Activities"; }

        public class Travel : ISimpleEmoji
        { public override string category => "Travel"; }

        public class Object : ISimpleEmoji
        { public override string category => "Objects"; }

        public class Symbol : ISimpleEmoji
        { public override string category => "Symbols"; }

        public class Flag : ISimpleEmoji
        { public override string category => "Flags"; }

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
            emojis = root.people.Concat<ISimpleEmoji>(root.nature)
                                  .Concat(root.food)
                                  .Concat(root.activity)
                                  .Concat(root.travel)
                                  .Concat(root.objects)
                                  .Concat(root.symbols)
                                  .Concat(root.flags);
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
            PickedEmoji?.Invoke((e.ClickedItem as ISimpleEmoji).surrogates, null);
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

                if (sender as ToggleButton != SkinTone6)
                    SkinTone6.IsChecked = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
