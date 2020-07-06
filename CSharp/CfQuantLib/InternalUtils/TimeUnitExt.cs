using System;
using QlTU = QuantLib.TimeUnit;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class TimeUnitExt
    {
        internal static QlTU ToQlTimeUnit(this TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Days: return QlTU.Days;
                case TimeUnit.Weeks: return QlTU.Weeks;
                case TimeUnit.Months: return QlTU.Months;
                case TimeUnit.Years: return QlTU.Years;
                case TimeUnit.Hours: return QlTU.Hours;
                case TimeUnit.Minutes: return QlTU.Minutes;
                case TimeUnit.Seconds: return QlTU.Seconds;
                case TimeUnit.Milliseconds: return QlTU.Milliseconds;
                case TimeUnit.Microseconds: return QlTU.Microseconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
            }
        }

        internal static TimeUnit ToTimeUnit(this QlTU timeUnit)
        {
            switch (timeUnit)
            {
                case QlTU.Days: return TimeUnit.Days;
                case QlTU.Weeks: return TimeUnit.Weeks;
                case QlTU.Months: return TimeUnit.Months;
                case QlTU.Years: return TimeUnit.Years;
                case QlTU.Hours: return TimeUnit.Hours;
                case QlTU.Minutes: return TimeUnit.Minutes;
                case QlTU.Seconds: return TimeUnit.Seconds;
                case QlTU.Milliseconds: return TimeUnit.Milliseconds;
                case QlTU.Microseconds: return TimeUnit.Microseconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
            }
        }
    }
}