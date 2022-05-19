// Quarrel © 2022

using Quarrel.ViewModels.SubPages.GuildSettings;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.GuildSettings
{
    public class GuildSettingsMenuItemSelector : DataTemplateSelector
    {
        public DataTemplate? HeaderItem { get; set; }

        public DataTemplate? MenuItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                GuildSettingsHeader => HeaderItem,
                GuildSettingsSubPageViewModel or _ => MenuItem,
            };
        }
    }
}
