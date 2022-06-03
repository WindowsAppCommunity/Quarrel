// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Quarrel.Markdown.Parsing
{
    internal static class Parser
    {
        private static readonly Regex HeadingRegex = new(@"^ *(#{1,3})(?:\s+)((?![#]+)[^\n]+?)#*\s*(?:\n|$)");
        private static readonly Regex CodeBlockRegex = new(@"^```(?:([a-z0-9_+\-.]+?)\n)?\n*([^\n][\s\S]*?)\n*```");
        private static readonly Regex CustomEmojiRegex = new(@"^<(a)?:(\w+):(\d+)>");
        private static readonly Regex BlockQuoteRegex = new(@"^( *>>> +([\s\S]*))|^( *>(?!>>) +[^\n]*(\n *>(?!>>) +[^\n]*)*\n?)");
        private static readonly Regex ListRegex = new(@"^( *)((?:[*-]|\d+\.)) [\s\S]+?(?:\n(?! )(?!\1(?:[*-]|\d+\.) )|$)");
        private static readonly Regex NewlineRegex = new(@"^(?:\n *)*\n");
        private static readonly Regex ParagraphRegex = new(@"^((?:[^\n]|\n(?! *\n))+)(?:\n *)+\n");
        private static readonly Regex EscapeRegex = new(@"^\\([^0-9A-Za-z\s])");
        private static readonly Regex AutoLinkRegex = new(@"^<([^: >]+:\/[^ >]+)>");
        private static readonly Regex UrlRegex = new(@"^((?:https?|steam):\/\/[^\s<]+[^<.,:;""'\]\s])");
        private static readonly Regex LinkRegex = new(@"^\[((?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\(\s*<?((?:\([^)]*\)|[^\s\\]|\\.)*?)>?(?:\s+['""]([\s\S] *?)['""])?\s*\)");
        private static readonly Regex StrongRegex = new(@"^\*\*((?:\\[\s\S]|[^\\])+?)\*\*(?!\*)");
        private static readonly Regex EmRegex = new(@"^\b_((?:__|\\[\s\S]|[^\\_])+?)_\b|^\*(?=\S)((?:\*\*|\\[\s\S]|\s+(?:\\[\s\S]|[^\s\*\\]|\*\*)|[^\s\*\\])+?)\*(?!\*)");
        private static readonly Regex URegex = new(@"^__((?:\\[\s\S]|[^\\])+?)__(?!_)");
        private static readonly Regex SRegex = new(@"^~~([\S]+?)~~(?!_)");
        private static readonly Regex LooseEmRegex = new(@"^\*(?=)((?:\*\*|\\[\s\S]|\s+(?:\\[\s\S]|[^\s\*\\]|\*\*)|[^\s\*\\])+?) {1,2}\*(?!\*)");
        private static readonly Regex InlineCodeRegex = new(@"^(`+)([\s\S]*?[^`])\1(?!`)");
        private static readonly Regex BrRegex = new(@"^ {2,}\n");
        private static readonly Regex TimestampRegex = new(@"^<t:(-?\d{1,17})(?::(t|T|d|D|f|F|R))?>");
        private static readonly Regex EmoticonRegex = new(@"^(¯\\_\(ツ\)_\/¯)");
        private static readonly Regex RoleMentionRegex = new(@"^<@&(\d+)>");
        private static readonly Regex MentionRegex = new(@"^<@!?(\d+)>");
        private static readonly Regex GlobalMentionRegex = new(@"^(@(?:everyone|here))");
        private static readonly Regex ChannelRegex = new(@"^<#(\d+)>");
        private static readonly Regex EmojiRegex = new(@"^:([^\s:]+?(?:::skin-tone-\d)?):");
        private static readonly Regex SpoilerRegex = new(@"^\|\|([\s\S]+?)\|\|");
        private static readonly Regex TextRegex = new(@"^[\s\S]+?(?=[^0-9A-Za-z\s\u00c0-\uffff]|\n\n| {2,}\n|\w+:\S|$)");

        private static readonly Regex InlineCodeReplaceRegex = new(@"^ (?= *`)|(` *) $");
        private static readonly Regex CodeBlockReplaceRegex = new("^ *>>> ?");
        private static readonly Regex CodeBlockReplaceSingleRegex = new("^ *> ?", RegexOptions.Multiline);
        private static readonly Regex TextEmojiRegex = new("[\u200d\ud800-\udfff\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff\ufe0e\ufe0f]");
        private static readonly Regex TextEmojiSplitRegex = new("\ud83c[\udffb-\udfff](?=\ud83c[\udffb-\udfff])|(?:[^\ud800-\udfff][\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]?|[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff]|[\ud800-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|\ud83c[\udffb-\udfff])?(?:\u200d(?:[^\ud800-\udfff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|\ud83c[\udffb-\udfff])?)*");
        private static readonly Regex TextUnicodeRangeRegex = new("^(?:\uDB40[\uDC61-\uDC7A])$");


        internal static IList<IAST> ParseAst(string text, bool inlineState, bool inQuote)
        {
            var collection = new List<IAST>();
            while (!string.IsNullOrEmpty(text))
            {
                IAST? inline = null;
                if (CodeBlockRegex.Match(text) is { Success: true } codeBlockMatch)
                {
                    inline = new CodeBlock(codeBlockMatch.Groups[2].Value, codeBlockMatch.Groups[1].Value);
                    text = text.Substring(codeBlockMatch.Length);
                }
                else if (CustomEmojiRegex.Match(text) is { Success: true } customEmojiMatch)
                {
                    bool isAnimated = customEmojiMatch.Groups[1].Value == "a";
                    string name = customEmojiMatch.Groups[2].Value;
                    string id = customEmojiMatch.Groups[3].Value;
                    inline = isAnimated ? new AnimatedEmoji(name, id) : new Emoji(name, id);
                    text = text.Substring(customEmojiMatch.Length);
                }
                else if (BlockQuoteRegex.Match(text) is { Success: true } blockQuoteMatch && !inQuote)
                {
                    string newStr = (CodeBlockReplaceRegex.IsMatch(blockQuoteMatch.Value) ? CodeBlockReplaceRegex : CodeBlockReplaceSingleRegex).Replace(blockQuoteMatch.Value, "");

                    inline = new BlockQuote(ParseAst(newStr.TrimEnd(), inlineState, true));
                    text = text.Substring(blockQuoteMatch.Length);
                }
                else if (NewlineRegex.Match(text) is { Success: true } newlineMatch && !inlineState)
                {
                    text = text.Substring(newlineMatch.Length);
                }
                else if (ParagraphRegex.Match(text) is { Success: true } paragraphMatch && !inlineState)
                {
                    collection.AddRange(ParseAst(paragraphMatch.Groups[1].Value, inlineState, inQuote));
                    text = text.Substring(paragraphMatch.Length);
                }
                else if (EscapeRegex.Match(text) is { Success: true } escapeMatch && inlineState)
                {
                    inline = new Text(escapeMatch.Groups[1].Value);
                    text = text.Substring(escapeMatch.Length);
                }
                else if (AutoLinkRegex.Match(text) is { Success: true } autoLinkMatch && inlineState)
                {
                    inline = new Url(autoLinkMatch.Groups[1].Value);
                    text = text.Substring(autoLinkMatch.Length);
                }
                else if (UrlRegex.Match(text) is { Success: true } urlMatch && inlineState)
                {
                    string urlContent = urlMatch.Groups[1].Value;
                    for (int i = urlContent.Length - 1; i >= 0; i--)
                    {
                        if (urlContent[i] != ')')
                        {
                            int count = urlContent.Count(c => c == '(');
                            urlContent = urlContent.Substring(0, Math.Min(i + count + 1, urlContent.Length));
                            break;
                        }
                    }
                    inline = new Url(urlContent);
                    text = text.Substring(urlContent.Length);
                }
                else if (StrongRegex.Match(text) is { Success: true } strongMatch && inlineState)
                {
                    var group = strongMatch.Groups[1];
                    inline = new Strong(ParseAst(group.Value, inlineState, inQuote));
                    text = text.Substring(strongMatch.Length);
                }
                else if (EmRegex.Match(text) is { Success: true } emMatch && inlineState)
                {
                    var group = emMatch.Groups[1].Success ? emMatch.Groups[1] : emMatch.Groups[2];
                    inline = new Em(ParseAst(group.Value, inlineState, inQuote));
                    text = text.Substring(emMatch.Length);
                }
                else if (URegex.Match(text) is { Success: true } uMatch && inlineState)
                {
                    var group = uMatch.Groups[1];
                    inline = new U(ParseAst(group.Value, inlineState, inQuote));
                    text = text.Substring(uMatch.Length);
                }
                else if (SRegex.Match(text) is { Success: true } sMatch && inlineState)
                {
                    var group = sMatch.Groups[1];
                    inline = new S(ParseAst(group.Value, inlineState, inQuote));
                    text = text.Substring(sMatch.Length);
                }
                else if (LooseEmRegex.Match(text) is { Success: true } looseEmMatch && inlineState)
                {
                    var group = looseEmMatch.Groups[1];
                    inline = new Em(ParseAst(group.Value, inlineState, inQuote));
                    text = text.Substring(looseEmMatch.Length);
                }
                else if (InlineCodeRegex.Match(text) is { Success: true } inlineCodeMatch && inlineState)
                {
                    inline = new InlineCode(InlineCodeReplaceRegex.Replace(inlineCodeMatch.Groups[2].Value, "$1"));
                    text = text.Substring(inlineCodeMatch.Length);
                }
                else if (BrRegex.Match(text) is { Success: true } brMatch)
                {
                    text = text.Substring(brMatch.Length);
                }
                else if (TimestampRegex.Match(text) is { Success: true } timestampMatch)
                {
                    inline = new Timestamp(DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestampMatch.Groups[1].Value)).ToLocalTime().DateTime, timestampMatch.Groups[2].Value);
                    text = text.Substring(timestampMatch.Length);
                }
                else if (EmoticonRegex.Match(text) is { Success: true } emoticonMatch)
                {
                    inline = new Text(emoticonMatch.Value);
                    text = text.Substring(emoticonMatch.Length);
                }
                else if (RoleMentionRegex.Match(text) is { Success: true } roleMentionMatch)
                {
                    inline = new RoleMention(roleMentionMatch.Groups[1].Value);
                    text = text.Substring(roleMentionMatch.Length);
                }
                else if (MentionRegex.Match(text) is { Success: true } mentionMatch)
                {
                    inline = new UserMention(ulong.Parse(mentionMatch.Groups[1].Value));
                    text = text.Substring(mentionMatch.Length);
                }
                else if (GlobalMentionRegex.Match(text) is { Success: true } globalMentionMatch)
                {
                    inline = new GlobalMention(globalMentionMatch.Groups[1].Value);
                    text = text.Substring(globalMentionMatch.Length);
                }
                else if (ChannelRegex.Match(text) is { Success: true } channelMatch)
                {
                    inline = new ChannelMention(channelMatch.Groups[1].Value);
                    text = text.Substring(channelMatch.Length);
                }
                else if (EmojiRegex.Match(text) is { Success: true } emojiMatch && EmojiTable.SurrogateLookup.TryGetValue(emojiMatch.Groups[1].Value, out string surrogate))
                {
                    inline = new SurrogateEmoji(emojiMatch.Groups[1].Value, surrogate);
                    text = text.Substring(emojiMatch.Length);
                }
                else if (SpoilerRegex.Match(text) is { Success: true } spoilerMatch)
                {
                    inline = new Spoiler(ParseAst(spoilerMatch.Groups[1].Value, inlineState, inQuote));
                    text = text.Substring(spoilerMatch.Length);
                }
                else if (TextRegex.Match(text) is { Success: true } textMatch)
                {
                    var sb = new StringBuilder();
                    
                    // TODO: VERY IMPORTANT TO IMPROVE PERFORMANCE HERE
                    // StringInfo.GetTextElementEnumerator might help if it weren't broken on uwp

                    var mergeText = new StringBuilder();
                    if (TextEmojiRegex.IsMatch(textMatch.Value))
                    {
                        var merge = new StringBuilder();
                        foreach (Match match in TextEmojiSplitRegex.Matches(textMatch.Value))
                        {
                            string splitChar = match.Value;
                            if (merge.Length != 0)
                            {
                                if (splitChar == char.ConvertFromUtf32(917631))
                                {
                                    splitChar = merge.Append(splitChar).ToString();
                                    merge.Clear();
                                }
                                else
                                {
                                    if (TextUnicodeRangeRegex.IsMatch(splitChar))
                                    {
                                        merge.Append(splitChar);
                                        continue;
                                    }

                                    string newString = merge.ToString();


                                    if (EmojiTable.EmojiLookup.TryGetValue(newString, out string emojiName))
                                    {
                                        if (mergeText.Length > 0)
                                        {
                                            collection.Add(new Text(mergeText.ToString()));
                                            mergeText.Clear();
                                        }

                                        collection.Add(new SurrogateEmoji(emojiName, newString));
                                    }
                                    else
                                    {
                                        mergeText.Append(splitChar);
                                    }

                                    merge.Clear();
                                }
                            }
                            else if (splitChar == char.ConvertFromUtf32(127988))
                            {
                                merge.Append(splitChar);
                                continue;
                            }

                            if (EmojiTable.EmojiLookup.TryGetValue(splitChar, out string emoji))
                            {
                                if (mergeText.Length > 0)
                                {
                                    collection.Add(new Text(mergeText.ToString()));
                                    mergeText.Clear();
                                }

                                collection.Add(new SurrogateEmoji(emoji, splitChar));
                            }
                            else
                            {
                                mergeText.Append(splitChar);
                            }
                        }

                    }
                    else
                    {
                        foreach (char c in textMatch.Value)
                        {
                            string cStr = c.ToString();
                            if (EmojiTable.EmojiLookup.TryGetValue(cStr, out string emoji))
                            {
                                if (mergeText.Length > 0)
                                {
                                    collection.Add(new Text(mergeText.ToString()));
                                    mergeText.Clear();
                                }

                                collection.Add(new SurrogateEmoji(emoji, cStr));
                            }
                            else
                            {
                                mergeText.Append(c);
                            }
                        }
                    }

                    if (mergeText.Length > 0)
                    {
                        collection.Add(new Text(mergeText.ToString()));
                    }
                    text = text.Substring(textMatch.Length);
                }
                else
                {
                    throw new Exception("Failed to parse");
                }

                if (inline != null)
                {
                    collection.Add(inline);
                }                
            }

            return collection;
        }

    }
}
