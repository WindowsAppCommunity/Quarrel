// Quarrel © 2022

using Quarrel.Bindables.Messages.Embeds;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.Messages
{
    public class AttachmentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DefaultAttachmentTemplate { get; set; }

        public DataTemplate? ImageAttachmentTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BindableAttachment attachment)
            {
                return attachment.FileExtension switch
                {
                    "png" or
                    "jpg" or
                    "jpeg" or
                    "gif" => ImageAttachmentTemplate,

                    //"mp4" or
                    //"mov" or
                    //"wmv" => VideoAttachmentTemplate

                    _ => DefaultAttachmentTemplate,
                };
            }

            return null;
        }
    }
}
