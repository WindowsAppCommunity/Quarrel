// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    public class SettingsPageViewModel : ObservableObject
    {
        private int _editedCount;
        private SettingsSubPageViewModel? _selectedSubPage;

        internal SettingsPageViewModel(ISettingsMenuItem[] pages)
        {
            foreach (var page in pages)
            {
                if (page is SettingsSubPageViewModel ssPage)
                {
                    ssPage.IsEditedChanged += IsEditedChanged;
                }
            }

            Pages = new ObservableCollection<ISettingsMenuItem>(pages);
        }

        /// <summary>
        /// Gets the view models of all subpage options.
        /// </summary>
        public ObservableCollection<ISettingsMenuItem> Pages { get; private set; }

        /// <summary>
        /// Gets the view model of the selected sub page.
        /// </summary>
        public SettingsSubPageViewModel? SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        public bool IsEdited => _editedCount != 0;

        private void IsEditedChanged(object sender, bool value)
        {
            bool oldEdited = IsEdited;
            _editedCount += value ? 1 : -1;

            if (IsEdited != oldEdited)
            {
                OnPropertyChanged(nameof(IsEdited));
            }
        }
    }
}
