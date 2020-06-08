// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

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