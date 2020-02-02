using Quarrel.Helpers;
using Quarrel.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace Quarrel.Controls
{
    public sealed partial class EmojiPicker : UserControl
    {
        /// <summary>
        /// Execute a command when Emoji is picked
        /// </summary>
        public ICommand EmojiPickedCommand { get; set; }

        public EmojiPicker()
        {
            this.InitializeComponent();

            // Sets DataContext
            Load();
            Debug.WriteLine("emojiflyout created");
        }

        /// <summary>
        /// Data Context for Emoji Picker
        /// </summary>
        public EmojiPickerViewModel ViewModel => this.DataContext as EmojiPickerViewModel;

        /// <summary>
        /// Sets DataContext to an EmojiPickerViewModel with the Emoji Lists
        /// </summary>
        public async void Load()
        {
            this.DataContext = new EmojiPickerViewModel(await Constants.FromFile.GetEmojiLists());
        }

        /// <summary>
        /// Invoke EmojiPicked with Emoji selected
        /// </summary>
        private void EmojiClicked(object sender, ItemClickEventArgs e)
        {
            // Use list so in future we can support adding multiple emoij at once
            EmojiPickedCommand.Execute(new List<Emoji>{e.ClickedItem as Emoji});
        }

        /// <summary>
        /// Filters Emoji List to search query
        /// </summary>
        private void Search(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterEmojis(SearchBox.Text);
        }
    }
}
