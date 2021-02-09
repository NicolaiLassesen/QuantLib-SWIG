using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlCalendar = QuantLib.Calendar;

// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public class Calendar
    {
        internal QlCalendar QlObj { get; }

        public Calendar(CalendarName calendar)
        {
            QlObj = calendar.ToQlCalendar();
        }

        public DateTime Advance(DateTime date, Period period)
        {
            return QlObj.advance(date, period.QlObj).AsDateTime();
        }
    }

    [Flags]
    public enum CalendarName
    {
        Unknown,
        TARGET,
        WeekendsOnly,
        UnitedStates,
        UnitedStatesFederalReserve,
        UnitedStatesSettlement,
        UnitedStatesNYSE,
        UnitedStatesGovernmentBond,
        UnitedStatesNERC,
        UnitedStatesLiborImpact,
        UnitedKingdom,
        UnitedKingdomExchange,
        UnitedKingdomMetals,
        UnitedKingdomSettlement,
        Canada,
        CanadaSettlement,
        CanadaTSX,
        Sweden,
        Switzerland,
        Denmark,
        Norway
    }

    internal static class CalendarExt
    {
        internal static QlCalendar ToQlCalendar(this CalendarName calendar)
        {
            switch (calendar)
            {
                case CalendarName.TARGET:
                    return new global::QuantLib.TARGET();
                case CalendarName.WeekendsOnly:
                    return new global::QuantLib.WeekendsOnly();
                case CalendarName.UnitedStates:
                    return new global::QuantLib.UnitedStates();
                case CalendarName.UnitedStatesFederalReserve:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.FederalReserve);
                case CalendarName.UnitedStatesSettlement:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.Settlement);
                case CalendarName.UnitedStatesNYSE:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.NYSE);
                case CalendarName.UnitedStatesGovernmentBond:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.GovernmentBond);
                case CalendarName.UnitedStatesNERC:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.NERC);
                case CalendarName.UnitedStatesLiborImpact:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.LiborImpact);
                case CalendarName.UnitedKingdom:
                    return new global::QuantLib.UnitedKingdom();
                case CalendarName.UnitedKingdomExchange:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Exchange);
                case CalendarName.UnitedKingdomMetals:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Metals);
                case CalendarName.UnitedKingdomSettlement:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Settlement);
                case CalendarName.Canada:
                    return new global::QuantLib.Canada();
                case CalendarName.CanadaSettlement:
                    return new global::QuantLib.Canada(global::QuantLib.Canada.Market.Settlement);
                case CalendarName.CanadaTSX:
                    return new global::QuantLib.Canada(global::QuantLib.Canada.Market.TSX);
                case CalendarName.Sweden:
                    return new global::QuantLib.Sweden();
                case CalendarName.Switzerland:
                    return new global::QuantLib.Switzerland();
                case CalendarName.Denmark:
                    return new global::QuantLib.Denmark();
                case CalendarName.Norway:
                    return new global::QuantLib.Norway();
                default:
                    throw new ArgumentOutOfRangeException(nameof(calendar), calendar, "Unmapped calendar");
            }
        }
    }
}