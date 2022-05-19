// Quarrel © 2022

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.UserSettings
{
    public class GuildSettingsPageSelector : DataTemplateSelector
    {
        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                _ => null,
            };
        }
    }
}
