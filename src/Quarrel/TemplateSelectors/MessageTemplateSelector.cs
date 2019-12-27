using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;
using Microsoft.Advertising.WinRT.UI;

namespace Quarrel.TemplateSelectors
{
    public sealed class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageTemplate { get; set; }
        public DataTemplate ActionMessageTemplate { get; set; }
        public DataTemplate AdTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is BindableMessage msg)
            {
                if (msg.Model.Id == "Ad")
                {
                    return AdTemplate;
                }
                return msg.Model.Type == 0 ? MessageTemplate : ActionMessageTemplate;
            }
            return null;
        }
    }
}
