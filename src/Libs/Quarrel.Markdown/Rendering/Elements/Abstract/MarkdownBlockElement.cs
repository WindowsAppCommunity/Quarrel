// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = nameof(RichBlockPartName), Type = typeof(RichTextBlock))]
    public abstract class MarkdownBlockElement : MarkdownElement
    {
        private const string RichBlockPartName = "RichBlock";

        internal MarkdownBlockElement(IAST iast) : base(iast)
        {
            Paragraph = new Paragraph();
        }

        protected override void OnApplyTemplate()
        {
            RichTextBlock richBlock = (RichTextBlock)GetTemplateChild(RichBlockPartName);
            Render(richBlock);
        }

        protected virtual void Render(RichTextBlock richBlock)
        {
            richBlock.Blocks.Add(Paragraph);
        }

        public Paragraph Paragraph { get; set; }
    }
}
