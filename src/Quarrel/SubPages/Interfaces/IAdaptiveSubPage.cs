// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page that can adapt when the layout is updated.
    /// </summary>
    public interface IAdaptiveSubPage
    {
        /// <summary>
        /// Sets a value indicating whether or not the page is expanded on the full height of the current window.
        /// </summary>
        bool IsFullHeight { set; }
    }
}
