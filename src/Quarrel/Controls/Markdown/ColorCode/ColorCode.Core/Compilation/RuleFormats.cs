// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation
{
    /// <summary>
    /// Some generic rule regexes.
    /// </summary>
    public static class RuleFormats
    {
        /// <summary>
        /// Initializes static members of the <see cref="RuleFormats"/> class.
        /// </summary>
        static RuleFormats()
        {
            const string script = @"(?xs)(<)(script)
                                        {0}[\s\n]+({1})[\s\n]*(=)[\s\n]*(""{2}""){0}[\s\n]*(>)
                                        (.*?)
                                        (</)(script)(>)";

            const string attributes = @"(?:[\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*([^\s\n""']+?)
                                           |[\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*(""[^\n]+?"")
                                           |[\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*('[^\n]+?')
                                           |[\s\n]+([a-z0-9-_]+) )*";

            JavaScript = string.Format(script, attributes, "type|language", "[^\n]*javascript");
            ServerScript = string.Format(script, attributes, "runat", "server");
        }

        /// <summary>
        /// Gets JavaScript regex.
        /// </summary>
        public static string JavaScript { get; private set; }

        /// <summary>
        /// Gets ServerScript regex.
        /// </summary>
        public static string ServerScript { get; private set; }
    }
}