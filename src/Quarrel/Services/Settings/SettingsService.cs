// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Messages.Services.Settings;
using Quarrel.Services.Settings.Enums;

namespace Quarrel.Services.Settings
{
    /// <summary>
    /// A simple <see langword="class"/> that handles the app settings, both locally and in the roaming directory
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        // Default settings
        public SettingsService() => EnsureDefaults();

        /// <inheritdoc/>
        public void EnsureDefaults()
        {
            Roaming.SetValue<string>(SettingKeys.Token, null, false);
        }

        /// <inheritdoc/>
        public ISettingsProvider this[SettingLocation location] => location == SettingLocation.Local ? Local : Roaming;

        /// <inheritdoc/>
        public ISettingsProvider Roaming { get; } = new SettingsProvider(ApplicationData.Current.RoamingSettings.Values);

        /// <inheritdoc/>
        public ISettingsProvider Local { get; } = new SettingsProvider(ApplicationData.Current.LocalSettings.Values);

        /// <summary>
        /// A <see langword="class"/> that handles the app settings on a specific settings storage location
        /// </summary>
        private sealed class SettingsProvider : ISettingsProvider
        {
            /// <summary>
            /// The <see cref="IPropertySet"/> with the settings targeted by the current instance
            /// </summary>
            [NotNull]
            private readonly IPropertySet SettingsStorage;

            /// <summary>
            /// Creates a new <see cref="SettingsProvider"/> instance that works on a specific settings <see cref="IPropertySet"/>
            /// </summary>
            /// <param name="settings">The target <see cref="IPropertySet"/> instance to use to store the settings</param>
            public SettingsProvider([NotNull] IPropertySet settings) => SettingsStorage = settings;

            /// <inheritdoc/>
            public void SetValue<T>(SettingKeys key, T value, bool overwrite = true, bool notify = false)
            {
                // Convert the value
                object serializable;
                if (typeof(T).IsEnum)
                {
                    Type type = Enum.GetUnderlyingType(typeof(T));
                    serializable = Convert.ChangeType(value, type);
                }
                else if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                {
                    serializable = value;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    serializable = Unsafe.As<T, DateTime>(ref value).ToBinary();
                }
                else throw new ArgumentException($"Invalid setting of type {typeof(T)}", nameof(value));

                // Store the new value
                if (!SettingsStorage.ContainsKey(key.ToString())) SettingsStorage.Add(key.ToString(), serializable);
                else if (overwrite) SettingsStorage[key.ToString()] = serializable;

                // Notify if needed
                if (notify) Messenger.Default.Send(new SettingChangedMessage<T>(key, value));
            }

            /// <inheritdoc/>
            public T GetValue<T>(SettingKeys key, bool fallback = false)
            {
                // Try to get the setting value
                if (!SettingsStorage.TryGetValue(key.ToString(), out object value))
                {
                    if (fallback) return default;
                    throw new InvalidOperationException($"The setting {key} doesn't exist");
                }

                // Cast and return the retrieved setting
                if (typeof(T) == typeof(DateTime)) value = DateTime.FromBinary(value.To<long>());
                return value.To<T>();
            }

            /// <inheritdoc/>
            public void Clear() => SettingsStorage.Clear();
        }
    }
}
