// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Profile
{
    /// <summary>
    /// A converter that takes gets an activity type string.
    /// </summary>
    public class PresenceTypeToStatusConverter
    {
        /// <summary>
        /// Converts a presence type to an action prefix.
        /// </summary>
        /// <param name="value">The playing type.</param>
        /// <returns>The activity prefix.</returns>
        public object Convert(int value)
        {
            // TODO: Localization
            switch (value)
            {
                case 0:
                    return "Playing";
                case 1:
                    return "Streaming";
                case 2:
                    return "Listening to";
                case 3:
                    return "Watching";
                default:
                    return string.Empty;
            }
        }
    }
}
