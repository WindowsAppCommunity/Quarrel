// Copyright (c) Quarrel. All rights reserved.

using Microsoft.UI.Xaml.Controls;
using Quarrel.SubPages.Interfaces;
using Quarrel.SubPages.UserSettings.Pages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.SubPages.UserSettings
{
    /// <summary>
    /// The subpage for modifying app and user account settings.
    /// </summary>
    public sealed partial class UserSettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private bool _isFullHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsPage"/> class.
        /// </summary>
        public UserSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = MyAccountItem;
            _pagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
            {
                [MyAccountItem] = typeof(MyAccountSettingsPage),
                [PrivacyItem] = typeof(PrivacySettingsPage),
                [ConnectionsItem] = typeof(ConnectionsSettingsPage),
                [DisplayItem] = typeof(DisplaySettingsPage),
                [BehaviorItem] = typeof(BehaviorSettingsPage),
                [NotificationsItem] = typeof(NotificationsSettingsPage),
                [VoiceItem] = typeof(VoiceSettingsPage),
            };
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 800;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 620;

        /// <summary>
        /// Gets or sets a value indicating whether or not the page is displaying at its max height.
        /// </summary>
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

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SettingsFrame.Navigate(_pagesMapping[args.SelectedItemContainer]);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }
    }
}
