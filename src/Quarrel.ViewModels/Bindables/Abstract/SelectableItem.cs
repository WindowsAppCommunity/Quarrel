// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Interfaces;

namespace Quarrel.Bindables.Abstract
{
    /// <summary>
    /// A base class for items that can be selected in a bindable context.
    /// </summary>
    public abstract class SelectableItem : ObservableObject, ISelectableItem
    {
        private bool _isSelected;

        /// <summary>
        /// Gets or sets whether or not the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
