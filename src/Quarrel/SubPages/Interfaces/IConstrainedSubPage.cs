// SubPage frame created by Sergio Pedri for BrainF*ck and Legere
// View Code in BrainF*ck
// https://github.com/Sergio0694/Brainf_ckSharp

namespace Quarrel.SubPages.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a sub page with size constraints.
    /// </summary>
    public interface IConstrainedSubPage
    {
        /// <summary>
        /// Gets the maximum width for the current sub page.
        /// </summary>
        double MaxExpandedWidth { get; }

        /// <summary>
        /// Gets the maximum height for the current sub page.
        /// </summary>
        double MaxExpandedHeight { get; }
    }
}
