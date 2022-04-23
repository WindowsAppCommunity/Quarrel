// Quarrel © 2022

using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Services.Localization;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// An artifical guild item for selecting DMs.
    /// </summary>
    public class BindableHomeItem : IBindableGuildListItem
    {
        private const string HomeResouece = "Guilds/Home";
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new isntance of the <see cref="BindableHomeItem"/> class.
        /// </summary>
        public BindableHomeItem(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <inheritdoc/>
        public string? Name => _localizationService[HomeResouece];
    }
}
