using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Quarrel.Controls.Messages.Embeds;
using DiscordAPI.Models;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the line to display in the console view
    /// </summary>
    public sealed class EmbedTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageEmbedTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is Embed embed)
            {
                switch (embed.Type)
                {
                    case "image": return ImageEmbedTemplate;
                }
            }

            return null;
        }
    }
}
