// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Panel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        private readonly IMessenger _messenger;

        public Shell()
        {
            this.InitializeComponent();
            _messenger = App.Current.Services.GetRequiredService<IMessenger>();

            _messenger.Register<NavigateToGuildMessage>(this, (_,_) => _messenger.Send(new TogglePanelMessage(PanelSide.Left, PanelState.Open)));
            _messenger.Register<TogglePanelMessage>(this, (_, e) => GoToPanelState(e));
        }

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.Medium"/> size UI.
        /// </summary>
        public double MediumMinSize { get; set; } = 600;

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.Large"/> size UI.
        /// </summary>
        public double LargeMinSize { get; set; } = 1100;

        /// <summary>
        /// Gets or sets the minimum size where the <see cref="SideDrawer"/> will enter the <see cref="SideDrawerSize.ExtraLarge"/> size UI.
        /// </summary>
        public double ExtraLargeMinSize { get; set; } = 1400;

        private void GoToPanelState(TogglePanelMessage state)
        {
            Action? action = state switch
            {
                { Side: PanelSide.Left, State: PanelState.Open } => Drawer.OpenLeft,
                { Side: PanelSide.Left, State: PanelState.Closed } => Drawer.CloseLeft,
                { Side: PanelSide.Left, State: PanelState.Toggle } => Drawer.ToggleLeft,
                { Side: PanelSide.Right, State: PanelState.Open } => Drawer.OpenRight,
                { Side: PanelSide.Right, State: PanelState.Closed } => Drawer.CloseRight,
                { Side: PanelSide.Right, State: PanelState.Toggle } => Drawer.ToggleRight,
                _ => null,
            };

            action?.Invoke();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            string stateName = string.Empty;
            if (width < MediumMinSize)
            {
                stateName = nameof(Small);
                Drawer.Size = SideDrawerSize.Small;
            }
            else if (width < LargeMinSize)
            {
                stateName = nameof(Medium);
                Drawer.Size = SideDrawerSize.Medium;
            }
            else if (width < ExtraLargeMinSize)
            {
                stateName = nameof(Large);
                Drawer.Size = SideDrawerSize.Large;
            }
            else
            {
                stateName = nameof(ExtraLarge);
                Drawer.Size = SideDrawerSize.ExtraLarge;
            }

            VisualStateManager.GoToState(this, stateName, true);
        }
    }
}
