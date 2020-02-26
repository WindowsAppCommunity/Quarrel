// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page with size constraints.
    /// </summary>
    public interface IFullscreenSubPage
    {
        /// <summary>
        /// Gets a value indicating whether the subpage is closeable from outside influence.
        /// </summary>
        bool Hideable { get; }
    }
}