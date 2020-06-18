// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.Converters.Profile.Relationships
{
    /// <summary>
    /// A converter that returns true if the user doesn't already have a blocked relation status.
    /// </summary>
    public sealed class RelationToBlockConverter
    {
        /// <summary>
        /// Whether or not to show the blocked button.
        /// </summary>
        /// <param name="value">The relation ship status.</param>
        /// <returns>Whether or not to show the blocked button</returns>
        public static bool Convert(int value)
        {
            return value != 2 && value != -1;
        }
    }
}
