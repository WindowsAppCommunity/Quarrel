// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using Quarrel.ViewModels.Messages.Abstract;

namespace Quarrel.ViewModels.Messages.Navigation.SubFrame
{
    /// <summary>
    /// A message that signals whenever a previous sub page is unlocked and free to be moved around in the visual tree
    /// </summary>
    public sealed class SubFrameContentUnlockedMessage : ValueChangedMessageBase<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubFrameContentUnlockedMessage"/> class.
        /// </summary>
        /// <param name="value">The page in the subframe.</param>
        public SubFrameContentUnlockedMessage(object value) : base(value) { }
    }
}
