// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.Guilds
{
    public class GuildStyleSelector : StyleSelector
    {
        public Style? GuildStyle { get; set; }

        public Style? HomeItemStyle { get; set; }

        public Style? GuildFolderStyle { get; set; }

        protected override Style? SelectStyleCore(object item, DependencyObject container)
        {
            return item switch
            {
                BindableGuild => GuildStyle,
                BindableHomeItem => HomeItemStyle,
                BindableGuildFolder => GuildFolderStyle,
                _ => null,
            };
        }
    }
}
