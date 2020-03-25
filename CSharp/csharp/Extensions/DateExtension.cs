using System;

namespace QuantLib
{
    //public partial class Date
    //{
    //    public static implicit operator Date(global::System.DateTime date)
    //    {
    //        if (date.Minute == 0 && date.Second == 0 && date.Millisecond == 0)
    //            return new Date(date.Day, (Month)date.Month, date.Year);
    //        return new Date(date.Day, (Month)date.Month, date.Year, date.Hour, date.Minute, date.Second, date.Millisecond);
    //    }
    //}

    public static class DateExtension
    {
        public static DateTime AsDateTime(this Date date)
        {
            return new DateTime(date.year(), (int)date.month(), date.dayOfMonth(), date.hours(), date.minutes(), date.seconds());
        }
    }
}
