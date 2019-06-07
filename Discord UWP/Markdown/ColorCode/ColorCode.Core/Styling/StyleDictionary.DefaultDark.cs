using ColorSyntax.Common;

namespace ColorSyntax.Styling
{
    /// <summary>
    /// Defines the Default Dark Theme.
    /// </summary>
    public partial class StyleDictionary
    {


        /// <summary>
        /// A theme with Dark Colors.
        /// </summary>
        public static StyleDictionary DefaultDark = new StyleDictionary()
        {
            new Style(ScopeName.Attribute){ Foreground= Mustard },
            new Style(ScopeName.Brackets){ Foreground=Grey5 },
            new Style(ScopeName.BuiltinFunction){ Foreground=Turquoise },
            new Style(ScopeName.BuiltinValue){ Foreground="" },
            new Style(ScopeName.ClassName){ Foreground=Turquoise  },
            new Style(ScopeName.Comment){ Foreground=Green },
            new Style(ScopeName.Constructor){ Foreground=Blue },
            new Style(ScopeName.Continuation){ Foreground=Blue },
            new Style(ScopeName.ControlKeyword){ Foreground=Blue },
            new Style(ScopeName.CssPropertyName){ Foreground= BlueLight },
            new Style(ScopeName.CssPropertyValue){ Foreground=Grey2 },
            new Style(ScopeName.CssSelector){ Foreground=MustardLight },
            new Style(ScopeName.Delimiter){ Foreground=Grey1 },
            new Style(ScopeName.HtmlAttributeName){ Foreground=BlueLight},
            new Style(ScopeName.HtmlAttributeValue){ Foreground=Grey2 },
            new Style(ScopeName.HtmlComment){ Foreground=Green },
            new Style(ScopeName.HtmlElementName){ Foreground=Blue },
            new Style(ScopeName.HtmlEntity){ Foreground=DarkTurquoise },
            new Style(ScopeName.HtmlOperator){ Foreground=Grey3 },
            new Style(ScopeName.HtmlServerSideScript){ Foreground=Black, Background=MustardVeryLight },
            new Style(ScopeName.HtmlTagDelimiter){ Foreground=Grey5 },
            new Style(ScopeName.Intrinsic){ Foreground=Blue },
            new Style(ScopeName.Keyword){ Foreground=Blue },
            new Style(ScopeName.LanguagePrefix){ Foreground=Turquoise },
            new Style(ScopeName.MarkdownBold){ Foreground=Blue, Bold=true },
            new Style(ScopeName.MarkdownCode){ Foreground=GreenLight },
            new Style(ScopeName.MarkdownEmph){ Italic=true },
            new Style(ScopeName.MarkdownHeader){ Foreground=Blue, Bold=true },
            new Style(ScopeName.MarkdownListItem){ Foreground=MustardLight },
            new Style(ScopeName.NameSpace){ Foreground=Grey2},
            new Style(ScopeName.Number){ Foreground=GreenLight },
            new Style(ScopeName.Operator){ Foreground=Grey1 },
            new Style(ScopeName.PlainText){ Foreground=White },
            new Style(ScopeName.PowerShellAttribute){ Foreground=Grey5 },
            new Style(ScopeName.PowerShellOperator){ Foreground=Grey1 },
            new Style(ScopeName.PowerShellType){ Foreground=Turquoise },
            new Style(ScopeName.PowerShellVariable){ Foreground=Mustard },
            new Style(ScopeName.Predefined){ Foreground=Turquoise },
            new Style(ScopeName.PreprocessorKeyword){ Foreground=Grey4 },
            new Style(ScopeName.PseudoKeyword){ Foreground=Grey4 },
            new Style(ScopeName.SpecialCharacter){ Foreground=Grey1 },
            new Style(ScopeName.SqlSystemFunction){ Foreground=Turquoise },
            new Style(ScopeName.String){ Foreground=LightOrange },
            new Style(ScopeName.StringCSharpVerbatim){ Foreground=LightOrange },
            new Style(ScopeName.StringEscape){ Foreground=MustardLight },
            new Style(ScopeName.Type){ Foreground=Turquoise },
            new Style(ScopeName.TypeVariable){ Foreground=BlueLight },
            new Style(ScopeName.Warning){Foreground=BrightRed},
            new Style(ScopeName.XmlAttribute){ Foreground=BlueLight },
            new Style(ScopeName.XmlAttributeQuotes){ Foreground=Grey5 },
            new Style(ScopeName.XmlAttributeValue){ Foreground=Grey2 },
            new Style(ScopeName.XmlCDataSection){ Foreground=Orange},
            new Style(ScopeName.XmlComment){ Foreground=Green },
            new Style(ScopeName.XmlDelimiter){ Foreground=Grey5 },
            new Style(ScopeName.XmlDocComment){ Foreground=DarkGreen },
            new Style(ScopeName.XmlDocTag){ Foreground=Grey4 },
            new Style(ScopeName.XmlName){ Foreground=Blue },
            new Style(ScopeName.DiffAddition){Foreground=Olive},
            new Style(ScopeName.DiffDeletion){Foreground=SolarizeRed},
            new Style(ScopeName.DiffMeta){Foreground=RealOrange}
        };
    }
}