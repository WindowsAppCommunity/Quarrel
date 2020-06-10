// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Microsoft.UI.Xaml.Controls;
using Quarrel.SubPages.AddServer.Pages;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Analytics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.SubPages.AddServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddServerPage : IAdaptiveSubPage, IConstrainedSubPage
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private IAnalyticsService _analyticsService = null;
        private bool _isFullHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddServerPage"/> class.
        /// </summary>
        public AddServerPage()
        {
            this.InitializeComponent();
            this.Loaded += (_, e) => NavigationControl.SelectedItem = CreateItem;

            _pagesMapping = new ConcurrentDictionary<NavigationViewItemBase, Type>
            {
                [CreateItem] = typeof(CreateServerPage),
                [JoinItem] = typeof(JoinServerPage),
            };

            AnalyticsService.Log(Constants.Analytics.Events.OpenAddServer);
        }

        /// <inheritdoc/>
        public bool IsFullHeight
        {
            get => _isFullHeight;
            set
            {
                if (Frame.Content is IAdaptiveSubPage page)
                {
                    page.IsFullHeight = value;
                }

                _isFullHeight = value;
            }
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 500;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 340;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Frame.Navigate(_pagesMapping[args.SelectedItemContainer]);
            HeaderTB.Text = args.SelectedItemContainer.Content.ToString();
        }
    }
}
