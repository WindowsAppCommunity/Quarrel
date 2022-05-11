// Quarrel © 2022

using Quarrel.Bindables.Channels.Interfaces;
using Windows.UI.Xaml;

namespace Quarrel.Converters.Discord.Channels
{
    public class IsPrivateChannelVisibleConverter
    {
        public static Visibility Convert(IBindableChannel channel)
        {
            return IsPrivateChannelConverter.Convert(channel) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
