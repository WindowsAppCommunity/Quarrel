// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.Settings
{
    public class SettingsMenuItemSelector : DataTemplateSelector
    {
        public DataTemplate? HeaderItem { get; set; }

        public DataTemplate? MenuItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                SettingsCategoryHeader => HeaderItem,
                SettingsSubPageViewModel or _ => MenuItem,
            };
        }
    }
}
