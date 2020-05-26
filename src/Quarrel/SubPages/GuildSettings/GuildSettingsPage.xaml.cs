// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Microsoft.UI.Xaml.Controls;
using Quarrel.SubPages.GuildSettings.Pages;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.SubPages.GuildSettings
{
    /// <summary>
    /// The subpage for modifying guild settings.
    /// </summary>
    public sealed partial class GuildSettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private IAnalyticsService _analyticsService = null;
        private bool _isFullHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildSettingsPage"/> class.
        /// </summary>
        public GuildSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = OverviewItem;

            if (SubFrameNavigationService.Parameter is BindableGuild guild)
            {
                DataContext = guild;
                AnalyticsService.Log(
                    Constants.Analytics.Events.OpenGuildSettings,
                    ("guild-id", guild.Model.Id));
            }

            _pagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
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
                [BansItem] = typeof(BanSettingsPage),
            };
        }

        /// <summary>
        /// Gets the Guild that settings is modifying.
        /// </summary>
        public BindableGuild ViewModel => DataContext as BindableGuild;

        /// <inheritdoc/>
        public bool IsFullHeight
        {
            get => _isFullHeight;
            set
            {
                if (SettingsFrame.Content is IAdaptiveSubPage page)
                {
                    page.IsFullHeight = value;
                }

                _isFullHeight = value;
            }
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 800;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 620;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        /// <summary>
        /// Gets the App's subframe navigation service.
        /// </summary>
        private ISubFrameNavigationService SubFrameNavigationService => SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SettingsFrame.Navigate(_pagesMapping[args.SelectedItemContainer], ViewModel);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }
    }
}
