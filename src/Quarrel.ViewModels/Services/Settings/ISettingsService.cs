// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using Quarrel.ViewModels.Services.Settings.Enums;
using System;

namespace Quarrel.ViewModels.Services.Settings
{
    /// <summary>
    /// The default <see langword="interface"/> for the settings manager used in the app.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance that works on roaming settings.
        /// </summary>
        [NotNull]
        ISettingsProvider Roaming { get; }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance that works on local settings.
        /// </summary>
        [NotNull]
        ISettingsProvider Local { get; }

        /// <summary>
        /// Gets an <see cref="IServiceProvider"/> instance for a specific location.
        /// </summary>
        /// <param name="location">The location to retrieve.</param>
        [NotNull]
        ISettingsProvider this[SettingLocation location] { get; }

        /// <summary>
        /// Initializes all the necessary settings, if not present.
        /// </summary>
        void EnsureDefaults();
    }

    /// <summary>
    /// The default <see langword="interface"/> for the settings manager used in the app.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Assigns a value to a settings key.
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key.</typeparam>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        /// <param name="overwrite">Indicates whether or not to overwrite the setting, if it already exists.</param>
        /// <param name="notify">Indicates whether or not to notify the app after the setting has changed.</param>
        void SetValue<T>(SettingKeys key, T value, bool overwrite = true, bool notify = false);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="fallback">If true, the method returns the default <typeparamref name="T"/> value in case of failure.</param>
        /// <returns>The value for the settings key.</returns>
        [Pure]
        T GetValue<T>(SettingKeys key, bool fallback = false);

        /// <summary>
        /// Deletes all the existing setting values.
        /// </summary>
        void Clear();
    }
}
