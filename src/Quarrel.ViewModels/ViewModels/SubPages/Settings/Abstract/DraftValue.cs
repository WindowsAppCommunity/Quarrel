// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// A value that can have an updated draft.
    /// </summary>
    /// <typeparam name="T">The type of value being drafted</typeparam>
    public class DraftValue<T> : ObservableObject
    {
        private T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DraftValue{T}"/> class.
        /// </summary>
        public DraftValue(T value)
        {
            _value = value;
            CanonValue = value;
        }

        /// <summary>
        /// The drafted value.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(nameof(Drafted));
                }
            }
        }

        /// <summary>
        /// The original value.
        /// </summary>
        public T CanonValue { get; }

        /// <summary>
        /// Gets whether or not the value drafted.
        /// </summary>
        public bool Drafted => !CanonValue?.Equals(Value) ?? false;
    }
}
