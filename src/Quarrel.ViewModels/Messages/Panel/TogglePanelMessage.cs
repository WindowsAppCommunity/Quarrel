// Quarrel © 2022

using Quarrel.Messages.Panel;

namespace Quarrel.Messages
{
    /// <summary>
    /// A message sent when adjusting the panel is requested.
    /// </summary>
    public class TogglePanelMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TogglePanelMessage"/> class.
        /// </summary>
        public TogglePanelMessage(PanelSide side, PanelState state)
        {
            Side = side;
            State = state;
        }

        /// <summary>
        /// The side to adjust.
        /// </summary>
        public PanelSide Side { get; }
        
        /// <summary>
        /// The state to set the panel to.
        /// </summary>
        public PanelState State { get; }
    }
}
