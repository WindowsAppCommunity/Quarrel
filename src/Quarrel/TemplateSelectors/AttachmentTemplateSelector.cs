// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Services.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the attachment type to show.
    /// </summary>
    public sealed class AttachmentTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the pure image attachment template.
        /// </summary>
        public DataTemplate ImageAttachmentTemplate { get; set; }

        /// <summary>
        /// Gets or sets the pure video attachment template.
        /// </summary>
        public DataTemplate VideoAttachmentTemplate { get; set; }

        /// <summary>
        /// Gets or sets the dynamic default attachment template.
        /// </summary>
        public DataTemplate DefaultAttachmentTemplate { get; set; }

        private ISettingsService SettingsService => SimpleIoc.Default.GetInstance<ISettingsService>();

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on details from the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="BindableAttachment"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BindableAttachment attachment)
            {
                int index = attachment.Model.Filename.LastIndexOf('.');
                string filetype = attachment.Model.Filename.Substring(index + 1);
                switch (filetype)
                {
                    case "png":
                    case "jpg":
                    case "gif":
                    case "jpeg": return ImageAttachmentTemplate;
                    case "mov":
                    case "wmv":
                    case "mp4": return VideoAttachmentTemplate;
                    default: return DefaultAttachmentTemplate;
                }
            }

            return null;
        }
    }
}
