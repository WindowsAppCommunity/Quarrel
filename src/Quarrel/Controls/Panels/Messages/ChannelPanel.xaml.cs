// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Windows.UI.Xaml.Controls;
using Quarrel.Bindables.Channels.Interfaces;
using Windows.UI.Xaml;
using Quarrel.Messages.Navigation;

namespace Quarrel.Controls.Panels.Messages
{
    public sealed partial class ChannelPanel : UserControl
    {
        private readonly IMessenger _messenger;

        public ChannelPanel()
        {
            this.InitializeComponent();
            _messenger = App.Current.Services.GetRequiredService<IMessenger>();

            _messenger.Register<ChannelSelectedMessage<IBindableSelectableChannel>>(this, OnChannelSelected);
        }

        private void OnChannelSelected(object sender, ChannelSelectedMessage<IBindableSelectableChannel> message)
        {
            var state = message.Channel switch
            {
                IBindableAudioChannel => nameof(AudioState),
                _ => nameof(DefaultState),
            };

            VisualStateManager.GoToState(this, state, true);
        }
    }
}
