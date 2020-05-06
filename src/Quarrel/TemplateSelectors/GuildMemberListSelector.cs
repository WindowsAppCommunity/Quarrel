// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Models.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the GuildMember item type.
    /// </summary>
    public sealed class GuildMemberListSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the Member template.
        /// </summary>
        public DataTemplate MemberTemplate { get; set; }

        /// <summary>
        /// Gets or sets the Group Header template.
        /// </summary>
        public DataTemplate MemberGroupTemplate { get; set; }

        /// <summary>
        /// Gets or sets the member placeholder template.
        /// </summary>
        public DataTemplate PlaceholderTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="IGuildMemberListItem"/> from the guild member list.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement)
            {
                switch (item)
                {
                    case BindableGuildMember _:
                        return MemberTemplate;

                    case BindableGuildMemberGroup _:
                        return MemberGroupTemplate;

                    default:
                        return PlaceholderTemplate;
                }
            }

            return null;
        }
    }
}
