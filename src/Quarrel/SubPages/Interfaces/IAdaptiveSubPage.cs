// Special thanks to Sergio Pedri for the basis of this design

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page that can adapt when the layout is updated
    /// </summary>
    public interface IAdaptiveSubPage
    {
        /// <summary>
        /// Sets whether or not the page is expanded on the full height of the current window
        /// </summary>
        bool IsFullHeight { set; }
    }
}
