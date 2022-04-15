// Quarrel © 2022

using Quarrel.Services.Quips.Model;
using System;
using System.Collections.Generic;

namespace Quarrel.Services.Quips
{
    /// <summary>
    /// A service for loading quips in a context.
    /// </summary>
    public class QuipService : IQuipService
    {
        private readonly List<QuipGroup> _groups;
        private readonly int _sumWeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuipService"/> class.
        /// </summary>
        public QuipService()
        {
            _groups = new List<QuipGroup>();
        }

        /// <inheritdoc/>
        public Quip GetQuip()
        {
            Random rand = new Random();
            rand.Next(_sumWeight);
            throw new NotImplementedException();
        }
    }
}
