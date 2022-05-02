// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.Localization;

namespace Quarrel.Converters.Discord.Channels
{
    /// <summary>
    /// Localizes the member count.
    /// </summary>
    public class MemberCountConverter
    {
        private const string MemberCountResource = "Channels/MemberCount";

        public static string Convert(int count)
        {
            return App.Current.Services.GetRequiredService<ILocalizationService>()[MemberCountResource, count];
        }
    }
}
