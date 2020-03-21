// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages
{
    /// <summary>
    /// A converter that returns a humaized filesize based on number of byte.
    /// </summary>
    public sealed class TypeToGlyphConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var match = Regex.Match(value.ToString(), @"^.*\.(.*)$");
            switch (match.Groups[1].Value.ToLower())
            {
                case "jpg":
                case "jpeg":
                case "png":
                    return "";
                case "gif":
                case "mov":
                case "mp4":
                    return "";
            }

            return "";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}