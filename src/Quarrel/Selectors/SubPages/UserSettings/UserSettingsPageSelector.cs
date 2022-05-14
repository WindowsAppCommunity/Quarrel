// Quarrel © 2022

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.UserSettings
{
    public class UserSettingsPageSelector : DataTemplateSelector
    {
        public DataTemplate? BehaviorsTemplate { get; set; }

        public DataTemplate? ConnectionsTemplate { get; set; }

        public DataTemplate? DisplayTemplate { get; set; }

        public DataTemplate? MyAccountTemplate { get; set; }

        public DataTemplate? NotificationsTemplate { get; set; }

        public DataTemplate? PrivacyTemplate { get; set; }

        public DataTemplate? VoiceTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                BehaviorPageViewModel => BehaviorsTemplate,
                ConnectionsPageViewModel => ConnectionsTemplate,
                DisplayPageViewModel => DisplayTemplate,
                MyAccountPageViewModel => MyAccountTemplate,
                NotificationsPageViewModel => NotificationsTemplate,
                PrivacyPageViewModel => PrivacyTemplate,
                VoicePageViewModel => VoiceTemplate,
                _ => null,
            };
        }
    }
}
