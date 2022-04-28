// Quarrel © 2022

using Quarrel.Markdown.Parsing;

namespace Quarrel.Markdown
{
    public sealed class InlineCodeElement : MarkdownTextElement
    {
        internal InlineCodeElement(InlineCode inlineCode) : base(inlineCode)
        {
            this.DefaultStyleKey = typeof(InlineCodeElement);
            Text = inlineCode.Content;
        }
    }
}
