using Quarrel.Helpers;
using Quarrel.ViewModels.Controls;
using System;
using System.Collections.Generic;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class EmojiPicker : UserControl
    {
        /// <summary>
        /// Trigger event when Emoji is picked
        /// </summary>
        public event EventHandler<Emoji> EmojiPicked;

        public EmojiPicker()
        {
            this.InitializeComponent();

            // Sets DataContext
            Load();
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
            EmojiPicked?.Invoke(this, e.ClickedItem as Emoji);
        }
    }
}
