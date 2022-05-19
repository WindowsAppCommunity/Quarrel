// Quarrel © 2022

using Quarrel.ViewModels.SubPages.DiscordStatus;
using Quarrel.ViewModels.SubPages.GuildSettings;
using Quarrel.ViewModels.SubPages.Meta;
using Quarrel.ViewModels.SubPages.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages
{
    public class SubPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AboutTemplate { get; set; }
        
        public DataTemplate? CreditTemplate { get; set; }

        public DataTemplate? DiscordStatusTemplate { get; set; }

        public DataTemplate? GuildSettingsTemplate { get; set; }

        public DataTemplate? UserSettingsTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                AboutPageViewModel => AboutTemplate,
                CreditPageViewModel => CreditTemplate,
                DiscordStatusViewModel => DiscordStatusTemplate,
                GuildSettingsPageViewModel => GuildSettingsTemplate,
                UserSettingsPageViewModel => UserSettingsTemplate,
                _ => null,
            };
        }
    }
}
