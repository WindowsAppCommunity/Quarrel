// Special thanks to Sergio Pedri for the basis of this design

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page with size constraints
    /// </summary>
    public interface IFullscreenSubPage
    {
        /// <summary>
        /// Is closeable from outside influence
        /// </summary>
        bool Hideable { get; }
    }
}