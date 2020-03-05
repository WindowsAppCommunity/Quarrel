// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// Constant regexes.
    /// </summary>
    public static class Regex
    {
        /// <summary>
        /// A regex for finding numbers.
        /// </summary>
        public const string CNumber = "\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?";
    }
}