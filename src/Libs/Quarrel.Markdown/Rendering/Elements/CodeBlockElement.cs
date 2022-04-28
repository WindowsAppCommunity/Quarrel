// Quarrel © 2022

using ColorCode;
using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Quarrel.Markdown
{
    public sealed class CodeBlockElement : MarkdownBlockElement
    {
        private readonly CodeBlock _codeBlock;
        
        internal CodeBlockElement(CodeBlock codeBlock) : base(codeBlock)
        {
            this.DefaultStyleKey = typeof(CodeBlockElement);

            _codeBlock = codeBlock;
        }

        protected override void Render(RichTextBlock richBlock)
        {
            var paragraph = new Paragraph();

            richBlock.Blocks.Clear();
            richBlock.Blocks.Add(paragraph);

            if (!string.IsNullOrEmpty(_codeBlock.Language) && Languages.FindById(_codeBlock.Language) is { } language)
            {
                var a = new RichTextBlockFormatter();
                a.FormatInlines(_codeBlock.Content, language, paragraph.Inlines);
            }
            else
            {
                paragraph.Inlines.Add(new Run() { Text = _codeBlock.Content });
            }
        }
    }
}
