// Quarrel © 2022

using Quarrel.Markdown.Parsing;

namespace Quarrel.Markdown
{
    public class SpoilerElement : MarkdownBlockElement
    {
        internal SpoilerElement(Spoiler spoiler) : base(spoiler)
        {
            this.DefaultStyleKey = typeof(SpoilerElement);
        }
    }
}
