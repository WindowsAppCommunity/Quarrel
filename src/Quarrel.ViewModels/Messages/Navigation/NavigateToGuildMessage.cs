// Quarrel © 2022

namespace Quarrel.Messages.Navigation
{
    /// <summary>
    /// A message sent when navigation to a guild is requested.
    /// </summary>
    public class NavigateToGuildMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToGuildMessage{T}"/> class.
        /// </summary>
        public NavigateToGuildMessage(T guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets the guild being navigated to.
        /// </summary>
        public T Guild { get; }
    }
}
