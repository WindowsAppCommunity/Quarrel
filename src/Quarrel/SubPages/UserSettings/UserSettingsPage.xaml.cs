using Quarrel.SubPages.Interfaces;
using Quarrel.SubPages.UserSettings.Pages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.UserSettings
{
    public sealed partial class UserSettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        public UserSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = MyAccountItem;
            PagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
            {
                [MyAccountItem] = typeof(MyAccountSettingsPage),
                [PrivacyItem] = typeof(PrivacySettingsPage),
                [ConnectionsItem] = typeof(ConnectionsSettingsPage),
                [DisplayItem] = typeof(DisplaySettingsPage),
                [BehaviorItem] = typeof(BehaviorSettingsPage),
                [NotificationsItem] = typeof(NotificationsSettingsPage),
                [VoiceItem] = typeof(VoiceSettingsPage)
            };
        }

        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> PagesMapping;

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SettingsFrame.Navigate(PagesMapping[args.SelectedItemContainer]);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 800;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 620;

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
