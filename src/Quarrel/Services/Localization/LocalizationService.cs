// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace Quarrel.Services.Localization
{
    /// <summary>
    /// A service that recieves localization details.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private const string AndResource = "And";
        private const string OxfordAndResource = "OxfordAnd";
        private const string AndConjunctionResource = "AndConjunction";

        private ResourceLoader? _loader;

        private ResourceLoader Loader
        {
            get
            {
                if (_loader is null)
                {
                    _loader = ResourceLoader.GetForViewIndependentUse();
                }

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
        
        public string LanguageOverride
        {
            get => ApplicationLanguages.PrimaryLanguageOverride;
            set => ApplicationLanguages.PrimaryLanguageOverride = value;
        }

        /// <inheritdoc/>
        public bool IsNeutralLanguage => CultureInfo.CurrentCulture.Name == "en-US";

        /// <inheritdoc/>
        public IReadOnlyList<string> AvailableLanguages => ApplicationLanguages.ManifestLanguages;

        public string CommaList(params string[] args)
        {
            if (args.Length == 1)
            {
                return args[0];
            }
            else if (args.Length == 2)
            {
                return this[AndResource, args[0], args[1]];
            }
            else
            {
                return OxfordCommaList(args);
            }
        }

        private string OxfordCommaList(Span<string> args)
        {
            if (args.Length == 2)
            {
                return this[OxfordAndResource, args[0], args[1]];
            }
            else
            {
                return this[AndConjunctionResource, args[0], OxfordCommaList(args.Slice(1))];
            }
        }
    }
}
