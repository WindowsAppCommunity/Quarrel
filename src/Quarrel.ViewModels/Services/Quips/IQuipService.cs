// Quarrel © 2022

using Quarrel.Services.Quips.Model;

namespace Quarrel.Services.Quips
{
    /// <summary>
    /// An interface for a service that loads quips for a context.
    /// </summary>
    public interface IQuipService
    {
        /// <summary>
        /// Gets a quip.
        /// </summary>
        Quip GetQuip();
    }
}
