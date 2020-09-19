using System;

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class DateExt
    {
        internal static DateTime AsDateTime(this global::QuantLib.Date date)
        {
            // QL not currently compiled with intra day support
            //return new DateTime(date.year(), (int)date.month(), date.dayOfMonth(), date.hours(), date.minutes(), date.seconds());
            return new DateTime(date.year(), (int)date.month(), date.dayOfMonth());
        }
    }
}