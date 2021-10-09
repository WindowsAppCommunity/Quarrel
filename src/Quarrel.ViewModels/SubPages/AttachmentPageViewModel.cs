// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Interfaces;
using DiscordAPI.Models.Messages.Embeds;
using GalaSoft.MvvmLight;

namespace Quarrel.ViewModels.ViewModels.SubPages
{
    /// <summary>
    /// Attachment page data.
    /// </summary>
    public class AttachmentPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentPageViewModel"/> class.
        /// </summary>
        /// <param name="image">The image to preview.</param>
        public AttachmentPageViewModel(IPreviewableImage image)
        {
            Image = image;
        }

        /// <summary>
        /// Gets the image on Page.
        /// </summary>
        public IPreviewableImage Image { get; }

        /// <summary>
        /// Gets a value indicating whether or not the file information can be read from the Attachment.
        /// </summary>
        public bool IsFile { get => Image is Attachment; }

        /// <summary>
        /// Gets the image as an Attachment (if available).
        /// </summary>
        public Attachment AsFile { get => Image as Attachment; }

        /// <summary>
        /// Gets a value indicating whether or not the image is animated, such as a GIFV.
        /// </summary>
        public bool IsImageAnimated { get => Image.AnimatedImageUrl != null; }
    }
}
