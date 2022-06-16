// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when a guild is selected.
    /// </summary>
    public class GuildSelectedMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildSelectedMessage{T}"/> class.
        /// </summary>
        public GuildSelectedMessage(T guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets the guild selected.
        /// </summary>
        public T Guild { get; }
    }
}
