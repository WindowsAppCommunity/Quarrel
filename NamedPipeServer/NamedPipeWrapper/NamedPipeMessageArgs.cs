using System;

namespace NamedPipeWrapper
{
    public class NamedPipeMessageArgs : EventArgs
    {
        public NamedPipeMessageArgs(NamedPipeMessage message)
        {
            Message = message;
        }

        public NamedPipeMessage Message { get; }
    }
}