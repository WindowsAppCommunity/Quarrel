// Adam Dernis © 2022

using Quarrel.Bindables.Guilds;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors
{
    public class GuildStyleSelector : StyleSelector
    {
        public Style? GuildFolderStyle { get; set; }

        public Style? GuildStyle { get; set; }

        protected override Style? SelectStyleCore(object item, DependencyObject container)
        {
            return item switch
            {
                BindableGuildFolder => GuildFolderStyle,
                BindableGuild => GuildStyle,
                _ => null,
            };
        }
    }
}
