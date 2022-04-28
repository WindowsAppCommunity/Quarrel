// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = nameof(RichBlockPartName), Type = typeof(RichTextBlock))]
    public abstract class MarkdownBlockElement : MarkdownElement
    {
        private const string RichBlockPartName = "RichBlock";

        internal MarkdownBlockElement(AST ast) : base(ast)
        {
        }

        protected override void OnApplyTemplate()
        {
            RichTextBlock richBlock = (RichTextBlock)GetTemplateChild(RichBlockPartName);

            Render(richBlock);
        }

        protected abstract void Render(RichTextBlock richBlock);
    }
}
