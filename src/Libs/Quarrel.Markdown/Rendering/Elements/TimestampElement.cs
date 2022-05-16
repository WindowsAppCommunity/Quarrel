// Quarrel © 2022

using Humanizer;
using Quarrel.Markdown.Parsing;

namespace Quarrel.Markdown
{
    public class TimestampElement : MarkdownTextElement
    {
        internal TimestampElement(Timestamp timestamp) : base(timestamp)
        {
            this.DefaultStyleKey = typeof(TimestampElement);
            Text = timestamp.Format switch
            {
                "F" or "" => timestamp.Time.ToString("F"),
                "D" => timestamp.Time.ToString("d MMMM yyyy"),
                "T" => timestamp.Time.ToString("T"),
                "d" => timestamp.Time.ToString("d"),
                "f" => timestamp.Time.ToString("MMMM yyyy HH:mm"),
                "t" => timestamp.Time.ToString("t"),
                "R" or _ => timestamp.Time.Humanize(),
            };
        }
    }
}
