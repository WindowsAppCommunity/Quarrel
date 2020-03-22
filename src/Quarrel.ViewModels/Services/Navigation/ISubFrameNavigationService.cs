// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Views;

namespace Quarrel.ViewModels.Services.Navigation
{
    /// <summary>
    /// An <see langword="interface"/> that provides the ability to control the subframe.
    /// </summary>
    public interface ISubFrameNavigationService : INavigationService
    {
        /// <summary>
        /// Gets an extra parameter for navigation.
        /// </summary>
        object Parameter { get; }

        /// <summary>
        /// Gets how many levels of subpages deep the user is.
        /// </summary>
        int Depth { get; }
    }
}
