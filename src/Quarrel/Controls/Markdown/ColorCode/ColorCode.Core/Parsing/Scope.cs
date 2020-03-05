// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing
{
    /// <summary>
    /// A scope in the code.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        /// <param name="index">The starting index of the scope.</param>
        /// <param name="length">The length of the scope.</param>
        public Scope(
            string name,
            int index,
            int length)
        {
            Guard.ArgNotNullAndNotEmpty(name, "name");

            Name = name;
            Index = index;
            Length = length;
            Children = new List<Scope>();
        }

        /// <summary>
        /// Gets or sets the child scopes.
        /// </summary>
        public IList<Scope> Children { get; set; }

        /// <summary>
        /// Gets or sets the index the scope starts at.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the length of the scope.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the parent scope.
        /// </summary>
        public Scope Parent { get; set; }

        /// <summary>
        /// Gets or sets the name of the scope.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds a child scope to the scope.
        /// </summary>
        /// <param name="childScope">new child scope.</param>
        public void AddChild(Scope childScope)
        {
            if (childScope.Parent != null)
            {
                throw new InvalidOperationException("The child scope already has a parent.");
            }

            childScope.Parent = this;

            Children.Add(childScope);
        }
    }
}