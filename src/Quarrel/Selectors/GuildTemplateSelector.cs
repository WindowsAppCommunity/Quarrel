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
        public DataTemplate? GuildFolderTemplate { get; set; }

        public DataTemplate? GuildTemplate { get; set; }
        
        /// <inheritdoc/>
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            return item switch
            {
                BindableGuildFolder => GuildFolderTemplate,
                BindableGuild => GuildTemplate,
                _ => null,
            };
        }
    }
}
