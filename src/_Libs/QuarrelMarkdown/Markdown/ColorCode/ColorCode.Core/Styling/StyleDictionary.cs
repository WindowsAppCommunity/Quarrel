// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System.Collections.ObjectModel;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Styling
{
    /// <summary>
    /// A dictionary of <see cref="Style" /> instances, keyed by the styles' scope name.
    /// </summary>
    public partial class StyleDictionary : KeyedCollection<string, Style>
    {
        private const string Black = "#000000";
        private const string Mustard = "#b58900";
        private const string RealOrange = "#cb4b16";
        private const string MustardLight = "#D7BA7D";
        private const string MustardVeryLight = "#FFFFB3";
        private const string Orange = "#E9D585";
        private const string White = "#dcdcdc";
        private const string Grey1 = "#d4d4d4";
        private const string Grey2 = "#C8C8C8";
        private const string Grey3 = "#b4b4b4";
        private const string Grey4 = "#9B9B9B";
        private const string Grey5 = "#808080";
        private const string Turquoise = "#4EC9B0";
        private const string DarkTurquoise = "#00A0A0";
        private const string TurquoiseBlue = "#2B91AF";
        private const string Green = "#57A64A";
        private const string GreenLight = "#b5cea8";
        private const string DarkGreen = "#006400";
        private const string Olive = "#859900";
        private const string Blue = "#569CD6";
        private const string BrightBlue = "#0000FF";
        private const string BlueLight = "#9CDCFE";
        private const string LightOrange = "#D69D85";
        private const string DarkOrange = "#A31515";
        private const string SolarizeRed = "#dc322f";
        private const string BrightRed = "#ff0000";

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(Style item)
        {
            return item.ScopeName;
        }
    }
}