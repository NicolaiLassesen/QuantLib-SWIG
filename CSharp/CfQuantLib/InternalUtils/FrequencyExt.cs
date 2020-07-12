using System;
using QlFreq = QuantLib.Frequency;
using CfFreq = CfAnalytics.Frequency;

namespace CfAnalytics.QuantLib.InternalUtils
{
    public static class FrequencyExt
    {
        public static QlFreq ToQlFrequency(this CfFreq frequency)
        {
            switch (frequency)
            {
                case CfFreq.NoFrequency: return QlFreq.NoFrequency;
                case CfFreq.Once: return QlFreq.Once;
                case CfFreq.Annual: return QlFreq.Annual;
                case CfFreq.SemiAnnual: return QlFreq.Semiannual;
                case CfFreq.EveryFourthMonth: return QlFreq.EveryFourthMonth;
                case CfFreq.Quarterly: return QlFreq.Quarterly;
                case CfFreq.BiMonthly: return QlFreq.Bimonthly;
                case CfFreq.Monthly: return QlFreq.Monthly;
                case CfFreq.EveryFourthWeek: return QlFreq.EveryFourthWeek;
                case CfFreq.BiWeekly: return QlFreq.Biweekly;
                case CfFreq.Weekly: return QlFreq.Weekly;
                case CfFreq.Daily:
                case CfFreq.Anytime: return QlFreq.Daily;
                case CfFreq.Every5Years:
                case CfFreq.Every2Years:
                default: throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null);
            }
        }

        public static CfFreq ToCfFrequency(this QlFreq frequency)
        {
            switch (frequency)
            {
                case QlFreq.NoFrequency: return CfFreq.NoFrequency;
                case QlFreq.Once: return CfFreq.Once;
                case QlFreq.Annual: return CfFreq.Annual;
                case QlFreq.Semiannual: return CfFreq.SemiAnnual;
                case QlFreq.EveryFourthMonth: return CfFreq.EveryFourthMonth;
                case QlFreq.Quarterly: return CfFreq.Quarterly;
                case QlFreq.Bimonthly: return CfFreq.BiMonthly;
                case QlFreq.Monthly: return CfFreq.Monthly;
                case QlFreq.EveryFourthWeek: return CfFreq.EveryFourthWeek;
                case QlFreq.Biweekly: return CfFreq.BiWeekly;
                case QlFreq.Weekly: return CfFreq.Weekly;
                case QlFreq.Daily: return CfFreq.Daily;
                case QlFreq.OtherFrequency:
                default: throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null);
            }
        }
    }
}