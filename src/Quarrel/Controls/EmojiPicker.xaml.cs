// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using Quarrel.ViewModels.Controls;
using Quarrel.ViewModels.Models;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    /// <summary>
    /// Control for selecting an Emoji.
    /// </summary>
    public sealed partial class EmojiPicker : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiPicker"/> class.
        /// </summary>
        public EmojiPicker()
        {
            this.InitializeComponent();

            // Sets DataContext
            Load();
        }

        /// <summary>
        /// Gets Data Context for Emoji Picker.
        /// </summary>
        public EmojiPickerViewModel ViewModel => this.DataContext as EmojiPickerViewModel;

        /// <summary>
        /// Gets or sets command for when Emoji is picked.
        /// </summary>
        public ICommand EmojiPickedCommand { get; set; }

        /// <summary>
        /// Sets DataContext to an EmojiPickerViewModel with the Emoji Lists.
        /// </summary>
        public async void Load()
        {
            this.DataContext = new EmojiPickerViewModel(await Constants.FromFile.GetEmojiLists());
        }

        /// <summary>
        /// Invoke EmojiPicked with Emoji selected.
        /// </summary>
        private void EmojiClicked(object sender, ItemClickEventArgs e)
        {
            // Use list so in future we can support adding multiple emoij at once
            EmojiPickedCommand.Execute(new List<Emoji> { e.ClickedItem as Emoji });
        }

        /// <summary>
        /// Filters Emoji List to search query.
        /// </summary>
        private void Search(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterEmojis(SearchBox.Text);
        }
    }
}
