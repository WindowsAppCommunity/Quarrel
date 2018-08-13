using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using DiscordPipeImpersonator;
using Wire;

namespace NamedPipeWrapper
{
    public sealed class NamedPipeMessage<T> : NamedPipeMessage
    {
        public NamedPipeMessage(NamedPipeMessage originalMessage) : base(originalMessage)
        {
        }

        public T Message => (T)MessageObject;
    }

    public class NamedPipeMessage
    {
        private readonly PipeStream _stream;
        private readonly NamedPipeBase _pipe;

        public NamedPipeMessage(PipeStream stream, NamedPipeBase pipe, byte[] message)
        {
            this._stream = stream;
            this._pipe = pipe;

            using (var ms = new MemoryStream(message))
            {
                PipeFrame frame = new PipeFrame();
                frame.ReadStream(ms);
                MessageObject = frame;
               // MessageObject = "";
               // MessageObject = new Serializer().Deserialize(ms);
            }
        }

        protected NamedPipeMessage(NamedPipeMessage original)
        {
            _stream = original._stream;
            _pipe = original._pipe;
            MessageObject = original.MessageObject;
        }

        public object MessageObject { get; }

        public Task<bool> RespondAsync<T>(T message, CancellationToken ct = default(CancellationToken))
        {
            return RespondAsync(message.Serialize(), ct);
        }

        public Task<bool> RespondAsync(byte[] response, CancellationToken ct = default(CancellationToken))
        {
            return _pipe.SendAsync(_stream, response, ct);
        }
    }
}