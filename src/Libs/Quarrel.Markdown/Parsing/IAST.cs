// Quarrel © 2022

using System;
using System.Collections.Generic;

namespace Quarrel.Markdown.Parsing
{
    internal interface IAST { }
    internal interface IEmojiAst : IAST { string Name { get; } }
    internal interface IMentionAst : IAST { }
    internal abstract record IASTChildren(IList<IAST> Children) : IAST;
    
    internal record CodeBlock(string Content, string Language) : IAST;
    internal record BlockQuote(IList<IAST> Children) : IASTChildren(Children);
    internal record SurrogateEmoji(string Name, string Surrogate) : IEmojiAst;
    internal record Emoji(string Name, string Id) : IEmojiAst;
    internal record AnimatedEmoji(string Name, string Id) : IEmojiAst;
    internal record Url(string Content) : IAST;
    internal record Strong(IList<IAST> Children) : IASTChildren(Children);
    internal record Em(IList<IAST> Children) : IASTChildren(Children);
    internal record U(IList<IAST> Children) : IASTChildren(Children);
    internal record S(IList<IAST> Children) : IASTChildren(Children);
    internal record InlineCode(string Content) : IAST;
    internal record Timestamp(DateTime Time, string Format) : IAST;
    internal record RoleMention(string RoleId) : IMentionAst;
    internal record UserMention(ulong UserId) : IMentionAst;
    internal record GlobalMention(string Target) : IMentionAst;
    internal record ChannelMention(string ChannelId) : IMentionAst;
    internal record Spoiler(IList<IAST> Children) : IASTChildren(Children);
    internal record Text(string Content) : IAST;
}
