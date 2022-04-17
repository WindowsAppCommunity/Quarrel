// Quarrel © 2022

using Quarrel.Messages.Panel;

namespace Quarrel.Messages
{
    public class TogglePanelMessage
    {
        public TogglePanelMessage(PanelSide side, PanelState state)
        {
            Side = side;
            State = state;
        }

        public PanelSide Side { get; }
        
        public PanelState State { get; }
    }
}
