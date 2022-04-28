// Quarrel © 2022

using System;
using System.Collections.Generic;

namespace Quarrel.Markdown.Parsing
{
    internal interface AST { }
    internal abstract record ASTChildren(IList<AST> Children) : AST;

    internal record CodeBlock(string Content, string Language) : AST;
    internal record BlockQuote(IList<AST> Children) : ASTChildren(Children);
    internal record ASTRoot(IList<AST> Children) : ASTChildren(Children);
    internal record SurrogateEmoji(string Name, string Surrogate) : AST;
    internal record Emoji(string Name, string Id) : AST;
    internal record AnimatedEmoji(string Name, string Id) : AST;
    internal record Url(string Content) : AST;
    internal record Strong(IList<AST> Children) : ASTChildren(Children);
    internal record Em(IList<AST> Children) : ASTChildren(Children);
    internal record U(IList<AST> Children) : ASTChildren(Children);
    internal record S(IList<AST> Children) : ASTChildren(Children);
    internal record InlineCode(string Content) : AST;
    internal record Timestamp(DateTime Time, string Format) : AST;
    internal record RoleMention(string RoleID) : AST;
    internal record Mention(ulong UserID) : AST;
    internal record GlobalMention(string Target) : AST;
    internal record Channel(string ChannelID) : AST;
    internal record Spoiler(IList<AST> Children) : ASTChildren(Children);
    internal record Text(string Content) : AST;
}
