// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Models.Suggesitons
{
    /// <summary>
    /// Represents a suggestion for the MessageBox.
    /// </summary>
    public interface ISuggestion
    {
        /// <summary>
        /// Gets what to add to the message when selected.
        /// </summary>
        string Surrogate { get; }
    }
}
