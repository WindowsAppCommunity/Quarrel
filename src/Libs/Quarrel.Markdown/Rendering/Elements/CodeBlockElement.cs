// Quarrel © 2022

using ColorCode;
using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = nameof(RichBlockPartName), Type = typeof(RichTextBlock))]
    public sealed class CodeBlockElement : MarkdownElement
    {
        private const string RichBlockPartName = "RichBlock";
        private readonly CodeBlock _codeBlock;
        
        private RichTextBlock? _richBlock;

        internal CodeBlockElement(CodeBlock codeBlock) : base(codeBlock)
        {
            this.DefaultStyleKey = typeof(CodeBlockElement);

            _codeBlock = codeBlock;
        }

        protected override void OnApplyTemplate()
        {
            _richBlock = (RichTextBlock)GetTemplateChild(RichBlockPartName);

            Render();
        }

        private void Render()
        {
            var paragraph = new Paragraph();

            _richBlock.Blocks.Clear();
            _richBlock.Blocks.Add(paragraph);

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
