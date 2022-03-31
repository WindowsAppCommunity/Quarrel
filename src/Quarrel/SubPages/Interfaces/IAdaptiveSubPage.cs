// SubPage frame inspired by created by Sergio Pedri for BrainF*ck and Legere
// View Code in BrainF*ck
// https://github.com/Sergio0694/Brainf_ckSharp/blob/master/src/Brainf_ckSharp.Uwp/Controls/SubPages/Interfaces/IAdaptiveSubPage.cs

namespace Quarrel.SubPages.Interfaces
{
    public interface IAdaptiveSubPage
    {
        /// <summary>
        /// Sets whether or not the page is expanded on the full height of the current window
        /// </summary>
        bool IsFullHeight { set; }
    }
}
