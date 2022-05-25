// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;
using System;

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

        /// <summary>
        /// An event raised when <see cref="IsEdited"/> is changed.
        /// </summary>
        public event EventHandler<bool>? IsEditedChanged;

        internal SettingsSubPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
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
        public bool IsEdited => _draftCount != 0;

        /// <summary>
        /// Resets all unsaved changes in settings.
        /// </summary>
        public abstract void ResetValues();

        /// <summary>
        /// Increments or decrements the draft count when a value changes.
        /// </summary>
        protected void ValueChanged<T>(object sender, DraftValueUpdated<T> e)
        {
            bool oldEdited = IsEdited;
            if (e.IsDraftChanged)
            {
                _draftCount += e.IsDraft ? 1 : -1;
            }

            if (IsEdited != oldEdited)
            {
                IsEditedChanged?.Invoke(this, IsEdited);
            }
        }
    }
}
