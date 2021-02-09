using System;
using System.Collections.Generic;
using System.Linq;
using CfAnalytics.QuantLib.TermStructures;
using QlPeriod = QuantLib.Period;
using QlTime = QuantLib.TimeUnit;
using QlBdc = QuantLib.BusinessDayConvention;
using QlIborIdx = QuantLib.IborIndex;
using QlEuribor = QuantLib.Euribor;
using QlUsdLibor = QuantLib.USDLibor;
using QlGbpLibor = QuantLib.GBPLibor;
using QlChfLibor = QuantLib.CHFLibor;
using QlJpyLibor = QuantLib.JPYLibor;
using QlCdor = QuantLib.Cdor;

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class IborIndexHelper
    {
        #region Index definitions

        internal static QlEuribor Euribor1M = new global::QuantLib.Euribor1M();
        internal static QlEuribor Euribor2M = new global::QuantLib.Euribor2M();
        internal static QlEuribor Euribor3M = new global::QuantLib.Euribor3M();
        internal static QlEuribor Euribor4M = new global::QuantLib.Euribor4M();
        internal static QlEuribor Euribor5M = new global::QuantLib.Euribor5M();
        internal static QlEuribor Euribor6M = new global::QuantLib.Euribor6M();
        internal static QlEuribor Euribor9M = new global::QuantLib.Euribor9M();
        internal static QlEuribor Euribor12M = new global::QuantLib.Euribor1Y();

        internal static QlUsdLibor UsdLibor1M = new QlUsdLibor(new QlPeriod(1, QlTime.Months));
        internal static QlUsdLibor UsdLibor2M = new QlUsdLibor(new QlPeriod(2, QlTime.Months));
        internal static QlUsdLibor UsdLibor3M = new QlUsdLibor(new QlPeriod(3, QlTime.Months));
        internal static QlUsdLibor UsdLibor4M = new QlUsdLibor(new QlPeriod(4, QlTime.Months));
        internal static QlUsdLibor UsdLibor5M = new QlUsdLibor(new QlPeriod(5, QlTime.Months));
        internal static QlUsdLibor UsdLibor6M = new QlUsdLibor(new QlPeriod(6, QlTime.Months));
        internal static QlUsdLibor UsdLibor9M = new QlUsdLibor(new QlPeriod(9, QlTime.Months));
        internal static QlUsdLibor UsdLibor12M = new QlUsdLibor(new QlPeriod(12, QlTime.Months));

        internal static QlGbpLibor GbpLibor1M = new QlGbpLibor(new QlPeriod(1, QlTime.Months));
        internal static QlGbpLibor GbpLibor2M = new QlGbpLibor(new QlPeriod(2, QlTime.Months));
        internal static QlGbpLibor GbpLibor3M = new QlGbpLibor(new QlPeriod(3, QlTime.Months));
        internal static QlGbpLibor GbpLibor4M = new QlGbpLibor(new QlPeriod(4, QlTime.Months));
        internal static QlGbpLibor GbpLibor5M = new QlGbpLibor(new QlPeriod(5, QlTime.Months));
        internal static QlGbpLibor GbpLibor6M = new QlGbpLibor(new QlPeriod(6, QlTime.Months));
        internal static QlGbpLibor GbpLibor9M = new QlGbpLibor(new QlPeriod(9, QlTime.Months));
        internal static QlGbpLibor GbpLibor12M = new QlGbpLibor(new QlPeriod(12, QlTime.Months));

        internal static QlChfLibor ChfLibor1M = new QlChfLibor(new QlPeriod(1, QlTime.Months));
        internal static QlChfLibor ChfLibor2M = new QlChfLibor(new QlPeriod(2, QlTime.Months));
        internal static QlChfLibor ChfLibor3M = new QlChfLibor(new QlPeriod(3, QlTime.Months));
        internal static QlChfLibor ChfLibor4M = new QlChfLibor(new QlPeriod(4, QlTime.Months));
        internal static QlChfLibor ChfLibor5M = new QlChfLibor(new QlPeriod(5, QlTime.Months));
        internal static QlChfLibor ChfLibor6M = new QlChfLibor(new QlPeriod(6, QlTime.Months));
        internal static QlChfLibor ChfLibor9M = new QlChfLibor(new QlPeriod(9, QlTime.Months));
        internal static QlChfLibor ChfLibor12M = new QlChfLibor(new QlPeriod(12, QlTime.Months));

        internal static QlJpyLibor JpyLibor1M = new QlJpyLibor(new QlPeriod(1, QlTime.Months));
        internal static QlJpyLibor JpyLibor2M = new QlJpyLibor(new QlPeriod(2, QlTime.Months));
        internal static QlJpyLibor JpyLibor3M = new QlJpyLibor(new QlPeriod(3, QlTime.Months));
        internal static QlJpyLibor JpyLibor4M = new QlJpyLibor(new QlPeriod(4, QlTime.Months));
        internal static QlJpyLibor JpyLibor5M = new QlJpyLibor(new QlPeriod(5, QlTime.Months));
        internal static QlJpyLibor JpyLibor6M = new QlJpyLibor(new QlPeriod(6, QlTime.Months));
        internal static QlJpyLibor JpyLibor9M = new QlJpyLibor(new QlPeriod(9, QlTime.Months));
        internal static QlJpyLibor JpyLibor12M = new QlJpyLibor(new QlPeriod(12, QlTime.Months));

        internal static SekStibor SekStibor1M = new SekStibor(new QlPeriod(1, QlTime.Months));
        internal static SekStibor SekStibor2M = new SekStibor(new QlPeriod(2, QlTime.Months));
        internal static SekStibor SekStibor3M = new SekStibor(new QlPeriod(3, QlTime.Months));
        internal static SekStibor SekStibor4M = new SekStibor(new QlPeriod(4, QlTime.Months));
        internal static SekStibor SekStibor5M = new SekStibor(new QlPeriod(5, QlTime.Months));
        internal static SekStibor SekStibor6M = new SekStibor(new QlPeriod(6, QlTime.Months));
        internal static SekStibor SekStibor9M = new SekStibor(new QlPeriod(9, QlTime.Months));
        internal static SekStibor SekStibor12M = new SekStibor(new QlPeriod(12, QlTime.Months));

        internal static DkkCibor DkkCibor1M = new DkkCibor(new QlPeriod(1, QlTime.Months));
        internal static DkkCibor DkkCibor2M = new DkkCibor(new QlPeriod(2, QlTime.Months));
        internal static DkkCibor DkkCibor3M = new DkkCibor(new QlPeriod(3, QlTime.Months));
        internal static DkkCibor DkkCibor4M = new DkkCibor(new QlPeriod(4, QlTime.Months));
        internal static DkkCibor DkkCibor5M = new DkkCibor(new QlPeriod(5, QlTime.Months));
        internal static DkkCibor DkkCibor6M = new DkkCibor(new QlPeriod(6, QlTime.Months));
        internal static DkkCibor DkkCibor9M = new DkkCibor(new QlPeriod(9, QlTime.Months));
        internal static DkkCibor DkkCibor12M = new DkkCibor(new QlPeriod(12, QlTime.Months));

        internal static NokNibor NokNibor1M = new NokNibor(new QlPeriod(1, QlTime.Months));
        internal static NokNibor NokNibor2M = new NokNibor(new QlPeriod(2, QlTime.Months));
        internal static NokNibor NokNibor3M = new NokNibor(new QlPeriod(3, QlTime.Months));
        internal static NokNibor NokNibor4M = new NokNibor(new QlPeriod(4, QlTime.Months));
        internal static NokNibor NokNibor5M = new NokNibor(new QlPeriod(5, QlTime.Months));
        internal static NokNibor NokNibor6M = new NokNibor(new QlPeriod(6, QlTime.Months));
        internal static NokNibor NokNibor9M = new NokNibor(new QlPeriod(9, QlTime.Months));
        internal static NokNibor NokNibor12M = new NokNibor(new QlPeriod(12, QlTime.Months));

        internal static QlCdor CadCdor1M = new QlCdor(new QlPeriod(1, QlTime.Months));
        internal static QlCdor CadCdor2M = new QlCdor(new QlPeriod(2, QlTime.Months));
        internal static QlCdor CadCdor3M = new QlCdor(new QlPeriod(3, QlTime.Months));
        internal static QlCdor CadCdor4M = new QlCdor(new QlPeriod(4, QlTime.Months));
        internal static QlCdor CadCdor5M = new QlCdor(new QlPeriod(5, QlTime.Months));
        internal static QlCdor CadCdor6M = new QlCdor(new QlPeriod(6, QlTime.Months));
        internal static QlCdor CadCdor9M = new QlCdor(new QlPeriod(9, QlTime.Months));
        internal static QlCdor CadCdor12M = new QlCdor(new QlPeriod(12, QlTime.Months));

        #endregion

        private static readonly Dictionary<string, QlIborIdx> IDX_DICT = new Dictionary<string, QlIborIdx>
        {
            {"EUR-EURIBOR-1M", Euribor1M},
            {"EUR-EURIBOR-2M", Euribor2M},
            {"EUR-EURIBOR-3M", Euribor3M},
            {"EUR-EURIBOR-4M", Euribor4M},
            {"EUR-EURIBOR-5M", Euribor5M},
            {"EUR-EURIBOR-6M", Euribor6M},
            {"EUR-EURIBOR-9M", Euribor9M},
            {"EUR-EURIBOR-12M", Euribor12M},

            {"USD-LIBOR-1M", UsdLibor1M},
            {"USD-LIBOR-2M", UsdLibor2M},
            {"USD-LIBOR-3M", UsdLibor3M},
            {"USD-LIBOR-4M", UsdLibor4M},
            {"USD-LIBOR-5M", UsdLibor5M},
            {"USD-LIBOR-6M", UsdLibor6M},
            {"USD-LIBOR-9M", UsdLibor9M},
            {"USD-LIBOR-12M", UsdLibor12M},

            {"GBP-LIBOR-1M", GbpLibor1M},
            {"GBP-LIBOR-2M", GbpLibor2M},
            {"GBP-LIBOR-3M", GbpLibor3M},
            {"GBP-LIBOR-4M", GbpLibor4M},
            {"GBP-LIBOR-5M", GbpLibor5M},
            {"GBP-LIBOR-6M", GbpLibor6M},
            {"GBP-LIBOR-9M", GbpLibor9M},
            {"GBP-LIBOR-12M", GbpLibor12M},

            {"CHF-LIBOR-1M", ChfLibor1M},
            {"CHF-LIBOR-2M", ChfLibor2M},
            {"CHF-LIBOR-3M", ChfLibor3M},
            {"CHF-LIBOR-4M", ChfLibor4M},
            {"CHF-LIBOR-5M", ChfLibor5M},
            {"CHF-LIBOR-6M", ChfLibor6M},
            {"CHF-LIBOR-9M", ChfLibor9M},
            {"CHF-LIBOR-12M", ChfLibor12M},

            {"JPY-LIBOR-1M", JpyLibor1M},
            {"JPY-LIBOR-2M", JpyLibor2M},
            {"JPY-LIBOR-3M", JpyLibor3M},
            {"JPY-LIBOR-4M", JpyLibor4M},
            {"JPY-LIBOR-5M", JpyLibor5M},
            {"JPY-LIBOR-6M", JpyLibor6M},
            {"JPY-LIBOR-9M", JpyLibor9M},
            {"JPY-LIBOR-12M", JpyLibor12M},

            {"SEK-STIBOR-1M", SekStibor1M},
            {"SEK-STIBOR-2M", SekStibor2M},
            {"SEK-STIBOR-3M", SekStibor3M},
            {"SEK-STIBOR-4M", SekStibor4M},
            {"SEK-STIBOR-5M", SekStibor5M},
            {"SEK-STIBOR-6M", SekStibor6M},
            {"SEK-STIBOR-9M", SekStibor9M},
            {"SEK-STIBOR-12M", SekStibor12M},

            {"DKK-CIBOR-1M", DkkCibor1M},
            {"DKK-CIBOR-2M", DkkCibor2M},
            {"DKK-CIBOR-3M", DkkCibor3M},
            {"DKK-CIBOR-4M", DkkCibor4M},
            {"DKK-CIBOR-5M", DkkCibor5M},
            {"DKK-CIBOR-6M", DkkCibor6M},
            {"DKK-CIBOR-9M", DkkCibor9M},
            {"DKK-CIBOR-12M", DkkCibor12M},

            {"NOK-NIBOR-1M", NokNibor1M},
            {"NOK-NIBOR-2M", NokNibor2M},
            {"NOK-NIBOR-3M", NokNibor3M},
            {"NOK-NIBOR-4M", NokNibor4M},
            {"NOK-NIBOR-5M", NokNibor5M},
            {"NOK-NIBOR-6M", NokNibor6M},
            {"NOK-NIBOR-9M", NokNibor9M},
            {"NOK-NIBOR-12M", NokNibor12M},

            {"CAD-CDOR-1M", CadCdor1M},
            {"CAD-CDOR-2M", CadCdor2M},
            {"CAD-CDOR-3M", CadCdor3M},
            {"CAD-CDOR-4M", CadCdor4M},
            {"CAD-CDOR-5M", CadCdor5M},
            {"CAD-CDOR-6M", CadCdor6M},
            {"CAD-CDOR-9M", CadCdor9M},
            {"CAD-CDOR-12M", CadCdor12M}
        };

        internal static QlIborIdx GetIborIndexInternal(string iborIndex, global::QuantLib.YieldTermStructureHandle forwardingCurve)
        {
            if (forwardingCurve == null)
            {
                if (IDX_DICT.ContainsKey(iborIndex))
                    return IDX_DICT[iborIndex];
                throw new ArgumentOutOfRangeException(nameof(iborIndex), iborIndex, "Unmapped Ibor Index");
            }

            switch (iborIndex)
            {
                case "EUR-EURIBOR-1M": return new global::QuantLib.Euribor1M(forwardingCurve);
                case "EUR-EURIBOR-2M": return new global::QuantLib.Euribor2M(forwardingCurve);
                case "EUR-EURIBOR-3M": return new global::QuantLib.Euribor3M(forwardingCurve);
                case "EUR-EURIBOR-4M": return new global::QuantLib.Euribor4M(forwardingCurve);
                case "EUR-EURIBOR-5M": return new global::QuantLib.Euribor5M(forwardingCurve);
                case "EUR-EURIBOR-6M": return new global::QuantLib.Euribor6M(forwardingCurve);
                case "EUR-EURIBOR-9M": return new global::QuantLib.Euribor9M(forwardingCurve);
                case "EUR-EURIBOR-12M": return new global::QuantLib.Euribor1Y(forwardingCurve);
                default: throw new ArgumentOutOfRangeException(nameof(iborIndex), iborIndex, "Unmapped Ibor Index");
            }
        }

        public static IborIndex GetIborIndex(string iborIndex, YieldTermStructure forwardingCurve)
        {
            var idx = GetIborIndexInternal(iborIndex, forwardingCurve?.GetHandle());
            return idx != null ? new IborIndex(idx) : null;
        }

        public static Dictionary<string, string> GetReverseDict(string type)
        {
            switch (type.ToLower())
            {
                case "qlname":
                    return IDX_DICT.Select(e => new {Key = e.Value.__str__(), Value = e.Key}).ToDictionary(ks => ks.Key, es => es.Value);
                default:
                    return IDX_DICT.Select(e => new {Key = $"{e.Value.currency()}-{e.Value.familyName()}-{e.Value.tenor().__repr__()}", Value = e.Key})
                                   .ToDictionary(ks => ks.Key, es => es.Value);
            }
        }
    }

    internal class SekStibor : QlIborIdx
    {
        internal SekStibor(QlPeriod tenor)
            : base("Stibor", tenor, 2, new global::QuantLib.SEKCurrency(), new global::QuantLib.Sweden(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360())
        {
        }

        internal SekStibor(QlPeriod tenor, global::QuantLib.YieldTermStructureHandle h)
            : base("Stibor", tenor, 2, new global::QuantLib.SEKCurrency(), new global::QuantLib.Sweden(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360(), h)
        {
        }
    }

    internal class DkkCibor : QlIborIdx
    {
        internal DkkCibor(QlPeriod tenor)
            : base("Cibor", tenor, 2, new global::QuantLib.DKKCurrency(), new global::QuantLib.Denmark(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360())
        {
        }

        internal DkkCibor(QlPeriod tenor, global::QuantLib.YieldTermStructureHandle h)
            : base("Cibor", tenor, 2, new global::QuantLib.DKKCurrency(), new global::QuantLib.Denmark(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360(), h)
        {
        }
    }

    internal class NokNibor : QlIborIdx
    {
        internal NokNibor(QlPeriod tenor)
            : base("Nibor", tenor, 2, new global::QuantLib.NOKCurrency(), new global::QuantLib.Norway(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360())
        {
        }

        internal NokNibor(QlPeriod tenor, global::QuantLib.YieldTermStructureHandle h)
            : base("Nibor", tenor, 2, new global::QuantLib.NOKCurrency(), new global::QuantLib.Norway(), QlBdc.ModifiedFollowing, false, new global::QuantLib.Actual360(), h)
        {
        }
    }
}