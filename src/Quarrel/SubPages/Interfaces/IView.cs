// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for views that expose a view model of a specific type
    /// </summary>
    /// <typeparam name="T">The view model type for the current instance</typeparam>
    public interface IView<out T>
    {
        /// <summary>
        /// Gets the view model for the current instance
        /// </summary>
        [NotNull]
        T ViewModel { get; }
    }
}