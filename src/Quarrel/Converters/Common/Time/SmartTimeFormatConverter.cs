// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.Localization;
using System;

namespace Quarrel.Converters.Common.Time
{
    public class SmartTimeFormatConverter
    {
        public static string Convert(DateTimeOffset time)
        {
            ILocalizationService localizationService = App.Current.Services.GetRequiredService<ILocalizationService>();
            string resource;
            DateTimeOffset now = DateTimeOffset.Now;
            var timeDiff = now - time;

            // Minutes
            if (timeDiff.TotalMinutes < 1)
            {
                resource = "Time/LTMinuteAgo";
                return localizationService[resource];
            }
            if (timeDiff.TotalMinutes < 2)
            {
                resource = "Time/OneMinuteAgo";
                return localizationService[resource];
            }
            if (timeDiff.TotalMinutes < 3)
            {
                resource = "Time/TwoMinutesAgo";
                return localizationService[resource];
            }
            if (timeDiff.TotalHours < 1)
            {
                resource = "Time/MinutesAgo";
                int count = (int)timeDiff.TotalMinutes;
                return localizationService[resource, count];
            }

            // Hours
            //if (timeDiff.TotalHours < 2)
            //{
            //    resource = "Time/OneHourAgo";
            //    return localizationService[resource];
            //}
            //if (timeDiff.TotalHours < 3)
            //{
            //    resource = "Time/TwoHoursAgo";
            //    return localizationService[resource];
            //}
            //if (timeDiff.TotalHours < 7)
            //{
            //    resource = "Time/HoursAgo";
            //    int count = (int)timeDiff.TotalHours;
            //    return localizationService[resource, count];
            //}

            // Day + Time
            string shortTime = time.ToString("t");
            if (time.DayOfYear == now.DayOfYear && time.Year == now.Year)
            {
                resource = "Time/TodayAt";
                return localizationService[resource, shortTime];
            }
            DateTime yesterday = (DateTime.Today - TimeSpan.FromDays(1)).Date;
            if (time.DayOfYear == yesterday.DayOfYear && time.Year == yesterday.Year)
            {
                resource = "Time/YesterdayAt";
                return localizationService[resource, shortTime];
            }

            return time.ToString("d");
        }
    }
}
