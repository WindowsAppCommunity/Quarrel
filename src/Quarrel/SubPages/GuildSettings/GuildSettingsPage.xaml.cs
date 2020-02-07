using Microsoft.UI.Xaml.Controls;
using Quarrel.SubPages.GuildSettings.Pages;
using Quarrel.SubPages.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings
{
    public sealed partial class GuildSettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        public GuildSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = OverviewItem;
            PagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
            {
                [OverviewItem] = typeof(OverviewSettingsPage),
                [RolesItem] = typeof(RolesSettingsPage),
                [EmojisItem] = typeof(EmojisSettingsPage),
                [ModerationItem] = typeof(ModerationSettingsPage),
                [AuditLogItem] = typeof(AuditLogSettingsPage),
                [MembersItem] = typeof(MembersSettingsPage),
                [InvitesItem] = typeof(InvitesSettingsPage),
                [BansItem] = typeof(BanSettingsPage)
            };
        }

        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> PagesMapping;

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7, 0))
            {
                var options = new FrameNavigationOptions
                {
                    TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
                    IsNavigationStackEnabled = false
                };

                SettingsFrame.NavigateToType(PagesMapping[args.SelectedItemContainer], IsFullHeight, options);
                HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
            }
            else
            {
                SettingsFrame.Navigate(PagesMapping[args.SelectedItemContainer]);
                HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
            }
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
