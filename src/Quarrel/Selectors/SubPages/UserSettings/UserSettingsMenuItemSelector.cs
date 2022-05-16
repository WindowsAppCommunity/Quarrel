// Quarrel © 2022

using Quarrel.ViewModels.SubPages.UserSettings;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.UserSettings
{
    public class UserSettingsMenuItemSelector : DataTemplateSelector
    {
        public DataTemplate? HeaderItem { get; set; }

        public DataTemplate? MenuItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                UserSettingsHeader => HeaderItem,
                UserSettingsSubPageViewModel or _ => MenuItem,
            };
        }
    }
}
