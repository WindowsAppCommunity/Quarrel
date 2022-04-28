// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Quarrel.Markdown
{
    public sealed class BlockQuoteElement : MarkdownBlockElement
    {
        internal BlockQuoteElement(BlockQuote blockQuote) : base(blockQuote)
        {
            this.DefaultStyleKey = typeof(BlockQuoteElement);
        }
    }
}
