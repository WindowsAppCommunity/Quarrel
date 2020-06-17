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
        public static string Convert(int value)
        {
            switch (value)
            {
                case 0:
                    return Helpers.Constants.Localization.GetLocalizedString("Playing");
                case 1:
                    return Helpers.Constants.Localization.GetLocalizedString("Streaming");
                case 2:
                    return Helpers.Constants.Localization.GetLocalizedString("ListeningTo");
                case 3:
                    return Helpers.Constants.Localization.GetLocalizedString("Watching");
                default:
                    return string.Empty;
            }
        }
    }
}
