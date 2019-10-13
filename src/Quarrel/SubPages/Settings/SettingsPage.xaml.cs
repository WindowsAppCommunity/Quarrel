using Quarrel.SubPages.Interfaces;
using Quarrel.SubPages.Settings.Pages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages.Settings
{
    public sealed partial class SettingsPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        ILogger<SettingsPage> Logger { get; }

        public SettingsPage()
        {
            Logger = App.ServiceProvider.GetService<ILogger<SettingsPage>>();
            try
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
            catch (Exception ex)
            {
                Logger.LogCritical(new EventId(), ex, "Error creating settings pages.");
            }
        }

        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> PagesMapping;

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //var options = new FrameNavigationOptions
            //{
            //    TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
            //    IsNavigationStackEnabled = false
            //};

            //SettingsFrame.NavigateToType(PagesMapping[args.SelectedItemContainer], IsFullHeight, options);

            SettingsFrame.Navigate(PagesMapping[args.SelectedItemContainer]);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }



        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 800;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 620;

        private bool _IsFullHeight;

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
    }
}
