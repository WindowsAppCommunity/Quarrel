// Quarrel © 2022

using System;

namespace Quarrel.Services.Quips.Model.Rules
{
    /// <summary>
    /// A 
    /// </summary>
    public class DailyRangeRule : ITimeSpanRule
    {
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRangeRule"/> class.
        /// </summary>
        /// <param name="start">The time of day to start as a TimeSpan.</param>
        /// <param name="end">The time of day to start as a TimeSpan.</param>
        public DailyRangeRule(TimeSpan start, TimeSpan end)
        {
            _startTime = start;
            _endTime = end;
        }

        /// <summary>
        /// If the end time comes before the start time, the active time wraps days.
        /// </summary>
        private bool IsWrapped => _endTime < _startTime;

        /// <inheritdoc/>
        public bool IsTimeBetween(DateTime now)
        {
            TimeSpan currentTime = now.TimeOfDay;
            bool isTimeBetween = currentTime < _endTime && currentTime > _startTime;
            
            // Return true when between and not wrapped or when not between and not wrapped
            return isTimeBetween ^ IsWrapped;
        }
    }
}
