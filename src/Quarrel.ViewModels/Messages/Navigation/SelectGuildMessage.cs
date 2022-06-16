// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent to request selecting a guild.
    /// </summary>
    public class SelectGuildMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectGuildMessage{T}"/> class.
        /// </summary>
        public SelectGuildMessage(T guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets the guild to request selecting.
        /// </summary>
        public T Guild { get; }
    }
}
