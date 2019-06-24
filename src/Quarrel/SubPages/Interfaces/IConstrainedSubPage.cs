// Special thanks to Sergio Pedri for the basis of this design

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page with size constraints
    /// </summary>
    public interface IConstrainedSubPage
    {
        /// <summary>
        /// Gets the maximum width for the current sub page
        /// </summary>
        double MaxExpandedWidth { get; }

        /// <summary>
        /// Gets the maximum height for the current sub page
        /// </summary>
        double MaxExpandedHeight { get; }
    }
}