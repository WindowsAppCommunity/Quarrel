// Copyright (c) Microsoft Corporation.  All rights reserved.

using ColorSyntax.Parsing;

namespace ColorSyntax.Common
{
    public class TextInsertion
    {
        public virtual int Index { get; set; }
        public virtual string Text { get; set; }
        public virtual Scope Scope { get; set; }
    }
}