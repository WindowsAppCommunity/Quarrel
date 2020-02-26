// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page with a transparent background.
    /// </summary>
    public interface ITransparentSubPage
    {
        /// <summary>
        /// Gets a value indicating whether is the background darkened.
        /// </summary>
        bool Dimmed { get; }
    }
}
