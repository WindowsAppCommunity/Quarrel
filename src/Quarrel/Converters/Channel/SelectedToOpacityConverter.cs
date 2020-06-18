// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// A converter than returns <see langword="0.2"/> when <see langword="true"/> or <see langword="0.0"/> when <see langword="false"/>.
    /// </summary>
    public sealed class SelectedToOpacityConverter
    {
        /// <summary>
        /// Gets the opacity of the selected indicator based on if the item is selected.
        /// </summary>
        /// <param name="selected">Whether or not the item is selected.</param>
        /// <returns>The opacity of the selection border.</returns>
        public static double Convert(bool selected)
        {
            return selected ? 0.2 : 0;
        }
    }
}
