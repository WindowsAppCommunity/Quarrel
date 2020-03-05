// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Styling
{
    /// <summary>
    /// Defines the Default Light Theme.
    /// </summary>
    public partial class StyleDictionary
    {
        /// <summary>
        /// Gets the default theme with Light Colors.
        /// </summary>
        public static StyleDictionary DefaultLight { get; } = new StyleDictionary()
        {
            new Style(ScopeName.Attribute) { Foreground = Mustard },
            new Style(ScopeName.Brackets) { Foreground = Grey1 },
            new Style(ScopeName.BuiltinFunction) { Foreground = TurquoiseBlue },
            new Style(ScopeName.BuiltinValue) { Foreground = string.Empty },
            new Style(ScopeName.ClassName) { Foreground = TurquoiseBlue },
            new Style(ScopeName.Comment) { Foreground = Green },
            new Style(ScopeName.Constructor) { Foreground = BrightBlue },
            new Style(ScopeName.Continuation) { Foreground = BrightBlue },
            new Style(ScopeName.ControlKeyword) { Foreground = BrightBlue },
            new Style(ScopeName.CssPropertyName) { Foreground = BlueLight },
            new Style(ScopeName.CssPropertyValue) { Foreground = Grey4 },
            new Style(ScopeName.CssSelector) { Foreground = MustardLight },
            new Style(ScopeName.Delimiter) { Foreground = Grey5 },
            new Style(ScopeName.HtmlAttributeName) { Foreground = BlueLight },
            new Style(ScopeName.HtmlAttributeValue) { Foreground = Grey4 },
            new Style(ScopeName.HtmlComment) { Foreground = Green },
            new Style(ScopeName.HtmlElementName) { Foreground = BrightBlue },
            new Style(ScopeName.HtmlEntity) { Foreground = DarkTurquoise },
            new Style(ScopeName.HtmlOperator) { Foreground = Grey3 },
            new Style(ScopeName.HtmlServerSideScript) { Foreground = Black, Background = MustardVeryLight },
            new Style(ScopeName.HtmlTagDelimiter) { Foreground = Grey1 },
            new Style(ScopeName.Intrinsic) { Foreground = BrightBlue },
            new Style(ScopeName.Keyword) { Foreground = BrightBlue },
            new Style(ScopeName.LanguagePrefix) { Foreground = TurquoiseBlue },
            new Style(ScopeName.MarkdownBold) { Foreground = BrightBlue, Bold = true },
            new Style(ScopeName.MarkdownCode) { Foreground = GreenLight },
            new Style(ScopeName.MarkdownEmph) { Italic = true },
            new Style(ScopeName.MarkdownHeader) { Foreground = BrightBlue, Bold = true },
            new Style(ScopeName.MarkdownListItem) { Foreground = MustardLight },
            new Style(ScopeName.NameSpace) { Foreground = Grey4 },
            new Style(ScopeName.Number) { Foreground = GreenLight },
            new Style(ScopeName.Operator) { Foreground = Grey5 },
            new Style(ScopeName.PlainText) { Foreground = Black },
            new Style(ScopeName.PowerShellAttribute) { Foreground = Grey1 },
            new Style(ScopeName.PowerShellOperator) { Foreground = Grey5 },
            new Style(ScopeName.PowerShellType) { Foreground = TurquoiseBlue },
            new Style(ScopeName.PowerShellVariable) { Foreground = Mustard },
            new Style(ScopeName.Predefined) { Foreground = TurquoiseBlue },
            new Style(ScopeName.PreprocessorKeyword) { Foreground = Grey2 },
            new Style(ScopeName.PseudoKeyword) { Foreground = Grey2 },
            new Style(ScopeName.SpecialCharacter) { Foreground = Grey5 },
            new Style(ScopeName.SqlSystemFunction) { Foreground = TurquoiseBlue },
            new Style(ScopeName.String) { Foreground = DarkOrange },
            new Style(ScopeName.StringCSharpVerbatim) { Foreground = DarkOrange },
            new Style(ScopeName.StringEscape) { Foreground = Mustard },
            new Style(ScopeName.Type) { Foreground = TurquoiseBlue },
            new Style(ScopeName.TypeVariable) { Foreground = BlueLight },
            new Style(ScopeName.Warning) { Foreground = BrightRed },
            new Style(ScopeName.XmlAttribute) { Foreground = BlueLight },
            new Style(ScopeName.XmlAttributeQuotes) { Foreground = Grey1 },
            new Style(ScopeName.XmlAttributeValue) { Foreground = Grey4 },
            new Style(ScopeName.XmlCDataSection) { Foreground = Orange },
            new Style(ScopeName.XmlComment) { Foreground = Green },
            new Style(ScopeName.XmlDelimiter) { Foreground = Grey1 },
            new Style(ScopeName.XmlDocComment) { Foreground = DarkGreen },
            new Style(ScopeName.XmlDocTag) { Foreground = Grey2 },
            new Style(ScopeName.XmlName) { Foreground = BrightBlue },
            new Style(ScopeName.DiffAddition) { Foreground = Olive },
            new Style(ScopeName.DiffDeletion) { Foreground = SolarizeRed },
            new Style(ScopeName.DiffMeta) { Foreground = RealOrange },
        };
    }
}