// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Services.Settings;

namespace Quarrel.ViewModels.Messages.Services.Settings
{
    /// <summary>
    /// A message that notifies whenever a given setting has changed.
    /// </summary>
    /// <typeparam name="T">The setting type.</typeparam>
    public sealed class SettingChangedMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingChangedMessage{T}"/> class.
        /// </summary>
        /// <param name="key">The key requested.</param>
        /// <param name="value">The value for the key.</param>
        public SettingChangedMessage(SettingKeys key, T value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the key of the setting that has changed.
        /// </summary>
        public SettingKeys Key { get; }

        /// <summary>
        /// Gets the updated setting value.
        /// </summary>
        public T Value { get; }
    }
}
