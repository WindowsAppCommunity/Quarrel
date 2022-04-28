// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Quarrel.Markdown.Parsing
{
    internal static class Parser
    {
        private static Regex heading = new Regex(@"^ *(#{1,3})(?:\s+)((?![#]+)[^\n]+?)#*\s*(?:\n|$)");
        private static Regex codeBlock = new Regex(@"^```(?:([a-z0-9_+\-.]+?)\n)?\n*([^\n][\s\S]*?)\n*```");
        private static Regex customEmoji = new Regex(@"^<(a)?:(\w+):(\d+)>");
        private static Regex blockQuote = new Regex(@"^( *>>> +([\s\S]*))|^( *>(?!>>) +[^\n]*(\n *>(?!>>) +[^\n]*)*\n?)");
        private static Regex list = new Regex(@"^( *)((?:[*-]|\d+\.)) [\s\S]+?(?:\n(?! )(?!\1(?:[*-]|\d+\.) )|$)");
        private static Regex newline = new Regex(@"^(?:\n *)*\n");
        private static Regex paragraph = new Regex(@"^((?:[^\n]|\n(?! *\n))+)(?:\n *)+\n");
        private static Regex escape = new Regex(@"^\\([^0-9A-Za-z\s])");
        private static Regex autolink = new Regex(@"^<([^: >]+:\/[^ >]+)>");
        private static Regex url = new Regex(@"^((?:https?|steam):\/\/[^\s<]+[^<.,:;""'\]\s])");
        private static Regex link = new Regex(@"^\[((?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\(\s*<?((?:\([^)]*\)|[^\s\\]|\\.)*?)>?(?:\s+['""]([\s\S] *?)['""])?\s*\)");
        private static Regex strong = new Regex(@"^\*\*((?:\\[\s\S]|[^\\])+?)\*\*(?!\*)");
        private static Regex em = new Regex(@"^\b_((?:__|\\[\s\S]|[^\\_])+?)_\b|^\*(?=\S)((?:\*\*|\\[\s\S]|\s+(?:\\[\s\S]|[^\s\*\\]|\*\*)|[^\s\*\\])+?)\*(?!\*)");
        private static Regex u = new Regex(@"^__((?:\\[\s\S]|[^\\])+?)__(?!_)");
        private static Regex s = new Regex(@"^~~([\s\S]+?)~~(?!_)");
        private static Regex looseEm = new Regex(@"^\*(?=\S)((?:\*\*|\\[\s\S]|\s+(?:\\[\s\S]|[^\s\*\\]|\*\*)|[^\s\*\\])+?) {1,2}\*(?!\*)");
        private static Regex inlineCode = new Regex(@"^(`+)([\s\S]*?[^`])\1(?!`)");
        private static Regex br = new Regex(@"^ {2,}\n");
        private static Regex timestamp = new Regex(@"^<t:(-?\d{1,17})(?::(t|T|d|D|f|F|R))?>");
        private static Regex emoticon = new Regex(@"^(¯\\_\(ツ\)_\/¯)");
        private static Regex roleMention = new Regex(@"^<@&(\d+)>");
        private static Regex mention = new Regex(@"^<@!?(\d+)>|^(@(?:everyone|here))");
        private static Regex channel = new Regex(@"^<#(\d+)>");
        private static Regex emoji = new Regex(@"^:([^\s:]+?(?:::skin-tone-\d)?):");
        private static Regex spoiler = new Regex(@"^\|\|([\s\S]+?)\|\|");
        private static Regex text = new Regex(@"^[\s\S]+?(?=[^0-9A-Za-z\s\u00c0-\uffff]|\n\n| {2,}\n|\w+:\S|$)");

        private static Regex inlineCodeReplace = new Regex(@"^ (?= *`)|(` *) $");
        private static Regex codeBlockReplace = new Regex("^ *>>> ?");
        private static Regex codeBlockReplaceSingle = new Regex("^ *> ?", RegexOptions.Multiline);
        private static Regex textEmoji = new Regex("[\u200d\ud800-\udfff\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff\ufe0e\ufe0f]");
        private static Regex textEmojiSplit = new Regex("\ud83c[\udffb-\udfff](?=\ud83c[\udffb-\udfff])|(?:[^\ud800-\udfff][\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]?|[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff]|[\ud800-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|\ud83c[\udffb-\udfff])?(?:\u200d(?:[^\ud800-\udfff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe2f\u20d0-\u20ff]|\ud83c[\udffb-\udfff])?)*");
        private static Regex textUnicodeRange = new Regex("^(?:\uDB40[\uDC61-\uDC7A])$");


        internal static IList<AST> ParseAST(string text, bool inlineState, bool nested)
        {
            List<AST> collection = new List<AST>();
            while (!string.IsNullOrEmpty(text))
            {
                AST? inline = null;
                if (codeBlock.Match(text) is { Success: true } codeBlockMatch)
                {
                    inline = new CodeBlock(codeBlockMatch.Groups[2].Value, codeBlockMatch.Groups[1].Value);
                    text = text.Substring(codeBlockMatch.Length);
                }
                else if (customEmoji.Match(text) is { Success: true } customEmojiMatch)
                {
                    bool isAnimated = customEmojiMatch.Groups[1].Value == "a";
                    string name = customEmojiMatch.Groups[2].Value;
                    string id = customEmojiMatch.Groups[3].Value;
                    inline = isAnimated ? new AnimatedEmoji(name, id) : new Emoji(name, id);
                    text = text.Substring(customEmojiMatch.Length);
                }
                else if (blockQuote.Match(text) is { Success: true } blockQuoteMatch && !nested)
                {
                    string newStr = (codeBlockReplace.IsMatch(blockQuoteMatch.Value) ? codeBlockReplace : codeBlockReplaceSingle).Replace(blockQuoteMatch.Value, "");

                    inline = new BlockQuote(ParseAST(newStr.TrimEnd(), inlineState, true));
                    text = text.Substring(blockQuoteMatch.Length);
                }
                else if (newline.Match(text) is { Success: true } newlineMatch && !inlineState)
                {
                    text = text.Substring(newlineMatch.Length);
                }
                else if (paragraph.Match(text) is { Success: true } paragraphMatch && !inlineState)
                {
                    collection.AddRange(ParseAST(paragraphMatch.Groups[1].Value, inlineState, true));
                    text = text.Substring(paragraphMatch.Length);
                }
                else if (escape.Match(text) is { Success: true } escapeMatch && inlineState)
                {
                    inline = new Text(escapeMatch.Groups[1].Value);
                    text = text.Substring(escapeMatch.Length);
                }
                else if (autolink.Match(text) is { Success: true } autolinkMatch && inlineState)
                {
                    inline = new Url(autolinkMatch.Groups[1].Value);
                    text = text.Substring(autolinkMatch.Length);
                }
                else if (url.Match(text) is { Success: true } urlMatch && inlineState)
                {
                    inline = new Url(urlMatch.Groups[1].Value);
                    text = text.Substring(urlMatch.Length);
                }
                else if (strong.Match(text) is { Success: true } strongMatch && inlineState)
                {
                    var group = strongMatch.Groups[1];
                    inline = new Strong(ParseAST(group.Value, inlineState, true));
                    text = text.Substring(strongMatch.Length);
                }
                else if (em.Match(text) is { Success: true } emMatch && inlineState)
                {
                    var group = emMatch.Groups[1].Success ? emMatch.Groups[1] : emMatch.Groups[2];
                    inline = new Em(ParseAST(group.Value, inlineState, true));
                    text = text.Substring(emMatch.Length);
                }
                else if (u.Match(text) is { Success: true } uMatch && inlineState)
                {
                    var group = uMatch.Groups[1];
                    inline = new U(ParseAST(group.Value, inlineState, true));
                    text = text.Substring(uMatch.Length);
                }
                else if (s.Match(text) is { Success: true } sMatch && inlineState)
                {
                    var group = sMatch.Groups[1];
                    inline = new S(ParseAST(group.Value, inlineState, true));
                    text = text.Substring(sMatch.Length);
                }
                else if (looseEm.Match(text) is { Success: true } looseEmMatch && inlineState)
                {
                    var group = looseEmMatch.Groups[1];
                    inline = new Em(ParseAST(group.Value, inlineState, true));
                    text = text.Substring(looseEmMatch.Length);
                }
                else if (inlineCode.Match(text) is { Success: true } inlineCodeMatch && inlineState)
                {
                    inline = new InlineCode(inlineCodeReplace.Replace(inlineCodeMatch.Groups[2].Value, "$1"));
                    text = text.Substring(inlineCodeMatch.Length);
                }
                else if (br.Match(text) is { Success: true } brMatch)
                {
                    text = text.Substring(brMatch.Length);
                }
                else if (timestamp.Match(text) is { Success: true } timestampMatch)
                {
                    inline = new Timestamp(DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestampMatch.Groups[1].Value)).ToLocalTime().DateTime, timestampMatch.Groups[2].Value);
                    text = text.Substring(timestampMatch.Length);
                }
                else if (emoticon.Match(text) is { Success: true } emoticonMatch)
                {
                    inline = new Text(emoticonMatch.Value);
                    text = text.Substring(emoticonMatch.Length);
                }
                else if (roleMention.Match(text) is { Success: true } roleMentionMatch)
                {
                    inline = new RoleMention(roleMentionMatch.Groups[1].Value);
                    text = text.Substring(roleMentionMatch.Length);
                }
                else if (mention.Match(text) is { Success: true } mentionMatch)
                {
                    inline = new Mention(ulong.Parse(mentionMatch.Groups[1].Value));
                    text = text.Substring(mentionMatch.Length);
                }
                else if (channel.Match(text) is { Success: true } channelMatch)
                {
                    inline = new Channel(channelMatch.Groups[1].Value);
                    text = text.Substring(channelMatch.Length);
                }
                else if (emoji.Match(text) is { Success: true } emojiMatch && EmojiTable.SurrogateLookup.TryGetValue(emojiMatch.Groups[1].Value, out string surrogate))
                {
                    inline = new SurrogateEmoji(emojiMatch.Groups[1].Value, surrogate);
                    text = text.Substring(emojiMatch.Length);
                }
                else if (spoiler.Match(text) is { Success: true } spoilerMatch)
                {
                    inline = new Spoiler(ParseAST(spoilerMatch.Groups[1].Value, inlineState, true));
                    text = text.Substring(spoilerMatch.Length);
                }
                else if (Parser.text.Match(text) is { Success: true } textMatch)
                {
                    StringBuilder sb = new StringBuilder();
                    
                    // TODO: VERY IMPORTANT TO IMPROVE PERFORMANCE HERE
                    // StringInfo.GetTextElementEnumerator might help if it weren't broken on uwp

                    StringBuilder mergeText = new StringBuilder();
                    if (textEmoji.IsMatch(textMatch.Value))
                    {
                        StringBuilder merge = new StringBuilder();
                        foreach (Match match in textEmojiSplit.Matches(textMatch.Value))
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
                                    if (textUnicodeRange.IsMatch(splitChar))
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
