// Adam Dernis © 2022

using Quarrel.Bindables.Channels.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors
{
    public class ChannelEnabledStyleSelector : StyleSelector
    {
        public Style EnabledStyle { get; set; }

        public Style DisabledStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is BindableGuildChannel channel)
            {
                return channel.IsAccessible switch
                {
                    true => EnabledStyle,
                    false => DisabledStyle,
                };
            }

            return EnabledStyle;
        }
    }
}
