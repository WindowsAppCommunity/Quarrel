// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the guild type.
    /// </summary>
    public class GuildTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the guild template.
        /// </summary>
        public DataTemplate GuildTemplate { get; set; }

        /// <summary>
        /// Gets or sets the guild folder template.
        /// </summary>
        public DataTemplate GuildFolderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the empty guild folder template.
        /// </summary>
        public DataTemplate EmptyGuildFolderTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">A <see cref="IGuildListItem"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IGuildListItem channel)
            {
                switch (item)
                {
                    case BindableGuild _:
                        return GuildTemplate;
                    case BindableGuildFolder folder:
                        {
                            if (folder.Model.Id != null)
                            {
                                return GuildFolderTemplate;
                            }

                            return EmptyGuildFolderTemplate;
                        }
                }
            }

            return null;
        }
    }
}
