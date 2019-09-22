using Quarrel.Messages.Abstract;

namespace Quarrel.Messages.Navigation.SubFrame
{
    /// <summary>
    /// A message that signals whenever a previous sub page is unlocked and free to be moved around in the visual tree
    /// </summary>
    public sealed class SubFrameContentUnlockedMessage : ValueChangedMessageBase<object>
    {
        public SubFrameContentUnlockedMessage(object value) : base(value) { }
    }
}
