using System;

// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public enum DayCounter
    {
        ActualActual,
        ActualActualISMA,
        ActualActualBond,
        ActualActualISDA,
        ActualActualHistorical,
        ActualActualActual365,
        ActualActualAFB,
        ActualActualEuro,

        Actual365Fixed,
        Actual365FixedStandard,
        Actual365FixedCanadian,
        Actual365FixedNoLeap,
        
        Actual360,
        Actual360LD,

        Thirty360,
        Thirty360BondBasis,
        Thirty360EurobondBasis,
        Thirty360European,
        //Thirty360German,
        Thirty360Italian,
        Thirty360USA,
    }

    internal static class DayCounterExt
    {
        internal static global::QuantLib.DayCounter ToQlDayCounter(this DayCounter dayCounter)
        {
            switch (dayCounter)
            {
                case DayCounter.ActualActual:
                    return new global::QuantLib.ActualActual();
                case DayCounter.ActualActualISMA:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.ISMA);
                case DayCounter.ActualActualBond:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.Bond);
                case DayCounter.ActualActualISDA:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.ISDA);
                case DayCounter.ActualActualHistorical:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.Historical);
                case DayCounter.ActualActualActual365:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.Actual365);
                case DayCounter.ActualActualAFB:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.AFB);
                case DayCounter.ActualActualEuro:
                    return new global::QuantLib.ActualActual(global::QuantLib.ActualActual.Convention.Euro);
                case DayCounter.Actual365Fixed:
                    return new global::QuantLib.Actual365Fixed();
                case DayCounter.Actual365FixedStandard:
                    return new global::QuantLib.Actual365Fixed(global::QuantLib.Actual365Fixed.Convention.Standard);
                case DayCounter.Actual365FixedCanadian:
                    return new global::QuantLib.Actual365Fixed(global::QuantLib.Actual365Fixed.Convention.Canadian);
                case DayCounter.Actual365FixedNoLeap:
                    return new global::QuantLib.Actual365Fixed(global::QuantLib.Actual365Fixed.Convention.NoLeap);
                case DayCounter.Actual360:
                    return new global::QuantLib.Actual360();
                case DayCounter.Actual360LD:
                    return new global::QuantLib.Actual360(true);
                case DayCounter.Thirty360:
                    return new global::QuantLib.Thirty360();
                case DayCounter.Thirty360BondBasis:
                    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.BondBasis);
                case DayCounter.Thirty360EurobondBasis:
                    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.EurobondBasis);
                case DayCounter.Thirty360European:
                    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.European);
                //case DayCounter.Thirty360German:
                //    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.German);
                case DayCounter.Thirty360Italian:
                    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.Italian);
                case DayCounter.Thirty360USA:
                    return new global::QuantLib.Thirty360(global::QuantLib.Thirty360.Convention.USA);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayCounter), dayCounter, null);
            }
        }
    }
}