// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Channels;

namespace Quarrel.ViewModels.Models.Suggesitons
{
    /// <summary>
    /// Represents a suggested channel to mention in the message box.
    /// </summary>
    public class ChannelSuggestion : ISuggestion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSuggestion"/> class.
        /// </summary>
        /// <param name="channel">The channel recommended for mentioning.</param>
        public ChannelSuggestion(BindableChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets the channel recommended for mentioning.
        /// </summary>
        public BindableChannel Channel { get; }

        /// <inheritdoc/>
        public string Surrogate => string.Format("#{0}", Channel.Model.Name);
    }
}
