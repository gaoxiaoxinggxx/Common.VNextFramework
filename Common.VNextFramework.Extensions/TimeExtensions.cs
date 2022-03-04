using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Common.VNextFramework.Extensions
{
    public static class TimeExtensions
    {
        //
        // Summary:
        //     Gets the date component of this instance.
        //
        // Returns:
        //     A new object with the same date and offset as this instance, and the time value set to 12:00:00
        //     midnight (00:00:00).
        public static DateTimeOffset GetDate(this DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year,dateTimeOffset.Month,dateTimeOffset.Day,0,0,0, dateTimeOffset.Offset);
        }

        public static DateTimeOffset ToStandardTime(this DateTimeOffset dateTimeOffset, TimeZoneIdEnum timeZoneId)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, timeZoneId.GetDescription());
        }

        public static DateTime ToStandardTime(this DateTime dateTime, TimeZoneIdEnum timeZoneId)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timeZoneId.GetDescription());
        }
        public static bool IsBetween(this DateTime dateTime, DateTime startTime, DateTime endTime)
        {
            return dateTime >= startTime && dateTime < endTime;
        }

        public static bool IsBetween(this DateTimeOffset dateTime, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return dateTime >= startTime && dateTime < endTime;
        }

        public static List<string> GetStartToEndDates(this DateTimeOffset startDateTime, DateTimeOffset endDateTime, string format = "yyyy-MM-dd")
        {
            var result = new List<string>();
            for (DateTimeOffset dt = startDateTime; dt <= endDateTime; dt = dt.AddDays(1))
            {
                result.Add(dt.Date.ToString(format));
            }
            return result;
        }

    }

    public enum TimeZoneIdEnum
    {
        [Description("China Standard Time")]
        ChinaStandard
    }
}