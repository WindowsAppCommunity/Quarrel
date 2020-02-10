using GalaSoft.MvvmLight.Ioc;
using Microsoft.Advertising.Ads.Requests.AdBroker;
using Microsoft.UI.Xaml.Controls;
using Quarrel.SubPages.GuildSettings.Pages;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings
{
    public sealed partial class GuildSettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        private ISubFrameNavigationService _SubFrameNavigationService => SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        public GuildSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = OverviewItem;

            if (_SubFrameNavigationService.Parameter is BindableGuild)
            {
                DataContext = _SubFrameNavigationService.Parameter;
            }

            PagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
            {
                [OverviewItem] = typeof(OverviewSettingsPage),
                [NotificationsItem] = typeof(NotificationsSettingsPage),
                [PrivacyItem] = typeof(PrivacySettingsPage),
                [RolesItem] = typeof(RolesSettingsPage),
                [EmojisItem] = typeof(EmojisSettingsPage),
                [ModerationItem] = typeof(ModerationSettingsPage),
                [AuditLogItem] = typeof(AuditLogSettingsPage),
                [MembersItem] = typeof(MembersSettingsPage),
                [InvitesItem] = typeof(InvitesSettingsPage),
                [BansItem] = typeof(BanSettingsPage)
            };
        }

        public BindableGuild ViewModel => DataContext as BindableGuild;

        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> PagesMapping;

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SettingsFrame.Navigate(PagesMapping[args.SelectedItemContainer], ViewModel);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 800;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 620;

        /// <inheritdoc/>
        public bool IsFullHeight
        {
            get => _IsFullHeight;
            set
            {
                if (SettingsFrame.Content is IAdaptiveSubPage page)
                    page.IsFullHeight = value;
                _IsFullHeight = value;
            }
        }
        private bool _IsFullHeight;
    }
}
