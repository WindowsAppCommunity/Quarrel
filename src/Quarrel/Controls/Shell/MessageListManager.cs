using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quarrel.Models.Bindables;
using Quarrel.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Views
{
    public static class MessageListManager
    {
        private static MainViewModel ViewModel => App.ViewModelLocator.Main;
        private static ILogger<MessageListControl> _logger;

        private static ILogger<MessageListControl> Logger
        {
            get
            {
                return _logger ?? (_logger = App.ServiceProvider.GetService<ILogger<MessageListControl>>());
            }
        }

        public static async Task ManagerKeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control);
                var menu = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Menu);

                if (e.Key == Windows.System.VirtualKey.Q
                    && ctrl == Windows.UI.Core.CoreVirtualKeyStates.Down)
                {
                    Logger.LogTrace($"CTRL-Q Pressed.");

                    await MarkRead();

                    if ((menu & Windows.UI.Core.CoreVirtualKeyStates.Down) ==
                            Windows.UI.Core.CoreVirtualKeyStates.Down)
                    {
                        Logger.LogTrace($"CTRL-ALT-Q Pressed.");

                        MarkAllRead();
                    }

                    e.Handled = true;
                }

                if (e.Key == Windows.System.VirtualKey.N
                    && ctrl == Windows.UI.Core.CoreVirtualKeyStates.Down)
                {
                    Logger.LogTrace($"CTRL-N Pressed.");

                    await MoveNext();

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(), ex, "Error in UserControl_KeyDown");
            }
        }

        public static void ManagerKeyUp(object sender, KeyRoutedEventArgs e)
        {
        }

        private static void MarkAllRead()
        {
            ViewModel.BindableChannels.ToList().ForEach(
                async channel =>
                {
                    if (channel?._Model?.LastMessageId != null)
                    {
                        await channel.UpdateLRMID(channel._Model.LastMessageId);
                    }
                });
        }

        private static async Task MarkRead(bool scroll = true)
        {
            ViewModel.BindableMessages.Where(m => m.IsLastReadMessage).ToList().ForEach(m =>
            {
                m.IsLastReadMessage = false;
                Logger.LogTrace($"Marking Read:\n{m.Model.Content}");
            });

            var lastMessage = ViewModel.BindableMessages.OrderByDescending(m => Convert.ToInt64(m.Model.Id)).FirstOrDefault();

            if (lastMessage != null)
            {
                await ViewModel.Channel.UpdateLRMID(lastMessage.Model.Id);

                if(scroll) _scrollTo?.Invoke(lastMessage);
            }
        }

        private static async Task MoveNext()
        {
            var menu = (Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Menu) 
                        & Windows.UI.Core.CoreVirtualKeyStates.Down) 
                            == Windows.UI.Core.CoreVirtualKeyStates.Down;
            var start = ViewModel.BindableChannels.IndexOf(ViewModel.Channel);
            var index = start;

            index++;

            while (index < ViewModel.BindableChannels.Count
                && !ViewModel.BindableChannels[index].ShowUnread)
            {
                index++;
            }

            if (index < ViewModel.BindableChannels.Count)
            {
                if(menu) await MarkRead(false);
                ViewModel.NavigateChannelCommand.Execute(ViewModel.BindableChannels[index]);
            }
            else
            {
                index = 0;

                while (index < start
                    && !ViewModel.BindableChannels[index].ShowUnread)
                {
                    index++;
                }

                if (index < start)
                {
                    if (menu) await MarkRead(false);
                    ViewModel.NavigateChannelCommand.Execute(ViewModel.BindableChannels[index]);
                }
            }
        }

        private static Action<BindableMessage> _scrollTo;
        private static readonly object _lock = new object();

        public static event Action<BindableMessage> ScrollTo
        {

            add
            {
                lock (_lock)
                {
                    _scrollTo?.GetInvocationList().ToList().ForEach(
                        d => _scrollTo -= (Action<BindableMessage>)d);
                    _scrollTo += value;
                }
            }

            remove
            {
                lock (_lock)
                {
                    _scrollTo -= value;
                }
            }
        }
    }
}
