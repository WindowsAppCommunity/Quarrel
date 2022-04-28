// Quarrel © 2022

using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Message
{
    public interface AST { }
    public abstract record ASTChildren(IList<AST> Children) : AST;
    
    public record CodeBlock(string Content, string Language) : AST;
    public record BlockQuote(IList<AST> Children) : ASTChildren(Children);
    public record ASTRoot(IList<AST> Children) : ASTChildren(Children);
    public record SurrogateEmoji(string Name, string Surrogate) : AST;
    public record Emoji(string Name, string Id) : AST;
    public record AnimatedEmoji(string Name, string Id) : AST;
    public record Url(string Content) : AST;
    public record Strong(IList<AST> Children) : ASTChildren(Children);
    public record Em(IList<AST> Children) : ASTChildren(Children);
    public record U(IList<AST> Children) : ASTChildren(Children);
    public record S(IList<AST> Children) : ASTChildren(Children);
    public record InlineCode(string Content) : AST;
    public record Timestamp(DateTime Time, string Format) : AST;
    public record RoleMention(string RoleID) : AST;
    public record Mention(ulong UserID) : AST;
    public record Channel(string ChannelID) : AST;
    public record Spoiler(IList<AST> Children) : ASTChildren(Children);
    public record Text(string Content) : AST;
}
