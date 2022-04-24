// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors
{
    /// <summary>
    /// A template selector for the guild type.
    /// </summary>
    public class GuildTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? GuildTemplate { get; set; }
        
        public DataTemplate? HomeItemTemplate { get; set; }

        public DataTemplate? GuildFolderTemplate { get; set; }
        
        /// <inheritdoc/>
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            return item switch
            {
                BindableGuild => GuildTemplate,
                BindableHomeItem => HomeItemTemplate,
                BindableGuildFolder => GuildFolderTemplate,
                _ => null,
            };
        }
    }
}
