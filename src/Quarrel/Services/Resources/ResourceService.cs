// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Services.Resources;

namespace Quarrel.Services.Resources
{
    /// <summary>
    /// Forwards resources through MVVM.
    /// </summary>
    public class ResourceService : IResourceService
    {
        /// <inheritdoc/>
        public object GetResource(string resource)
        {
            return App.Current.Resources[resource];
        }
    }
}
