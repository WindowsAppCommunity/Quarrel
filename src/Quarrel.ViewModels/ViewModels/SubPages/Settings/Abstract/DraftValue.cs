// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

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
        /// An event raised when the value is updated.
        /// </summary>
        public event EventHandler<DraftValueUpdated<T>>? ValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DraftValue{T}"/> class.
        /// </summary>
        public DraftValue(T value)
        {
            _value = value;
            CanonicalValue = value;
        }

        /// <summary>
        /// The drafted value.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                bool oldIsDraft = IsDrafted;
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(nameof(IsDrafted));
                    ValueChanged?.Invoke(this, new DraftValueUpdated<T>(Value, IsDrafted, oldIsDraft != IsDrafted));
                }
            }
        }

        /// <summary>
        /// The original value.
        /// </summary>
        public T CanonicalValue { get; }

        /// <summary>
        /// Gets whether or not the value drafted.
        /// </summary>
        public bool IsDrafted => !CanonicalValue?.Equals(Value) ?? false;

        /// <summary>
        /// Resets the value to the canonical value.
        /// </summary>
        public void Reset()
        {
            Value = CanonicalValue;
        }
    }
}
