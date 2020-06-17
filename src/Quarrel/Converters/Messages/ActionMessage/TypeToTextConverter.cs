// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages.ActionMessage
{
    /// <summary>
    /// A converter that returns text based on the ActionMessage type.
    /// </summary>
    public sealed class TypeToTextConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string key = "DidSomething";
            if (value is int iValue)
            {
                switch (iValue)
                {
                    case 1:
                        key = "AddedUser";
                        break;
                    case 2:
                        key = "RemovedUser";
                        break;
                    case 3:
                        key = "Called";
                        break;
                    case 4:
                        key = "ChangedChannelName";
                        break;
                    case 5:
                        key = "ChangedChannelIcon";
                        break;
                    case 6:
                        key = "PinnedMessage";
                        break;
                    case 7:
                        key = "JoinedServer";
                        break;
                }
            }

            return Helpers.Constants.Localization.GetLocalizedString(key);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
