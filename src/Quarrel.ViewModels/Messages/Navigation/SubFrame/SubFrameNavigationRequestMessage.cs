// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;

namespace Quarrel.ViewModels.Messages.Navigation.SubFrame
{
    /// <summary>
    /// A message that signals a request to display a page in the subframe UI.
    /// </summary>
    public sealed class SubFrameNavigationRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubFrameNavigationRequestMessage"/> class.
        /// </summary>
        /// <param name="subPage">The page to navigate to.</param>
        private SubFrameNavigationRequestMessage([NotNull] object subPage) => SubPage = subPage;

        /// <summary>
        /// Gets the page to display.
        /// </summary>
        [NotNull]
        public object SubPage { get; }

        /// <summary>
        /// Creates a new request message to the target sub page type.
        /// </summary>
        /// <param name="subPage">The secondary page to display.</param>
        /// <returns>Creates a new <see cref="SubFrameNavigationRequestMessage"/>.</returns>
        [Pure]
        [NotNull]
        public static SubFrameNavigationRequestMessage To([NotNull] object subPage) => new SubFrameNavigationRequestMessage(subPage);
    }
}
