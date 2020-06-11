// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Quarrel.Services.Settings
{
    /// <summary>
    /// A simple <see langword="class"/> that handles the app settings, both locally and in the roaming directory.
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class with default settings.
        /// </summary>
        public SettingsService() => EnsureDefaults();

        /// <inheritdoc/>
        public ISettingsProvider Roaming { get; } = new SettingsProvider(ApplicationData.Current.RoamingSettings.Values);

        /// <inheritdoc/>
        public ISettingsProvider Local { get; } = new SettingsProvider(ApplicationData.Current.LocalSettings.Values);

        /// <inheritdoc/>
        public ISettingsProvider this[SettingLocation location] => location == SettingLocation.Local ? Local : Roaming;

        /// <inheritdoc/>
        public void EnsureDefaults()
        {
            Roaming.SetValue<string>(SettingKeys.Token, null, false);

            Roaming.SetValue(SettingKeys.Theme, Theme.Windows, false);
            Roaming.SetValue(SettingKeys.Blurple, false, false);
            Roaming.SetValue(SettingKeys.ServerMuteIcons, true, false);
            Roaming.SetValue(SettingKeys.DerivedColor, false, false);
            Roaming.SetValue(SettingKeys.ExpensiveRendering, true, false);
            Roaming.SetValue(SettingKeys.FluentTheme, false, false);

            Roaming.SetValue(SettingKeys.MentionGlow, false, false);
            Roaming.SetValue(SettingKeys.AuthorPresence, false, false);
            Roaming.SetValue(SettingKeys.ShowNoPermssions, false, false);
            Roaming.SetValue(SettingKeys.HideMuted, false, false);
            Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.Unread, false);
            Roaming.SetValue(SettingKeys.FilterMembers, true, false);
            Roaming.SetValue(SettingKeys.TTLAttachments, false, false);
            Roaming.SetValue(SettingKeys.DataCompression, true, false);

            Roaming.SetValue(SettingKeys.OutputDevice, "Default", false);
            Roaming.SetValue(SettingKeys.InputDevice, "Default", false);

            Roaming.SetValue(
                SettingKeys.AcrylicSettings,
                AcrylicSettings.GuildView | AcrylicSettings.CommandBar,
                false);
        }

        /// <summary>
        /// A <see langword="class"/> that handles the app settings on a specific settings storage location.
        /// </summary>
        private sealed class SettingsProvider : ISettingsProvider
        {
            /// <summary>
            /// The <see cref="IPropertySet"/> with the settings targeted by the current instance.
            /// </summary>
            [NotNull]
            private readonly IPropertySet _settingsStorage;

            /// <summary>
            /// Initializes a new instance of the <see cref="SettingsProvider"/> class that works on a specific settings <see cref="IPropertySet"/>.
            /// </summary>
            /// <param name="settings">The target <see cref="IPropertySet"/> instance to use to store the settings.</param>
            public SettingsProvider([NotNull] IPropertySet settings) => _settingsStorage = settings;

            /// <inheritdoc/>
            public void SetValue<T>(SettingKeys key, T value, bool overwrite = true, bool notify = false)
            {
                // Convert the value
                object serializable;
                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    Type type = Enum.GetUnderlyingType(typeof(T));
                    serializable = Convert.ChangeType(value, type);
                }
                else if (typeof(T).GetTypeInfo().IsPrimitive || typeof(T) == typeof(string))
                {
                    serializable = value;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    serializable = Unsafe.As<T, DateTime>(ref value).ToBinary();
                }
                else
                {
                    throw new ArgumentException($"Invalid setting of type {typeof(T)}", nameof(value));
                }

                // Store the new value
                if (!_settingsStorage.ContainsKey(key.ToString()))
                {
                    _settingsStorage.Add(key.ToString(), serializable);
                }
                else if (overwrite)
                {
                    _settingsStorage[key.ToString()] = serializable;
                }

                // Notify if needed
                if (notify)
                {
                    Messenger.Default.Send(new SettingChangedMessage<T>(key, value));
                }
            }

            /// <inheritdoc/>
            public T GetValue<T>(SettingKeys key, bool fallback = false)
            {
                // Try to get the setting value
                if (!_settingsStorage.TryGetValue(key.ToString(), out object value))
                {
                    if (fallback)
                    {
                        return default;
                    }

                    throw new InvalidOperationException($"The setting {key} doesn't exist");
                }

                // Cast and return the retrieved setting
                if (typeof(T) == typeof(DateTime))
                {
                    value = DateTime.FromBinary((long)value);
                }

                return (T)value;
            }

            /// <inheritdoc/>
            public void Clear() => _settingsStorage.Clear();
        }
    }
}
