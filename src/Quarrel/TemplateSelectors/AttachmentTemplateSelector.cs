using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DiscordAPI.Models;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the line to display in the console view
    /// </summary>
    public sealed class AttachmentTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is Attachment attachment)
            {
                int index = attachment.Filename.LastIndexOf('.');
                string filetype = attachment.Filename.Substring(index+1);
                switch (filetype)
                {
                    case "png":
                    case "jpg":
                    case "jpef": return parent.FindResource<DataTemplate>("ImageAttachmentTemplate");
                }
            }

            return null;
        }
    }
}
