// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Quarrel.Services.Abstract;
using Quarrel.Services.Analytics;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.Services.Analytics
{
    /// <summary>
    /// Logs analytics in the app center.
    /// </summary>
    public class AppCenterService : ServiceWithAccessTokensBase<AppCenterClientInfo>, IAnalyticsService
    {
        /// <summary>
        /// The maximum length for any property name and value.
        /// </summary>
        private const int PropertyStringMaxLength = 124; // It's 125, but one character is reserved for the leading '|' to indicate trimming

#if RELEASE
        /// <inheritdoc/>
        protected override string Path => "Tokens/AppCenter.json";
#elif INSIDER
        /// <inheritdoc/>
        protected override string Path => "Tokens/AppCenterInsider.json";
#else
        /// <inheritdoc/>
        protected override string Path => "Tokens/Fake.json";
#endif

        /// <inheritdoc/>
        public void Log(string title, params (string Property, string Value)[] data)
        {
            IDictionary<string, string> properties = data?.ToDictionary(
                pair => pair.Property,
                pair => pair.Value.Length <= PropertyStringMaxLength
                    ? pair.Value
                    : $"|{pair.Value.Substring(pair.Value.Length - PropertyStringMaxLength)}");
#if !DEBUG
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(title, properties);
#endif
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            AppCenter.Start(
                ClientInfo.Secret,
                typeof(Microsoft.AppCenter.Analytics.Analytics),
                typeof(Crashes));
        }
    }
}
