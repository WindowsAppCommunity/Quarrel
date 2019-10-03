using Quarrel.Services.Settings;

namespace Quarrel.Messages.Services.Settings
{
    /// <summary>
    /// A message that notifies whenever a given setting has changed
    /// </summary>
    /// <typeparam name="T">The setting type</typeparam>
    public sealed class SettingChangedMessage<T>
    {
        /// <summary>
        /// Gets the key of the setting that has changed
        /// </summary>
        public SettingKeys Key { get; }

        /// <summary>
        /// Gets the updated setting value
        /// </summary>
        public T Value { get; }

        public SettingChangedMessage(SettingKeys key, T value)
        {
            Key = key;
            Value = value;
        }
    }
}
