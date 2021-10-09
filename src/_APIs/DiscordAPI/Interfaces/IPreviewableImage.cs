// Copyright (c) Quarrel. All rights reserved.

namespace DiscordAPI.Interfaces
{
    /// <summary>
    /// An interface for an image that can be opened in the attachment view.
    /// </summary>
    public interface IPreviewableImage
    {
        /// <summary>
        /// Gets the image's url.
        /// </summary>
        string ImageUrl { get; }

        /// <summary>
        /// Gets the image's height.
        /// </summary>
        double ImageHeight { get; }

        /// <summary>
        /// Gets the image's width.
        /// </summary>
        double ImageWidth { get; }

        /// <summary>
        /// Gets the URL to a video file or null if the image is static.
        /// </summary>
        string AnimatedImageUrl { get; }
    }
}
