// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// Adds extensions methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sorts items in the list by <paramref name="comparison"/>.
        /// </summary>
        /// <typeparam name="T">Type of item in the list.</typeparam>
        /// <param name="list">The original list of items.</param>
        /// <param name="comparison">How to compare the items for sorting.</param>
        public static void SortStable<T>(
            this IList<T> list,
            Comparison<T> comparison)
        {
            Guard.ArgNotNull(list, "list");

            int count = list.Count;

            for (int j = 1; j < count; j++)
            {
                T key = list[j];

                int i = j - 1;
                for (; i >= 0 && comparison(list[i], key) > 0; i--)
                {
                    list[i + 1] = list[i];
                }

                list[i + 1] = key;
            }
        }
    }
}