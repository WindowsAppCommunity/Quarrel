// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// A base class for settings pages.
    /// </summary>
    public abstract class SettingsPageViewModel : ObservableObject
    {
        private SettingsSubPageViewModel? _selectedSubPage;

        internal SettingsPageViewModel(ISettingsMenuItem[] pages)
        {
            Pages = new ObservableCollection<ISettingsMenuItem>(pages);
            SelectedSubPage = (SettingsSubPageViewModel)Pages.FirstOrDefault(x => x is SettingsSubPageViewModel { IsActive: true });
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
    }
}
