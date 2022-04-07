// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Quarrel.Bindables.Abstract
{
    public abstract partial class SelectableItem : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;
    }
}
