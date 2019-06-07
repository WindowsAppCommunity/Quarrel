// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;

namespace ColorSyntax.Common
{
    public static class Regexes
    {
        public static string CNumber = "\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?";
    }
}