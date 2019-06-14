using System;

namespace NamedPipeWrapper
{
    public class ClientConnectedArgs : EventArgs
    {
        public ClientConnectedArgs(NamedPipeServer server)
        {
            Server = server;
        }

        public NamedPipeServer Server { get; set; }
    }
}