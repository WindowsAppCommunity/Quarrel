// Copyright (c) Microsoft Corporation.  All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    public class TextInsertion
    {
        public virtual int Index { get; set; }
        public virtual string Text { get; set; }
        public virtual Scope Scope { get; set; }
    }
}