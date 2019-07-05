using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.TemplateSelectors
{
    public sealed class MessageTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is BindableMessage msg)
            {
                return parent.FindResource<DataTemplate>(msg.Model.Type == 0 ? "MessageTemplate" : "ActionMessageTemplate");
            }
            return null;
        }
    }
}
