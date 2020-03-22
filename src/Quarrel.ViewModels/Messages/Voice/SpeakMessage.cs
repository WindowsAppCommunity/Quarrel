// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Voice.DownstreamEvents;

namespace Quarrel.ViewModels.Messages.Voice
{
    /// <summary>
    /// A message that indicates a user has changed speaking status..
    /// </summary>
    public sealed class SpeakMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeakMessage"/> class.
        /// </summary>
        /// <param name="eventData">The speaking status.</param>
        public SpeakMessage(Speak eventData)
        {
            EventData = eventData;
        }

        /// <summary>
        /// Gets speaking status.
        /// </summary>
        public Speak EventData { get; private set; }
    }
}
