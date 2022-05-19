// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.SubPages.Settings.GuildSettings
{
    public class GuildSettingsPageSelector : DataTemplateSelector
    {
        public DataTemplate? OverviewTemplate { get; set; }

        public DataTemplate? PrivacyTemplate { get; set; }

        public DataTemplate? NotificationsTemplate { get; set; }

        public DataTemplate? RoleTemplate { get; set; }

        public DataTemplate? EmojisTemplate { get; set; }

        public DataTemplate? ModerationTemplate { get; set; }

        public DataTemplate? AuditLogTemplate { get; set; }

        public DataTemplate? MembersTemplate { get; set; }

        public DataTemplate? InvitesTemplate { get; set; }

        public DataTemplate? BansTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                OverviewPageViewModel => OverviewTemplate,
                PrivacyPageViewModel => PrivacyTemplate,
                NotificationsPageViewModel => NotificationsTemplate,
                RolesPageViewModel => RoleTemplate,
                EmojisPageViewModel => EmojisTemplate,
                ModerationPageViewModel => ModerationTemplate,
                AuditLogPageViewModel => AuditLogTemplate,
                MembersPageViewModel => MembersTemplate,
                InvitesPageViewModel => InvitesTemplate,
                BansPageViewModel => BansTemplate,
                _ => null,
            };
        }
    }
}
