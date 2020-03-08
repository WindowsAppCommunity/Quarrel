// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;

namespace Quarrel.ViewModels.Messages.Abstract
{
    /// <summary>
    /// A base message that signals whenever a specific value has changed.
    /// </summary>
    /// <typeparam name="T">That type of the changed value.</typeparam>
    public abstract class ValueChangedMessageBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedMessageBase{T}"/> class.
        /// </summary>
        /// <param name="value">The changed value.</param>
        protected ValueChangedMessageBase(T value) => Value = value;

        /// <summary>
        /// Gets the value that has changed.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Unwraps the inner message value directly.
        /// </summary>
        /// <param name="message">The value type wrapped by the current instance.</param>
        public static implicit operator T([NotNull] ValueChangedMessageBase<T> message) => message.Value;
    }
}
