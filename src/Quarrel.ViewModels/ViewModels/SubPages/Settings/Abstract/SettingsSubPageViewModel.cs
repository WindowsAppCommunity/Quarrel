// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// A base class for settings sub-page view models.
    /// </summary>
    public abstract class SettingsSubPageViewModel : ObservableObject, ISettingsMenuItem
    {
        /// <summary>
        /// The localization service.
        /// </summary>
        protected readonly ILocalizationService _localizationService;
        private int _draftCount;

        internal SettingsSubPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            ApplyChangesCommand = new RelayCommand(ApplyChanges);
            RevertChangesCommand = new RelayCommand(RevertChanges);
        }

        /// <summary>
        /// Gets the string used as a glyph for the sub page.
        /// </summary>
        public abstract string Glyph { get; }

        /// <summary>
        /// Gets the title of the sub page.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets whether or not the page is currently active.
        /// </summary>
        public virtual bool IsActive => false;

        /// <summary>
        /// Gets whether or not the page contains edited value.
        /// </summary>
        public bool IsEdited => DraftCount != 0;

        /// <summary>
        /// Gets a command that applies all changes from the sub page.
        /// </summary>
        public RelayCommand ApplyChangesCommand { get; }

        /// <summary>
        /// Gets a command that reverts all changes in the sub page.
        /// </summary>
        public RelayCommand RevertChangesCommand { get; }

        private int DraftCount
        {
            get => _draftCount;
            set
            {
                bool oldEdited = IsEdited;
                _draftCount = value;
                if (IsEdited != oldEdited)
                {
                    OnPropertyChanged(nameof(IsEdited));
                }
            }
        }

        /// <summary>
        /// Applies all changes made in settings.
        /// </summary>
        public virtual void ApplyChanges()
        {
            DraftCount = 0;
        }

        /// <summary>
        /// Reverts all unsaved changes in settings.
        /// </summary>
        public virtual void RevertChanges()
        {
            DraftCount = 0;
        }

        /// <summary>
        /// Increments or decrements the draft count when a value changes.
        /// </summary>
        protected void ValueChanged<T>(object sender, DraftValueUpdated<T> e)
        {
            if (e.IsDraftChanged)
            {
                DraftCount += e.IsDraft ? 1 : -1;
            }
        }
    }
}
