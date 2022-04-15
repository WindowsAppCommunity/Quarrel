// Quarrel © 2022

using System;

namespace Quarrel.Services.Quips.Model.Rules
{
    /// <summary>
    /// An interface for checking if a time is within a constraint.
    /// </summary>
    public interface ITimeSpanRule
    {
        /// <summary>
        /// Gets if a <see cref="DateTime"/> fits within the rule's constraint.
        /// </summary>
        /// <param name="now">The time to check.</param>
        /// <returns>A value representing whether or not the time <paramref name="now"/> fits the rule's constraint.</returns>
        public bool IsTimeBetween(DateTime now);
    }
}
