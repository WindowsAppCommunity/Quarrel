// Quarrel © 2022

using System;
using System.Text.Json;

namespace Discord.API.Exceptions
{
    /// <summary>
    /// A base class for exceptions thrown while parsing a socket frame.
    /// </summary>
    public class SocketFrameException : JsonException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketFrameException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="opcode">The op code of the message if available.</param>
        /// <param name="eventName">The name of the event type if available.</param>
        /// <param name="innerException">The inner exception.</param>
        public SocketFrameException(string? message = null, int? opcode = null, string? eventName = null, Exception? innerException = null) :
            base(message, innerException)
        {
            OPCode = opcode;
            EventName = eventName;
        }

        /// <summary>
        /// Gets the op code of the socket frame, or null if unavailable.
        /// </summary>
        public int? OPCode { get; }

        /// <summary>
        /// Gets the name of the event type, or null if unavailable.
        /// </summary>
        public string? EventName { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.GetType().Name} (OP: {this.OPCode}) (EventName: {this.EventName}): {this.Message}";
        }
    }
}
