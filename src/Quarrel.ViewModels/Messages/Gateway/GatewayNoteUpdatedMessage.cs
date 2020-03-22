// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message that indicates a note has been updated.
    /// </summary>
    public sealed class GatewayNoteUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayNoteUpdatedMessage"/> class.
        /// </summary>
        /// <param name="userId">The id of the user who's note updated.</param>
        /// <param name="note">The new note value for the user.</param>
        public GatewayNoteUpdatedMessage(string userId, string note)
        {
            UserId = userId;
            Note = note;
        }

        /// <summary>
        /// Gets the id of the user who's note updated.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the new note.
        /// </summary>
        public string Note { get; }
    }
}
