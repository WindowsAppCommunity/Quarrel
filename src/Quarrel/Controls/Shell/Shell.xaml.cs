// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Panel;
using System;
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

            action();
        }
    }
}
