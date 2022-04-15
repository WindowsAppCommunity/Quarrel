// Quarrel © 2022

using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Quarrel.Services.Localization
{
    /// <summary>
    /// A service that recieves localization details.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private ResourceLoader? _loader;

        private ResourceLoader Loader
        {
            get
            {
                if (_loader is null)
                    _loader = ResourceLoader.GetForViewIndependentUse();

                return _loader;
            }
        }

        /// <inheritdoc/>
        public string this[string key] => Loader.GetString(key);

        /// <inheritdoc/>
        public string this[string key, params object[] args] => string.Format(this[key], args);

        /// <inheritdoc/>
        public bool IsRightToLeftLanguage
        {
            get
            {
                string flowDirectionSetting = ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];
                return flowDirectionSetting == "RTL";
            }
        }
        
        /// <inheritdoc/>
        public bool IsNeutralLanguage => CultureInfo.CurrentCulture.Name == "en-US";
    }
}
