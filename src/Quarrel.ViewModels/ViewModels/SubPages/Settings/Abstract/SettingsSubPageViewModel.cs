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
        private IDraftValue[] _draftValues;
        private int _draftCount;

        internal SettingsSubPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _draftValues = new IDraftValue[0];

            ApplyChangesCommand = new RelayCommand(ApplyChangesRoot);
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
        protected abstract void ApplyChanges();

        private void ApplyChangesRoot()
        {
            // Run polymorphic ApplyChanges
            ApplyChanges();

            // Apply draft values
            foreach (var value in _draftValues)
            {
                value.Apply();
            }
        }

        /// <summary>
        /// Reverts all unsaved changes in settings.
        /// </summary>
        private void RevertChanges()
        {
            foreach (var value in _draftValues)
            {
                value.Reset();
            }
        }

        /// <summary>
        /// Registers <see cref="IDraftValue"/> to track for is edited, and to reset.
        /// </summary>
        /// <param name="draftValues">The <see cref="IDraftValue"/>s to track.</param>
        protected void RegisterDraftValues(params IDraftValue[] draftValues)
        {
            _draftValues = draftValues;

            foreach (var value in _draftValues)
            {
                value.ValueChanged += ValueChanged;
            }
        }

        /// <summary>
        /// Increments or decrements the draft count when a value changes.
        /// </summary>
        private void ValueChanged(object sender, DraftValueUpdated e)
        {
            if (e.IsDraftChanged)
            {
                DraftCount += e.IsDraft ? 1 : -1;
            }
        }
    }
}
