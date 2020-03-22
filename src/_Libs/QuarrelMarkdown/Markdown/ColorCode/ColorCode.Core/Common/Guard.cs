// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// Adds methods to check objects for forms of nullity.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws if <paramref name="arg"/> is null.
        /// </summary>
        /// <param name="arg">object to check for nullity.</param>
        /// <param name="paramName">The name of <paramref name="arg"/>.</param>
        public static void ArgNotNull(object arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws if the <paramref name="arg"/> is null or empty.
        /// </summary>
        /// <param name="arg"><see cref="string"/> to check.</param>
        /// <param name="paramName">The name of <paramref name="arg"/>.</param>
        public static void ArgNotNullAndNotEmpty(string arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentException(string.Format("The {0} argument value must not be empty.", paramName), paramName);
            }
        }

        /// <summary>
        /// Throws if <paramref name="parameter"/> is null or empty.
        /// </summary>
        /// <typeparam name="TKey">Key type in the <paramref name="parameter"/>.</typeparam>
        /// <typeparam name="TValue">Value type in <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter"><see cref="IDictionary{TKey, TValue}"/> to check for nullity or emptiness.</param>
        /// <param name="parameterName">The name of <paramref name="parameter"/>.</param>
        public static void EnsureParameterIsNotNullAndNotEmpty<TKey, TValue>(IDictionary<TKey, TValue> parameter, string parameterName)
        {
            if (parameter == null || parameter.Count == 0)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throws if <paramref name="arg"/> is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of values in <paramref name="arg"/>.</typeparam>
        /// <param name="arg"><see cref="IList{T}"/> to check for nullity or emptiness.</param>
        /// <param name="paramName">The name of <paramref name="arg"/>.</param>
        public static void ArgNotNullAndNotEmpty<T>(IList<T> arg, string paramName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (arg.Count == 0)
            {
                throw new ArgumentException(string.Format("The {0} argument value must not be empty.", paramName), paramName);
            }
        }
    }
}
