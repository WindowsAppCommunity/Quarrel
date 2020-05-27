// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Controls.UniformGrid;

namespace Quarrel.Controls.UniformGrid
{
    /// <summary>
    /// Referencable class object we can use to have a reference shared between
    /// our <see cref="Quarrel.Controls.UniformGrid.UniformGrid.MeasureOverride"/> and
    /// <see cref="Quarrel.Controls.UniformGrid.UniformGrid.GetFreeSpot"/> iterator.
    /// This is used so we can better isolate our logic and make it easier to test.
    /// </summary>
    internal class TakenSpotsReferenceHolder
    {
        /// <summary>
        /// Gets or sets the array to hold taken spots.
        /// True value indicates an item in the layout is fixed to that position.
        /// False values indicate free openings where an item can be placed.
        /// </summary>
        public bool[,] SpotsTaken { get; set; }

        public TakenSpotsReferenceHolder(int rows, int columns)
        {
            SpotsTaken = new bool[rows, columns];
        }

        public TakenSpotsReferenceHolder(bool[,] array)
        {
            SpotsTaken = array;
        }
    }
}
