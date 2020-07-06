using System;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public enum Calendar
    {
        TARGET,
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
        UnitedKingdomSettlement
    }

    internal static class CalendarExt
    {
        internal static global::QuantLib.Calendar ToQlCalendar(this Calendar calendar)
        {
            switch (calendar)
            {
                case Calendar.TARGET:
                    return new global::QuantLib.TARGET();
                case Calendar.UnitedStates:
                    return new global::QuantLib.UnitedStates();
                case Calendar.UnitedStatesFederalReserve:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.FederalReserve);
                case Calendar.UnitedStatesSettlement:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.Settlement);
                case Calendar.UnitedStatesNYSE:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.NYSE);
                case Calendar.UnitedStatesGovernmentBond:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.GovernmentBond);
                case Calendar.UnitedStatesNERC:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.NERC);
                case Calendar.UnitedStatesLiborImpact:
                    return new global::QuantLib.UnitedStates(global::QuantLib.UnitedStates.Market.LiborImpact);
                case Calendar.UnitedKingdom:
                    return new global::QuantLib.UnitedKingdom();
                case Calendar.UnitedKingdomExchange:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Exchange);
                case Calendar.UnitedKingdomMetals:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Metals);
                case Calendar.UnitedKingdomSettlement:
                    return new global::QuantLib.UnitedKingdom(global::QuantLib.UnitedKingdom.Market.Settlement);
                default:
                    throw new ArgumentOutOfRangeException(nameof(calendar), calendar, "Unmapped calendar");
            }
        }
    }
}