// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Resources
{
    /// <summary>
    /// Forwards resources through MVVM.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// Gets a resource from App.Current.Resources.
        /// </summary>
        /// <param name="resource">Resource name.</param>
        /// <returns>The resource from App.Current.Resources.</returns>
        object GetResource(string resource);
    }
}
