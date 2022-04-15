// Quarrel © 2022

using Quarrel.Services.Quips.Model.Rules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Quarrel.Services.Quips.Model
{
    /// <summary>
    /// A group of quips chosen at random.
    /// </summary>
    public class QuipGroup
    {
        /// <summary>
        /// Gets the name of the quip group.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set;}
        
        /// <summary>
        /// Gets the weight of the quip group.
        /// </summary>
        /// <remarks>
        /// This is the weighted likely hood a quip from this group should appear when available.
        /// </remarks>
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
        
        /// <summary>
        /// Gets the culture of the quips.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the <see cref="ITimeSpanRule"/> that determines if the group is active.
        /// </summary>
        [JsonPropertyName("rule")]
        public ITimeSpanRule Rule { get; }

        /// <summary>
        /// Gets the list of quips to pick from in the group
        /// </summary>
        [JsonPropertyName("quips")]
        public List<Quip> Quips { get; set;}

        /// <summary>
        /// Gets whether or not the group is active in a given context.
        /// </summary>
        /// <param name="now">The time to check if active.</param>
        /// <param name="culture">The culture context.</param>
        public bool Active(DateTime now, CultureInfo culture)
        {
            if (Culture != culture)
            {
                return false;
            }

            // Check time rule
            if (Rule is null)
            {
                return true;
            }

            return Rule.IsTimeBetween(now);
        }
    }
}
