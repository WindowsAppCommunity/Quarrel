// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Interfaces;

namespace Quarrel.Bindables.Abstract
{
    public abstract class SelectableItem : ObservableObject, ISelectableItem
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
