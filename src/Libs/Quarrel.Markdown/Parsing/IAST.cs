// Quarrel © 2022

using System;
using System.Collections.Generic;

namespace Quarrel.Markdown.Parsing
{
    internal interface IAST { }
    internal interface IEmojiAST : IAST { string Name { get; } }
    internal interface IMentionAST : IAST { }
    internal abstract record ASTChildren(IList<IAST> Children) : IAST;
    
    internal record CodeBlock(string Content, string Language) : IAST;
    internal record BlockQuote(IList<IAST> Children) : ASTChildren(Children);
    internal record SurrogateEmoji(string Name, string Surrogate) : IEmojiAST;
    internal record Emoji(string Name, string Id) : IEmojiAST;
    internal record AnimatedEmoji(string Name, string Id) : IEmojiAST;
    internal record Url(string Content) : IAST;
    internal record Strong(IList<IAST> Children) : ASTChildren(Children);
    internal record Em(IList<IAST> Children) : ASTChildren(Children);
    internal record U(IList<IAST> Children) : ASTChildren(Children);
    internal record S(IList<IAST> Children) : ASTChildren(Children);
    internal record InlineCode(string Content) : IAST;
    internal record Timestamp(DateTime Time, string Format) : IAST;
    internal record RoleMention(string RoleID) : IMentionAST;
    internal record UserMention(ulong UserID) : IMentionAST;
    internal record GlobalMention(string Target) : IMentionAST;
    internal record ChannelMention(string ChannelID) : IMentionAST;
    internal record Spoiler(IList<IAST> Children) : ASTChildren(Children);
    internal record Text(string Content) : IAST;
}
