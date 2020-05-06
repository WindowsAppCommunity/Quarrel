// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the message type.
    /// </summary>
    public sealed class MessageTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the Message template.
        /// </summary>
        public DataTemplate MessageTemplate { get; set; }

        /// <summary>
        /// Gets or sets the Action Message template.
        /// </summary>
        public DataTemplate ActionMessageTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">A <see cref="BindableMessage"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is BindableMessage msg)
            {
                return msg.Model.Type == 0 ? MessageTemplate : ActionMessageTemplate;
            }

            return null;
        }
    }
}
