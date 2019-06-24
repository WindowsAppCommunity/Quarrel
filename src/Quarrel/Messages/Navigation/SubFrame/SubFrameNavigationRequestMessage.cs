using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;

namespace Quarrel.Messages.Navigation.SubFrame
{
    /// <summary>
    /// A message that signals a request to display a page in the subframe UI
    /// </summary>
    public sealed class SubFrameNavigationRequestMessage
    {
        /// <summary>
        /// Gets the page to display
        /// </summary>
        [NotNull]
        public UserControl SubPage { get; }

        private SubFrameNavigationRequestMessage([NotNull] UserControl subPage) => SubPage = subPage;

        /// <summary>
        /// Creates a new request message to the target sub page type
        /// </summary>
        /// <param name="subPage">The secondary page to display</param>
        [Pure, NotNull]
        public static SubFrameNavigationRequestMessage To([NotNull] UserControl subPage) => new SubFrameNavigationRequestMessage(subPage);
    }
}
